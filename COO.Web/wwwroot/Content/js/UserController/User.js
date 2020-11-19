var user = {
    init: function () {
        user.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            console.log(id);
            $.ajax({
                url: "/User/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Active');
                    }
                    else {
                        btn.text('Lock');
                    }
                }
            });
        });

        $('.btn-delete').off('click').on('click', function (e) {
            if (confirm('Do you want delete this user?')) {
                e.preventDefault();
                var btn = $(this);
                var id = btn.data('id');
                console.log(id)
                $.ajax({
                    data: { id: id },
                    url: '/User/Delete',
                    dataType: 'json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/User/ShowUser";
                        }
                    }
                })
            }
        });
    }
}
user.init();