$(document).ready(function () {
    // INIT
    $('.btnRequest').prop('disabled', true);

    // EVENT
    $('.checkbox').change(function () {
        if (this.checked) {
            $('.btnRequest').prop('disabled', false);
            $('.vqBackground').remove();
            $('.fcBackground').remove();
        }
    });

    //$(document).on("click", ".submit", function (e) {
    $(".submit").on("click", function (e) {

        debugger;
        if (!$('form')[0].checkValidity()) {
            $('form').find('input[type="submit"]').click();
            return false;
        }


        // Check Value
        var check = "";
        var listChecked = $('.loadData').find(':checkbox:checked');
        var maxSample = 0; count = 0;

        debugger;
        // get type of submit
        var type = $(this).val();
        if (type == 'Inspected') {
            $('.stockReturn').removeAttr('required');
        }

        // Check stock return
        $('.stockReturn[required="true"]').each(function () {
            if ($.trim($(this).val()) == '') {
                check = "Warning, You must input returned stock!";
            }
        });


        // No need create TO
        var notTo = $('#notTO').val();
        // Check total > max sample

        var vqSample = parseInt($('.vqSample').text()) || 0; // if null value = 0
        var fcSample = parseInt($('.fcSample').text()) || 0;
        maxSample = Math.max(vqSample, fcSample);

        // count total value
        listChecked.each(function () {
            count = count + parseInt($(this).closest("tr").find('td:nth-child(3)').text());
        });

        if (count < maxSample && notTo != "True") {
            check = "Warning, Not enought total stock for inspection!";
        }

        if (listChecked.length == 0 && notTo != "True") {
            check = 'Error, Kindly choose location first!';
        }


        // Submit
        if (check != '') {
            alert(check);
            return false;
        }
        else {
            $("#divLoader").show();
            var updatedId = $('.selectedId').val();
            var form = $('#detailGRForm')[0];
            var formData = new FormData(form);

            $.each($("input[type='file']")[0].files, function (i, file) {
                formData.append('file', file);
            });

            debugger;
            $.ajax({
                type: "POST",
                url: '/Home/DetailGR?submitType=' + type,     //your action
                //data: type, formModal,   //your form name.it takes all the values of model               
                data: formData,
                dataType: 'json',
                processData: false,
                contentType: false,
                success: function (result) {
                    if (result.result == 'ok') {
                        Noti('Sucess!', 'Update lot information successfully', 'success');
                        // Update information
                        localStorage.setItem('updateId', updatedId);
                        localStorage.setItem('updateType', type);

                    }
                    else {
                        Noti('Error!', 'Update lot information failure, Error: ' + result.result, 'error');
                    }
                    $('.close').click();
                    $("#divLoader").fadeOut(300);
                }
            })
            return false;
        }
    });




    //$('#detailGRForm').submit(function () {
    //    // Check Value
    //    debugger;

    //    var check = "";
    //    var listChecked = $('.loadData').find(':checkbox:checked');
    //    var maxSample = 0; count = 0;

    //    // get type of submit
    //    var hoang = $(this).val();


    //    // No need create TO
    //    var notTo = $('#notTO').val();
    //    // Check total > max sample

    //    var vqSample = parseInt($('.vqSample').text()) || 0; // if null value = 0
    //    var fcSample = parseInt($('.fcSample').text()) || 0;
    //    maxSample = Math.max(vqSample, fcSample);

    //    // count total value
    //    listChecked.each(function () {
    //        count = count + parseInt($(this).closest("tr").find('td:nth-child(3)').text());
    //    });

    //    if (count < maxSample && notTo != "True") {
    //        check = "Warning, Not enought total stock for inspection!";
    //    }

    //    console.log(notTo);
    //    if (listChecked.length == 0 && notTo != "True") {
    //        check = 'Error, Kindly choose location first!';
    //    }
    //    // Submit
    //    if (check != '') {
    //        alert(check);
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //});

});