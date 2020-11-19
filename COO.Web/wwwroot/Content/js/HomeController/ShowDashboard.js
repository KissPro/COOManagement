var home = {
    init: function () {
        home.regEvents();
    },
    //Show detail images
    regEvents: function () {
        //Show STT
        var date = new Date().toLocaleDateString();

        var t = $('#dataTable').DataTable({
            //rowReorder: {
            //    selector: 'td:nth-child(8)'
            //},
            stateSave: true,
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
                    "targets": 13,
                    "searchable": true,
                    "visible": false,
                }
            ],
            buttons: [
                {
                    extend: 'excel',
                    filename: "GR Incoming List-" + date
                }
            ],
            //"order": [[1, 'asc']],
            "autoWidth": false,
            "initComplete": () => { $("#dataTable").show(); $("#divLoader").fadeOut(300); }

            //// Oh Kia
            //,"bStateSave": true,
            //"fnStateSave": function (oSettings, oData) {
            //    localStorage.setItem('offersDataTables', JSON.stringify(oData));
            //},
            //"fnStateLoad": function (oSettings) {
            //    return JSON.parse(localStorage.getItem('offersDataTables'));
            //}
        });
        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        // Export excel
        $('#btnExport').off('click').on('click', function (e) {
            if (confirm('Do you want export this table?')) {
                table.button('.buttons-excel').trigger();
            }
        });
        countTotal();


        // Search confirmdate range
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {

                var dateMinFormat = $('#datetime').val().toString().split("/");
                var dateMaxFormat = $('#datetime1').val().toString().split("/");

                var min = new Date(dateMinFormat[2] + "/" + dateMinFormat[1] + "/" + dateMinFormat[0]);
                var max = new Date(dateMaxFormat[2] + "/" + dateMaxFormat[1] + "/" + dateMaxFormat[0]);

                var date = new Date(data[6]) || 0; // use data for the age column
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
             //Check TO status
            var dep = $('#selectDepartment').val();
            var searchString = [];
            if ($('#ckOpen:checked').val() == 'on') {
                searchString.push('^\s*$');
            }
            if ($('#ckRequest:checked').val() == 'on') {
                searchString.push('Requested');
            }
            if ($('#ckConfirm:checked').val() == 'on') {
                searchString.push('Received');
            }

            table.column(13).every(function () {
                var that = this;
                if (that.search() !== dep) {
                    that
                        //.search(dep)
                        .search(searchString.join('|'), true, false).draw();
                }
            });

            table.draw();
        });

        $('#btnClear').off('click').on('click', function () {
            $("#datetime").datepicker().val('');
            $("#datetime1").datepicker().val('');
            $('#btnFilter').click();
        });


        // Select Type
        // Event listener to the two range filtering inputs to redraw on input
        var table = $('#dataTable').DataTable();

        // Update in GR list
        setInterval(function () {
            var id = parseInt(localStorage['updateId']) || 0;
            var type = localStorage['updateType'] || null;
            if (id != 0 && type != null) {
                if (type === 'requestTO') {
                    $('tr[data-id="' + id + '"]').removeClass('table-primary');
                    $('tr[data-id="' + id + '"]').removeClass('table-danger');
                    $('tr[data-id="' + id + '"]').toggleClass('table-primary');
                }
                else if (type === 'confirmTO') {
                    $('tr[data-id="' + id + '"]').removeClass('table-primary');
                    $('tr[data-id="' + id + '"]').removeClass('table-danger');
                    $('tr[data-id="' + id + '"]').toggleClass('table-success');
                }
                else if (type === 'submitResult') {
                    $('tr[data-id="' + id + '"]').remove();
                }
                // set update id null.
                localStorage.setItem('updateId', null);
                countTotal();
            }
        }, 500);


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
                data: { id: id },
                url: '/Home/DetailGR',
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
            var totalConfirm = $('#dataTable').DataTable().rows('.table-success')[0].length;
            var totalRequest = $('#dataTable').DataTable().rows('.table-primary')[0].length;
            var totalOpen = totalRecords - (totalConfirm + totalRequest);
            // Set value
            $('.entriesTotal').text(totalRecords)
            $('.confirmTotal').text(totalConfirm)
            $('.requestTotal').text(totalRequest)
            $('.openTotal').text(totalOpen)
        }
    }
}
home.init();