var home = {
    init: function () {
        home.regEvents();
    },
    //Show detail images
    regEvents: function () {
        //Show STT
        var t = $('#dataTable').DataTable({
            rowReorder: {
                selector: 'td:nth-child(2)'
            },
            responsive: true,
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0,
            }
            ],
            "order": [[1, 'asc']],
            "autoWidth": false
        });
        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

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
                var min = new Date($('.form_datetime').val());
                var max = new Date($('.form_datetime1').val());
                var date = new Date(data[4]) || 0; // use data for the age column
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

        // Select Type
        // Event listener to the two range filtering inputs to redraw on input
        var table = $('#dataTable').DataTable()

        $('#dataTable tbody').on('click', 'tr', function () {
            $(this).toggleClass('table-secondary');
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


        $('#btnFilter').off('click').on('click', function () {
            table.draw();
        });

        $('#btnClear').off('click').on('click', function () {
            $(".form_datetime").datepicker().val('');
            $(".form_datetime1").datepicker().val('');
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

        /// DELETE - APPROVAL

        // Show
        $('#exampleModal').on('show.bs.modal', function (event) {
            var modal = $(this);
            modal.find('.modal-title').text('Delete Request Form');
            // Call reception
            $.ajax({
                url: '/Request/GetListReception',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.result != false) {
                        modal.find('.modal-body input').val(res.result);
                    }
                    else {
                        modal.find('.modal-body input').val('Your Head Email!');
                    }
                }
            })
        })

        // Submit
        $('#btnSubmit').off('click').on('click', function (e) {
            var id = $(this).data('id');
            var listReceptions = $('#recipient-name').val();
            var reason = $('#message-text').val();
            if (reason != "") {
                $.ajax({
                    data: { id: id, listReceptions: listReceptions, reason: reason },
                    url: '/Request/DeleteRequest',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            $('#exampleModal').modal('toggle');
                            $('#message-text').val('');
                            setTimeout(
                                function () {
                                    alert("Delete request successfully!");
                                    window.location.href = "/my-request";
                                }, 1000);
                        }
                        else {
                            alert("Delete request false!");
                            $('#message-text').val('');
                        }
                    }
                })
            }
            else {
                alert("Please fill your reason for request");
                $('#message-text').focus();
            }
        });
    }
}
home.init();