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


        $('#dataTable tbody').on('click', '.btnUpdate', function () {
            $("#divLoader").show();
            var id = $(this).data('id');
            var roleName = $(this).data('role');
            var role = $('#' + id + roleName + ' :selected').val();

            var type = $(this).data('type');
            if (type == "update") {
                if (role != null && role != "") {
                    $.ajax({
                        data: { employeeId: id, role: role },
                        url: '/Setting/UpdatePermission',
                        dataType: 'json',
                        type: 'POST',
                        success: function (res) {
                            if (res) {
                                alert("Update Successfully");
                            }
                            else {
                                alert("Update False");
                            }
                        }
                    });
                }
            }
            else {
                if (role == '' || role == null) {
                    role = $('#' + id + roleName + ' option:contains(' + roleName + ')').filter(":gt(0)").val();
                    role1 = $('#V1517299Host option:contains(Host)').filter(":gt(0)").val();
                    console.log(role);
                    console.log(role1);
                }
                $.ajax({
                    data: { employeeId: id, role: role },
                    url: '/Setting/DeletePermission',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res) {
                            alert("Delete Successfully");
                        }
                        else {
                            alert("Update False");
                        }
                        $("#divLoader").fadeOut(300);
                    }
                });
            }
            window.location.href = "/user-role";
        });
    }
}
home.init();