var home = {
    init: function () {
        home.regEvents();
    },

    //Show detail images
    regEvents: function () {
        var date = new Date().toLocaleDateString();
        //Show STT
        var t = $('#dataTable').DataTable({
            responsive: true,
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0,
            }
            ],
            buttons: [
                {
                    extend: 'excel',
                    filename: "Material List-" + date
                }
            ],
            "processing": true,
            "order": [[1, 'asc']],
            "autoWidth": false,
            "initComplete": () => { $("#dataTable").show(); $("#divLoader").fadeOut(3000); }
        });

        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        var table = $('#dataTable').DataTable()
        // Export excel
        $('#btnExport').off('click').on('click', function (e) {
            if (confirm('Do you want export this table?')) {
                table.button('.buttons-excel').trigger();
            }
        });

        // Select Type
        // Event listener to the two range filtering inputs to redraw on input
        //var table = $('#dataTable').DataTable()


        $('.btnImport').on('click', function () {
            $("#filename").trigger("click", function (e) {
            });
        });

        $("#filename").change(function (e) {
            var ext = $("input#filename").val().split(".").pop().toLowerCase();
            if ($.inArray(ext, ["xlsx"]) == -1) {
                alert('Upload File.xlsx');
                return false;
            }
            else {
                var fileName = e.target.files[0].name;
                $('.showResult').text(fileName);
                $('.showResult').addClass('text-danger');
            }
        });

        // edit
        $('#dataTable tbody').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $("#divLoader").show();
            $('#myModalEdit').html('');
            var id = $(this).data('id');
            $.ajax({
                data: { id: id },
                url: '/Material/Edit',
                type: 'GET',
                success: function (res) {
                    $('.modal').modal('hide');
                    $('#myModalEdit').append(res).find('.clickModel').click();
                    $("#divLoader").fadeOut(300);
                }
            })
        });

        // issue
        $('#dataTable tbody').on('click', '.btn-issue', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $("#divLoader").show();
            $('#myModalIssue').html('');
            var id = $(this).data('id');
            $.ajax({
                data: { id: id },
                url: '/Material/Issue',
                type: 'GET',
                success: function (res) {
                    $('.modal').modal('hide');
                    $('#myModalIssue').append(res).find('.clickModel').click();
                    $("#divLoader").fadeOut(300);
                }
            })
        });

        ///// DELETE - APPROVAL

        //// Show
        //$('#exampleModal').on('show.bs.modal', function (event) {
        //    var modal = $(this);
        //    modal.find('.modal-title').text('Delete Request Form');
        //    // Call reception
        //    $.ajax({
        //        url: '/Request/GetListReception',
        //        dataType: 'json',
        //        type: 'POST',
        //        success: function (res) {
        //            if (res.result != false) {
        //                modal.find('.modal-body input').val(res.result);
        //            }
        //            else {
        //                modal.find('.modal-body input').val('Your Head Email!');
        //            }
        //        }
        //    })
        //})

        //// Submit
        //$('#btnSubmit').off('click').on('click', function (e) {
        //    var id = $(this).data('id');
        //    var listReceptions = $('#recipient-name').val();
        //    var reason = $('#message-text').val();
        //    if (reason != "") {
        //        $.ajax({
        //            data: { id: id, listReceptions: listReceptions, reason: reason },
        //            url: '/Request/DeleteRequest',
        //            dataType: 'json',
        //            type: 'POST',
        //            success: function (res) {
        //                if (res.status == true) {
        //                    $('#exampleModal').modal('toggle');
        //                    $('#message-text').val('');
        //                    setTimeout(
        //                        function () {
        //                            alert("Delete request successfully!");
        //                            window.location.href = "/my-request";
        //                        }, 1000);
        //                }
        //                else {
        //                    alert("Delete request false!");
        //                    $('#message-text').val('');
        //                }
        //            }
        //        })
        //    }
        //    else {
        //        alert("Please fill your reason for request");
        //        $('#message-text').focus();
        //    }
        //});
    }
}

home.init();