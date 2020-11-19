$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    // #region INIT
    var t = $('#dataTable').DataTable();
    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    // if swipe time process 1 null -> disable process 2
    $("#dataTable tr").each(function () {
        // hide checkIn
        if ($(this).find('td:nth-last-child(2)').text() == 1
            && $(this).find('input:nth-child(4)').val() == ''
            && $(this).find('input.setTime').val() == '00:00') {
            // find process 2
            $(this).next().find('td:last-child').css("visibility", "hidden");
        }
        // hide checkOut
        if ($(this).find('td:nth-last-child(2)').text() == 2
            && $(this).find('input:nth-child(4)').val() != ''
            && $(this).find('input.setTime').val() == '00:00') {
            $(this).find('td:last-child').css("visibility", "hidden");
        }
    })
    // disabled if haven't swipe yet 
    //$("#dataTable tr > td:nth-child(10) > input[value='']").closest('tr').find("input,button,textarea,select,span").attr("disabled", "disabled");


    // Prevent submit without update information
    $('form')
        .each(function () {
            $(this).data('serialized', $(this).serialize())
        })
        .on('change input', function () {
            $(this)
                .find('input:submit, button:submit')
                .attr('disabled', $(this).serialize() == $(this).data('serialized'))
                ;
        })
        .find('input:submit, button:submit')
        .attr('disabled', true)
        ;


    // Submit all page in datatables js
    $('#requestDetail').submit(function () {
        $("#divLoader").show();
        t.search('').draw();

        var form = this;
        // Encode a set of form elements from all pages as an array of names and values
        var params = t.$('input,select,textarea').serializeArray();
        // Iterate over all form elements
        $.each(params, function () {
            // If element doesn't exist in DOM
            if (!$.contains(document, form[this.name])) {
                // Create a hidden element
                $(form).append(
                    $('<input>')
                        .attr('type', 'hidden')
                        .attr('name', this.name)
                        .val(this.value)
                );
            }
        });
        return true;
    });


    // update double textbox - textbox when lend card
    $(document).on('keyup', '.changeCard', function () {
        var value = $(this).val();
        var thisTime = $(this).closest('tr').find('input.setTime').val();
        var employeeId = $(this).closest('tr').find('td:nth-child(7)').text();
        $('.changeCard').each(function () {
            if ($(this).closest('tr').find('td:nth-child(7)').text() == employeeId && $(this).closest('tr').find('input.setTime').val() == thisTime) {
                $(this).val(value);
            }
        });
    });

    // #endregion

    // Approval
    $('.btnApproval').on("click", function () {
        var comment = $('#comment').val();
        var requestId = $('#requestId').data('id');
        var type = $(this).data('type');
        var process = $(this).data('process');

        if (type == -1 && comment == "") {
            alert("Please leave a reason for the rejection");
            return;
        }
        $("#divLoader").show();
        $.ajax({
            data: { approval: null, Id: requestId, type: type, process: process, comment: comment },
            url: '/Approval/Approval',
            dataType: 'json',
            type: 'POST',
            success: function (res) {
                window.location.href = "/approval/" + process;
                $("#divLoader").fadeOut(300);
            }
        });
    });


    // #region SWIPE CARD CHECK - FOR THE SECURIRY
    function addZero(i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    }


    $(document).on('click', '.btnSwipe', function () {
        var thisRow = $(this).closest('tr');
        thisRow.addClass('table-primary');
        thisRow.find('td:nth-child(9) > input').val(addZero(new Date().getHours()) + ':' + addZero(new Date().getMinutes()));
        thisRow.find('td:nth-child(10) > input').attr('required', true);
        thisRow.find('td:nth-child(10) > input').removeAttr("disabled", "disabled");
    });


    $(document).on('click', '.btnLendCard', function (e) {
        var thisRow = $(this).closest('tr');
        var nextRow = $(this).closest('tr').next();
        // check employeeId exists in system
        var EmployeeId = $(this).closest('tr').find('td:nth-child(7)').text();
        $('#submit').prop('disabled', true);
        $.ajax({
            data: { employeeId: EmployeeId },
            url: '/Visitor/CheckEscorter',
            dataType: 'json',
            type: 'POST',
            success: function (res) {
                if (res.result != "true") {
                    alert('ERROR, This EmployeeId: ' + res.result + ' not exists in AD system, please recheck!');
                    // recover changes
                    // this row
                    thisRow.removeClass('table-warning');
                    thisRow.find('td:nth-child(10) > input').attr('required', false);
                    thisRow.find("input,button,textarea,select,span").attr("disabled", "disabled");
                    thisRow.find('td:nth-child(10) > input').val('');
                    // newrow
                    //if (nextRow.find('td:nth-last-child(2)').text() != 2) {

                    //}
                    nextRow.removeClass('table-warning');
                    nextRow.find("input,button,textarea,select,span").attr("disabled", "disabled");
                    nextRow.find('td:nth-child(10) > input').attr('required', false);
                    nextRow.find('td:nth-child(10) > input').val('');

                    // Change return status
                    thisRow.find('input.returnCardDate').val(null);
                    nextRow.find('input.returnCardDate').val(null);
                }
                $('#submit').prop('disabled', false);
            }
        })

        // change this row
        thisRow.addClass('table-warning');
        thisRow.find('td:nth-child(9) > input').val('00:00');
        thisRow.find('td:nth-child(10) > input').attr('required', true);
        thisRow.find('td:nth-child(10) > input').removeAttr("disabled", "disabled");

        // Change return status
        thisRow.find('input.returnCardDate').val(new Date('9999').toLocaleDateString());
        nextRow.find('input.returnCardDate').val(new Date('9999').toLocaleDateString());

        // change next row
        var process = $(this).closest('tr').find('td:nth-last-child(2)').text();
        if (process != " ") {
            nextRow.addClass('table-warning');
            nextRow.find('td:nth-child(9) > input').val('00:00');
            nextRow.find('td:nth-child(10) > input').attr('required', true);
            nextRow.find('td:nth-child(10) > input').removeAttr("disabled", "disabled");
        }
    });

    $(document).on('click', '.btnClear_Swipe', function () {
        var thisRow = $(this).closest('tr');
        var nextRow = $(this).closest('tr').next();
        // remove swiper
        thisRow.removeClass('table-primary');
        thisRow.find("input,button,textarea,select,span").attr("disabled", "disabled");
        thisRow.find('td:nth-child(9) > input').val('00:00');
        thisRow.find('td:nth-child(10) > input').attr('required', false);
        thisRow.find('td:nth-child(10) > input').val('');

        // remove lend card
        thisRow.removeClass('table-warning');
        nextRow.removeClass('table-warning');
        nextRow.find("input,button,textarea,select,span").attr("disabled", "disabled");
        nextRow.find('td:nth-child(10) > input').attr('required', false);
        nextRow.find('td:nth-child(10) > input').val('');

        // Change return status
        thisRow.find('input.returnCardDate').val(null);
        nextRow.find('input.returnCardDate').val(null);
    });

    // check out return lend card
    $(document).on('click', '.btnReturnCard', function () {
        var thisRow = $(this).closest('tr');
        var nextRow = $(this).closest('tr').next();
        if ($(this).hasClass('text-success')) {
            thisRow.find('input.returnCardDate').val(new Date().toLocaleString());
            thisRow.find('td:nth-child(10) > input').css('background', '#c4f5c4');
            nextRow.find('input.returnCardDate').val(new Date().toLocaleString());
            nextRow.find('td:nth-child(10) > input').css('background', '#c4f5c4');
            // change
            $(this).removeClass('text-success');
            $(this).addClass('text-danger');
            $(this).find('i').removeClass('fa-chevron-circle-down');
            $(this).find('i').addClass('fa-redo-alt');
            $('#submit').prop('disabled', false);
        }
        else {
            thisRow.find('input.returnCardDate').val(null);
            thisRow.find('td:nth-child(10) > input').css('background', '');
            nextRow.find('input.returnCardDate').val(null);
            nextRow.find('td:nth-child(10) > input').css('background', '');
            $(this).addClass('text-success');
            $(this).removeClass('text-danger');
            $(this).find('i').addClass('fa-chevron-circle-down');
            $(this).find('i').removeClass('fa-redo-alt');
            $('#submit').prop('disabled', true);
        }
    });

    //$('.btnReturnCard')

    // #endregion

    // #region SET TIME - WHEN CONFIRM BILL
    $(document).on('click', '.btnSetTime', function () {
        $(this).closest('td').find('input').val(addZero(new Date().getHours()) + ':' + addZero(new Date().getMinutes()));
        $(this).closest('td').find('input').removeClass('bg-danger');
    });
    $(document).on('change', '.setTime', function () {
        var getValue = $(this).val() + ':00';
        var datetime = new Date('1970-01-01T' + getValue + 'Z');
        if (datetime.toString() == 'Invalid Date') {
            $(this).addClass('bg-danger');
            $(this).val('00:00');
        }
        else {
            $(this).removeClass('bg-danger');
        }
    });
    // #endregion




});