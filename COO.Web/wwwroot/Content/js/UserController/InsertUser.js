var insertUser = {
    init: function () {
        insertUser.registerEvents();
    },
    registerEvents: function () {
        $('#btnCheck').off('click').on('click', function (e) {
            e.preventDefault();
            var id = $('#inputEmployeeId');
            var name = $('#inputName');
            console.log(id)
            $.ajax({
                data: { userId: id.val() },
                url: '/User/CheckUser',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.result == false) {
                        alert("This employee id don't exits, Please recheck!");
                        id.val('');
                        name.val('');
                    }
                    else {
                        name.val(res.result);
                    }
                }
            })
        });
    }
}
insertUser.init();