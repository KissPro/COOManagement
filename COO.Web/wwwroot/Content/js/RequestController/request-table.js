$(document).ready(function () {
    $("#add_row").on("click", function () {
        // Dynamic Rows Code
        // Get max row id and set new id
        var newid = 0;
        $.each($("#tab_logic tr"), function () {
            if (parseInt($(this).data("id")) > newid) {
                newid = parseInt($(this).data("id"));
            }
        });
        newid++;

        var tr = $("<tr></tr>", {
            id: "addr" + newid, "data-id": newid
        });
        // loop through each td and create new elements with name of newid
        $.each($("#tab_logic tbody tr:nth(0) td"), function () {
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
                c.appendTo($(td));
                td.appendTo($(tr));
            } else {
                td = $("<td></td>", {
                    'text': $('#tab_logic tr').length
                }).appendTo($(tr));
            }
        });

        this.focus();

        // add the new row
        $(tr).appendTo($('#tab_logic'));

        $(tr).find("td button.row-remove").on("click", function () {
            $(this).closest("tr").remove();
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



    // Check national id in black list
    function checkNationalId(nationalID) {
        $.ajax({
            data: { nationalId: nationalID },
            url: '/Visitor/CheckNationalId',
            dataType: 'json',
            type: 'POST',
            success: function (res) {
                if (res.status != "true") {
                    //$('#checkNationalId').val("false");
                    alert('This national id: ' + res.status + " in blacklist, please remove this to upload!");
                    $('#clear_CSV').click();
                }
            }
        });
    }

    $("#filename").change(function (e) {
        var ext = $("input#filename").val().split(".").pop().toLowerCase();
        if ($.inArray(ext, ["xlsx"]) == -1) {
            alert('Upload Excel.xlsx');
            return false;
        }
        if (e.target.files != undefined) {
            var fileReader = new FileReader();
            fileReader.onload = function (e) {
                // call 'xlsx' to read the file
                var readDate = e.target.result;
                var workbook = XLSX.read(readDate, { type: 'binary' });
                var sheet_name_list = workbook.SheetNames;
                var cnt = 0; /*This is used for restricting the script to consider only first sheet of excel*/
                var dataResult;
                sheet_name_list.forEach(function (y) { /*Iterate through all sheets*/
                    /*Convert the cell value to Json*/
                    dataResult = XLSX.utils.sheet_to_csv(workbook.Sheets[y]);
                });

                var input = $('#listVisitor');
                input.val(dataResult);
                var employee_data = dataResult.split(/\r?\n|\r/);
                console.log(dataResult);
                var table_data = '<table class="table table-bordered table-hover" id="dataTable" width="100%" cellspacing="0">';
                table_data += '<thead>';
                for (var count = 0; count < 1; count++) {
                    var cell_data = employee_data[count].split(",");
                    table_data += '<tr>';
                    for (var cell_count = 0; cell_count < cell_data.length; cell_count++) {
                        if (count === 0) {
                            table_data += '<th>' + cell_data[cell_count] + '</th>';
                        }
                    }
                    table_data += '</tr>';
                }
                table_data += '</thead>';
                table_data += '<tbody>';
                for (var count = 1; count < employee_data.length - 1; count++) {
                    var cell_data = employee_data[count].split(",");
                    table_data += '<tr>';
                    for (var cell_count = 0; cell_count < cell_data.length; cell_count++) {
                        if (count !== 0) {
                            table_data += '<td>' + cell_data[cell_count] + '</td>';
                        }
                    }
                    table_data += '</tr>';
                }
                table_data += '</tbody>';
                table_data += '</table>';
                $('#employee_table').html(table_data);
                $("#dataTable").dataTable();
                $('#singleRow').hide();

                // For post - restrict by validate
                $('#inputCompany').removeAttr('required');
                $('#inputNameBox').removeAttr('required');
                $('#inputNationalId').removeAttr('required');

                // check file
                var check = true;
                var nationalId = "";
                $('#dataTable > tbody  > tr').each(function () {
                    $(this).find("td:eq(0)").each(function () {
                        if ($(this).text() == '' || /\d/.test($(this).text()) || $(this).text().length > 50) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(1)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 50) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(2)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 150) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(3)").each(function () {
                        if ($(this).text().length > 150) {
                            check = false;
                        }
                    });
                    checkNationalId($(this).text());

                    //Check national id in black list
                    $(this).find("td:eq(1)").each(function () {
                        checkNationalId($(this).text());
                        //if ($('#checkNationalId').val() == "false") {
                        //    console.log($('#checkNationalId').val());
                        //    nationalId = $(this).text();
                        //    check = false;
                        //}
                    });
                });
                if (check == false) {
                    //if (nationalId != null && nationalId != "") {
                    //    alert('This national id: ' + nationalId + " in blacklist, please remove this to upload!");
                    //}
                    //else {
                    alert('Have some field is being empty, greater than maxlength or not correct format, please recheck Excel file!');
                    //}
                    $('#listVisitor').val('');
                    $('#filename').val(null);
                    $('#employee_table').html('');
                    $('#singleRow').show();

                    $("#inputNameBox").prop('required', true);
                    $("#inputNationalId").prop('required', true);
                    $("#inputCompany").prop('required', true);
                    $('#checkNationalId').val('true');
                    return false;
                }
            };
            fileReader.readAsArrayBuffer(e.target.files.item(0));
        }
        return false;
    });

    $('#import_CSV').click(function () {
        $("#filename").trigger("click", function (e) {
        });
    });

    $('#clear_CSV').click(function () {
        $('#listVisitor').val('');
        $('#filename').val(null);
        $('#employee_table').html('');
        $('#singleRow').show();

        $("#inputNameBox").prop('required', true);
        $("#inputNationalId").prop('required', true);
        $("#inputCompany").prop('required', true);
        $('#checkNationalId').val('true');
    });

});

