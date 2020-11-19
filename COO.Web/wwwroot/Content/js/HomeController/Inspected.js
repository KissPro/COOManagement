var home = {
    init: function () {
        home.regEvents();
    },

    //Show detail images
    regEvents: function () {
        var date = new Date().toLocaleDateString();
        //Show STT
        var t = $('#dataTable').DataTable({
            //rowReorder: {
            //    selector: 'td:nth-child(2)'
            //},
            responsive: true,
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0,
            }
            ],
            // hide status column
            "columnDefs": [
                {
                    "targets": 12,
                    "searchable": true,
                    "visible": false,
                }
            ],
            buttons: [
                {
                    extend: 'excel',
                    filename: "Inspected List-" + date
                }
            ],
            //"order": [[1, 'asc']],
            "autoWidth": false,
            "initComplete": () => { $("#dataTable").show(); $("#divLoader").fadeOut(300); }
        });
        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
        countTotal();

        // Export excel
        $('#btnExport').off('click').on('click', function (e) {
            if (confirm('Do you want export this table?')) {
                table.button('.buttons-excel').trigger();
            }
        });


        // Update in GR list
        setInterval(function () {
            var id = parseInt(localStorage['updateId']) || 0;
            var type = localStorage['updateType'] || null;
            if (id != 0 && type != null) {
                if (type === 'createTO') {
                    $('tr[data-id="' + id + '"]').removeClass('table-success');
                    $('tr[data-id="' + id + '"]').toggleClass('table-warning');
                }
                if (type === 'Inspected') {
                    window.location.href = "/inspected";
                }
                // set update id null.
                localStorage.setItem('updateId', null);
                countTotal();
            }
        }, 500);



        //$('#dataTable').DataTable({
        //    columnDefs: [
        //        { "width": "15rem", "targets": [3] },
        //    ]
        //});

        //$('#dataTable tbody').on('click', '.btnDetail', function () {
        //    var id = $(this).data('id');
        //    console.log(id);
        //    window.location.href = "/request-detail/" + id + "/" + 1;
        //});

        //$('#btnDetail').off('click').on('click', function () {
        //    var id = $(this).data('id');
        //    console.log(id);
        //    window.location.href = "/request-detail/" + id + "/" + 1;
        //});

        // Search confirmdate range
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var dateMinFormat = $('#datetime').val().toString().split("/");
                var dateMaxFormat = $('#datetime1').val().toString().split("/");

                var min = new Date(dateMinFormat[2] + "/" + dateMinFormat[1] + "/" + dateMinFormat[0]);
                var max = new Date(dateMaxFormat[2] + "/" + dateMaxFormat[1] + "/" + dateMaxFormat[0]);

                var date = new Date(data[9]) || 0; // use data for the age column
                max.setHours(23, 59, 59, 999);

                if ((isNaN(min) && isNaN(max)) ||
                    (isNaN(min) && date <= max) ||
                    (min <= date && isNaN(max)) ||
                    (min <= date && date <= max)) {
                    return true;
                }
                return false;
            }
        );

        $('#btnFilter').off('click').on('click', function () {
            // Check TO status
            var dep = $('#selectDepartment').val();
            var searchString = [];
            if ($('#ckInspectedOK:checked').val() == 'on') {
                searchString.push('1');
            }
            if ($('#ckInspectedNG:checked').val() == 'on') {
                searchString.push('0');
            }
            if ($('#ckReturned:checked').val() == 'on') {
                searchString.push('2');
            }
            var hoang = searchString.join('|');
            table.column(12).every(function () {
                var that = this;
                if (that.search() !== dep) {
                    that
                        .search(searchString.join('|'), true, false).draw();
                }
            });

            table.draw();
        });


        // Select Type
        // Event listener to the two range filtering inputs to redraw on input
        var table = $('#dataTable').DataTable()

        $('#dataTable tbody').on('click', 'tr', function () {
            $(this).toggleClass('table-danger');
        });

        $('#btnMultipleDel').click(function () {
            var listDelete = new Array();
            $('.table-secondary td:nth-child(2)').each(function () {
                listDelete.push($(this).data('id'));
            });
            if (listDelete != '' && listDelete != null) {
                if (confirm('Do you want delete this selection?')) {
                    $("#divLoader").show();

                    $.ajax({
                        data: { listRequest: listDelete },
                        url: '/Request/DeleteListRequest',
                        dataType: 'json',
                        type: 'POST',
                        success: function (res) {
                            if (res.status == true) {
                                alert("Delete list requests successfully!");
                            }
                            else {
                                alert("Error, can't delete list requests, please contact with IT!");
                            }
                            window.location.href = "/my-request";
                            $("#divLoader").fadeOut(300);
                        }
                    })
                }
            }
        });


      
        $('#btnClear').off('click').on('click', function () {
            $("#datetime").datepicker().val('');
            $("#datetime1").datepicker().val('');
            $('#btnFilter').click();
        });

        $('#dataTable tbody').on('click', '.btn-delete', function (e) {
            if (confirm('Do you want delete this request?')) {
                e.preventDefault();
                e.stopPropagation();
                $("#divLoader").show();
                var id = $(this).data('id');
                $.ajax({
                    data: { id: id },
                    url: '/Request/DeleteRequest',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            alert("Delete request successfully!");
                            window.location.href = "/my-request";
                        }
                        else {
                            alert("Delete request false!");
                        }
                        $("#divLoader").fadeOut(300);
                    }
                })
            }
        });

        $('.btnImport').on('click', function () {
            $("#filename").trigger("click", function (e) {
            });
        });

        $("#filename").change(function (e) {
            var ext = $("input#filename").val().split(".").pop().toLowerCase();
            if (ext !== 'xls') {
                alert('Upload File.xls');
                return false;
            }
            else {
                var fileName = e.target.files[0].name;
                $('.showResult').text(fileName);
                $('.showResult').addClass('text-danger');
            }
        });


        $('#dataTable tbody').on('click', '.btn-detail', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $("#divLoader").show();
            $('#detailLot').empty();
            var id = $(this).data('id');
            $.ajax({
                //data: { id: id },
                url: '/inspected/editGR/' + id,
                type: 'GET',
                success: function (res) {
                    $('#detailLot').append(res).find('.clickModel').click();
                    $("#divLoader").fadeOut(300);
                }
            })
        });


        // Count total function.
        function countTotal() {
            var totalRecords = $('#dataTable').dataTable().fnGetData().length;
            var totalOK = $('#dataTable').DataTable().rows('.table-success')[0].length;
            var totalReturn = $('#dataTable').DataTable().rows('.table-warning')[0].length;
            var totalNG = totalRecords - (totalOK + totalReturn);
            // Set value
            $('.entriesTotal').text(totalRecords)
            $('.okTotal').text(totalOK)
            $('.ngTotal').text(totalNG)
            $('.returnTotal').text(totalReturn)
        }
    }
}
home.init();