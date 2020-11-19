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




    $('#updateRequest').submit(function (e) {
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
                $("#alertError").text('Error: ' + $(this).val() + ',Length of name must less than 50');
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

    /// LOADING
    // Hide first remove 
    $('tr:first-child td div.row-remove').css("display", "none");


    // Show row
    $("#add_row").on("click", function () {
        // Dynamic Rows Code
        // Get max row id and set new id
        var newid = 0;
        $.each($("#dataTable tr"), function () {
            if (parseInt($(this).data("id")) > newid) {
                newid = parseInt($(this).data("id"));
            }
        });
        newid++;

        var tr = $("<tr></tr>", {
            id: "addr" + newid, "data-id": newid
        });
        // loop through each td and create new elements with name of newid
        $.each($("#dataTable tbody tr:nth(0) td"), function () {
            var td;
            var cur_td = $(this);

            var children = cur_td.children();

            // add new td and element if it has a nane
            if ($(this).data("name") !== undefined) {
                td = $("<td></td>", {
                    "data-name": $(cur_td).data("name")
                });
                var c = $(cur_td).find($(children[0]).prop('tagName')).clone().val("");
                c.attr("name", "Visitor[" + newid + "]." + $(cur_td).data("name"));
                c.css('display', 'inline-block');
                c.appendTo($(td));
                td.appendTo($(tr));
            } else {
                td = $("<td></td>", {
                    'text': $('#dataTable tr').length
                }).appendTo($(tr));
            }
        });
        this.focus();
        // add the new row
        $(tr).appendTo($('#dataTable'));

        /// REMOVE - ADD
        $(tr).find("td div.row-remove").on("click", function () {
            $("#divLoader").show();

            // Check if contain add class
            // add new
            var index = ($(this).attr('name')).match(/\d+/);
            var requestId = $('#idRequests').data('id');
            var name = $('#addr' + index + ' td:nth-child(2) > input').val();
            var id = $('#addr' + index + ' td:nth-child(3) > input').val();
            var company = $('#addr' + index + ' td:nth-child(4) > input').val();
            var remark = $('#addr' + index + ' td:nth-child(5) > textarea').val();
            var remark_UI = $('#addr' + index + ' td:nth-child(6) > textarea').val();
            if (name != "" && id != "" && company != "") {
                if ($(this).hasClass('addNewVisitor')) {
                    $.ajax({
                        data: { requestId: requestId, name: name, id: id, company: company, remark: remark, remark_UI: remark_UI },
                        url: '/Visitor/AddNewVisitor',
                        dataType: 'json',
                        type: 'POST',
                        success: function (res) {
                            if (res.result == true) {
                                location.reload();
                                alert('Add new user successfully, Please refresh to update list visitor!');
                                var btnChange = $('div[name="Visitor[' + index + '].del"]');
                                $(btnChange).css("background-color", "#dc3545");
                                $(btnChange).css("border-color", "#dc3545");
                                $(btnChange).find('span').text('×');
                                $(btnChange).removeClass("addNewVisitor");
                            }
                            else {
                                alert('Add new user failure!');
                                return;
                            }
                            $("#divLoader").fadeOut(300);
                        }
                    })
                }
                else {
                    var nationalId = $(this).closest("tr").find('td:nth-child(3) > input').val();
                    var requestId = $('#idRequests').data('id');
                    $.ajax({
                        data: { id: requestId, nationalId: nationalId },
                        url: '/Visitor/RemoveVisitor',
                        dataType: 'json',
                        type: 'POST',
                        success: function (res) {
                            if (res.result == true) {
                                alert('Remove visitor successfully!');
                                location.reload();
                            }
                            $("#divLoader").fadeOut(300);
                        }
                    });
                    $(this).closest("tr").remove();
                }
            }
            else {
                $(this).closest("tr").remove();
            }
        });
    });
    // Sortable Code
    var fixHelperModified = function (e, tr) {
        var $originals = tr.children();
        var $helper = tr.clone();

        $helper.children().each(function (index) {
            $(this).width($originals.eq(index).width())
        });
        return $helper;
    };

    /// Remove visitor infor exists in list view
    $(document).on('click', 'td div[name="del0"]', function () {
        $("#divLoader").show();
        var requestId = $('#idRequests').data('id');
        var nationalId = $(this).closest("tr").find('td:nth-child(3) > input').val();
        $.ajax({
            data: { id: requestId, nationalId: nationalId },
            url: '/Visitor/RemoveVisitor',
            dataType: 'json',
            type: 'POST',
            success: function (res) {
                if (res.result == true) {
                    alert('Remove visitor successfully!');
                    location.reload();
                }
                $("#divLoader").fadeOut(300);
            }
        })
        $(this).closest("tr").remove();
    });


    // Change button to add <-> remove
    $(document).on('change', 'tr td:nth-child(2) > input,td:nth-child(3) > input:first-child, td:nth-child(4) > input', function () {

        var name = $(this).closest("tr").find('td:nth-child(2) > input').val();
        var nationalId = $(this).closest("tr").find('td:nth-child(3) > input').val();
        var company = $(this).closest("tr").find('td:nth-child(4) > input').val();
        var delButton = $(this).closest("tr").find('td:nth-child(7) > div');
        var inputDel = $(delButton).find('span');
        var checkHasValue = $(this).closest("tr").find('td:nth-child(3)').data('id');

        if (checkHasValue == "" || checkHasValue == undefined) {
            if (name != "" && nationalId != "" && company != "") {
                $(delButton).css("background-color", "#329bbc");
                $(delButton).css("border-color", "#329bbc");
                inputDel.text('+');
                $(delButton).addClass("addNewVisitor");
            }
            else {
                $(delButton).css("background-color", "#dc3545");
                $(delButton).css("border-color", "#dc3545");
                inputDel.text('×');
                $(delButton).removeClass("addNewVisitor");
            }
        }
    });
});

