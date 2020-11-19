var home = {
    init: function () {
        home.regEvents();
    },
    //Show detail images
    regEvents: function () {
        //Show STT
        var t = $('#dataTable').DataTable({
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0
            }],
            "order": [[1, 'asc']]
        });
        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        $('#dataTable tbody').on('click', '.btn-delete', function (e) {
            if (confirm('Do you want delete this visitor?')) {
                e.preventDefault();
                $("#divLoader").show();
                var id = $(this).data('id');
                $.ajax({
                    data: { nationalId: id },
                    url: '/Setting/BlackListDelete',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            alert("Delete visitor in blacklist successfully!");
                        }
                        else {
                            alert("Delete visitor in blacklist error!");

                        }
                        window.location.href = "/black-list";
                        $("#divLoader").fadeOut(300);
                    }
                })
            }
        });

        $('#dataTable tbody').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            $(window).scrollTop(0);
            // Change name of button
            $('.btn-add').val('Update');
            // Change tabs selection -> single
            $('ul.tabs li').removeClass('current');
            $('.tab-content').removeClass('current');
            $('ul.tabs li:first-child').addClass('current');
            $("#tab-1").addClass('current');

            var id = $(this).data('id');
            $('#visitorName').val($('#' + id + ' td:nth-child(2)').text());
            $('#visitorCompany').val($('#' + id + ' td:nth-child(3)').text());
            $('#nationalId').val($('#' + id + ' td:nth-child(4)').text());
            $('#reason').text($('#' + id + ' td:nth-child(5)').text());
            $('#nationalId').prop('readonly', true);
        });

        $('.btn-reset').off('click').on('click', function (e) {
            $('#visitorName').val('');
            $('#visitorCompany').val('');
            $('#nationalId').val('');
            $('#reason').text('');
            $('#nationalId').prop('readonly', false);
            $('.btn-add').val('Add');
        });

        $('.btn-add').off('click').on('click', function (e) {
            e.preventDefault();
            var visitor = $('#visitorName').val();
            var company = $('#visitorCompany').val();
            var nationalId = $('#nationalId').val();
            var reason = $('#reason').val();
            if (nationalId != null && nationalId != '') {
                $("#divLoader").show();
                $.ajax({
                    data: { visitor: visitor, company: company, nationalId: nationalId, reason: reason },
                    url: '/Setting/BlackListInsertOrUpdate',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            alert("Update visitor in blacklist successfully!");
                            $('.btn-add').val('Add');
                        }
                        else {
                            alert("Update visitor in blacklist error!");

                        }
                        window.location.href = "/black-list";
                        $("#divLoader").fadeOut(300);
                    }
                })
            }
        });
    }
}
home.init();