$(document).ready(function () {
    $("#alertError").hide();

    // Set Character Remaining
    function updateCountdown() {
        // 140 is the max message length
        var remaining = 150 - $('#inputPurposeVisit').val().length;
        if (remaining <= 0) {
            $('#inputRemark123').focus();
        }
        $('.btnCountDown').text(remaining + ' characters remaining.');
    }
    updateCountdown();
    $('#inputPurposeVisit').change(updateCountdown);
    $('#inputPurposeVisit').keyup(updateCountdown);

    function updateCountdown1() {
        // 140 is the max message length
        var remaining = 150 - $('#inputRemark123').val().length;
        if (remaining <= 0) {
            $('#inputPurposeVisit').focus();
        }
        $('.btnCountDown1').text(remaining + ' characters remaining.');
    }
    updateCountdown1();
    $('#inputRemark123').change(updateCountdown1);
    $('#inputRemark123').keyup(updateCountdown1);

    // Fill fushan name
    $('.btnFushan').on('click', function () {
        $(this).closest("div.input-group").find('input').val('Fushan Technology');
    });

    $('#download_CSV').click(function () {
        $.ajax({
            url: '/InputListUser.xlsx',
            method: 'GET',
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);
                a.href = url;
                a.download = 'InputListUser.xlsx';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $('#myInfor').click(function () {
        var employeeId = $(this).data('id');
        $('#tab_logic tr:nth-child(1) td:nth-child(2) input').val(employeeId)
        var name = $(this).data('name');
        $('#tab_logic tr:nth-child(1) td:nth-child(1) input').val(name)
        var slm = $(this).data('slm');
        $('#tab_logic tr:nth-child(1) td:nth-child(3) input').val(slm)
    });

    $('#createRequest').submit(function () {
        // Code validate form
        $(window).scrollTop(0);
        // get value
        var incommingDate = $('.form_datetime').val();

        var now = $.datepicker.formatDate('mm/dd/yy', new Date())
        // Biến cờ hiệu
        var flag = true;
        // Validate Time 
        if (incommingDate < now) {
            $("#alertError").text('Using date must greater than current date');
            flag = false;
        }
        // Validate Max Lengths
        if ($('#inputPurposeVisit').val().length > 150) {
            $("#alertError").text('Length of purpose of visit must less than 150');
            flag = false;
        }
        if ($('#inputRemark123').val().length > 150) {
            $("#alertError").text('Length of remark must less than 150');
            flag = false;
        }
        // Check Name, National Id, CompanyName, Remark
        $('tr td:nth-child(1) > input').each(function (index, tr) {
            if ($(this).val().length > 50) {
                $("#alertError").text('Error: '+$(this).val()+',Length of name must less than 50');
                flag = false;
            }
        });
        $('tr td:nth-child(2) > input').each(function (index, tr) {
            if ($(this).val().length > 20) {
                $("#alertError").text('Error: ' + $(this).val() + ',Length of employee id must less than 20');
                flag = false;
            }
        });
        $('tr td:nth-child(3) > input').each(function (index, tr) {
            if ($(this).val().length > 150) {
                $("#alertError").text('Error: ' + $(this).val() + ',Length of SLM name must less than 100');
                flag = false;
            }
        });
        $('tr td:nth-child(4) > input').each(function (index, tr) {
            if ($(this).val().length > 150) {
                $("#alertError").text('Error: ' + $(this).val() + ',Length of visitor remark must less than 150');
                flag = false;
            }
        });

        // Check EmployeeId Duplicate
        var checkArray = [];
        $('table tr td:nth-child(2) input').each(function () {
            checkArray.push($(this).val());
        })
        function hasDuplicates(arr) {
            return arr.some(x => arr.indexOf(x) !== arr.lastIndexOf(x));
        }
        if (hasDuplicates(checkArray)) {
            $("#alertError").text('A Passenger can not use two car at the same time, Please re-check in Passenger Information');
            flag = false;
        }

        if (flag == false) {
            $("#alertError").fadeTo(2000, 500).slideUp(500, function () {
            });
            return false;
        }
        else {
            $("#divLoader").show();
            return true;
        }
    });
});

