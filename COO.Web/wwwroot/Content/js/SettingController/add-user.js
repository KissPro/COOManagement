var insertUser = {
    init: function () {
        insertUser.registerEvents();
    },

    registerEvents: function () {
        $('#btnCheck').off('click').on('click', function (e) {
            e.preventDefault();
            $("#divLoader").show();
            var id = $('#inputEmployeeId');
            var department = $('#inputDepartment :selected').text() 
            console.log(id)
            $.ajax({
                data: { userId: id.val(), dep: department},
                url: '/Setting/AddUser',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.result == false) {
                        alert("This employee id don't exits, Please recheck!");
                        id.val('');
                        id.focus();
                    }
                    else {
                        alert("Add employee successfully!");
                        var res = department.replace("&", "%26");
                        window.location.href = "/Setting/Index?Department=" + res;
                    }
                    $("#divLoader").fadeOut(300);
                }
            })
        });

        $('.btn-delete').off('click').on('click', function (e) {
            if (confirm('Do you want delete this user?')) {
                e.preventDefault();
                $("#divLoader").show();
                var btn = $(this);
                var id = btn.data('id');
                var department = $('#inputDepartment :selected').text() 
                // Get list deputyId without choosing delete
                var list = new Array();
                $('td:first-child').each(function () {
                    if ($(this).text() != id) {
                        list.push($(this).text());
                        console.log($(this).text());
                    }
                });
                $.ajax({
                    data: { listDeputy: list.join(';'), id: id, dep: department },
                    url: '/Setting/Delete',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            var res = department.replace("&", "%26");
                            window.location.href = "/Setting/Index?Department=" + res;
                            //console.log("thanh cong roi nhe!");
                        }
                        $("#divLoader").fadeOut(300);
                    }
                })
            }
        });
        // DropDownList on change
        $('#inputDepartment').change(function () {
            var selectedValue = $(this).val();
            var res = selectedValue.replace("&", "%26");
            window.location.href = "/Setting/Index?Department=" + res;
        });
    }
}
insertUser.init();