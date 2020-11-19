$(document).ready(function () {
    var images = {
        init: function () {
            images.regEvents();
        },
        //Delete all list images
        regEvents: function () {
            var modal = null;
            $('.myImg').off('click').on('click', function (e) {
                id = $(this).data('id');

                var modalImg = document.getElementById("img01+" + id);
                modal = document.getElementById("myModal+" + id);
                var captionText = document.getElementById("caption+" + id);
                modal.style.display = "block";
                modalImg.src = this.src;
                captionText.innerHTML = this.alt;
            });

            $('.closeImages').off('click').on('click', function (e) {
                modal.style.display = "none";
            });
        }
    }
    images.init()
});


;