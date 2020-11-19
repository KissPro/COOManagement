$(document).ready(function () {
    $(function () {
        $('.amount').on("keyup", function (event) {
            // When user select text in the document, also abort.
            var selection = window.getSelection().toString();
            if (selection !== '') {
                return;
            }

            // When the arrow keys are pressed, abort.
            if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
                return;
            }

            var $this = $(this);

            // Get the value.
            var input = $this.val();

            var input = input.replace(/[\D\s\._\-]+/g, "");
            input = input ? parseInt(input, 10) : 0;

            $this.val(function () {
                return (input === 0) ? "" : input.toLocaleString("en-US");
            });
        });


        // Change to number when upload 
        $('#requestDetail').submit(function (e) {
            // convert string money to decimal
            $('.amount').each(function () {
                $(this).val($(this).val().replace(/[($)\s\,_\-]+/g, ''));
            });

            // disable all content row to submit
            $('#dataTable tr').find("input,button,textarea,select,span").removeAttr("disabled", "disabled");
            return true;
        });
    });
});