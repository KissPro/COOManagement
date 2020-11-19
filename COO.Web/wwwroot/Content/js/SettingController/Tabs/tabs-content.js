$(document).ready(function () {
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
                //$("#dataTable").dataTable();
                //$('#singleRow').hide();

                //// For post - restrict by validate
                //$('#inputCompany').removeAttr('required');
                //$('#inputNameBox').removeAttr('required');
                //$('#inputNationalId').removeAttr('required');

                // check file
                var check = true;
                $('#dataTable > tbody  > tr').each(function () {
                    $(this).find("td:eq(0)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 50) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(1)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 50) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(2)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 50) {
                            check = false;
                        }
                    });
                    $(this).find("td:eq(3)").each(function () {
                        if ($(this).text() == '' || $(this).text().length > 150) {
                            check = false;
                        }
                    });
                });
                if (check == false) {
                    alert('Have some field is being empty, greater than maxlength or not correct format, please recheck Excel file!');
                    $('#listVisitor').val('');
                    $('#filename').val(null);
                    $('#employee_table').html('');
                    //$('#singleRow').show();

                    //$("#inputNameBox").prop('required', true);
                    //$("#inputNationalId").prop('required', true);
                    //$("#inputCompany").prop('required', true);
                    //$('#checkNationalId').val('true');
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
    });

    $('#save_CSV').click(function () {
        var list = $('#listVisitor').val();
        if (list != null && list != '') {
            $("#divLoader").show();
            $.ajax({
                data: { list: list },
                url: '/Visitor/AddBlacklists',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status != true) {
                        //$('#checkNationalId').val("false");
                        alert(res.status + " can't update, please re-check with master data to upload!");
                    }
                    else {
                        alert("Upload list blacklist successfully!")
                    }
                    $('#clear_CSV').click();
                    window.location.href = "/black-list";
                    $("#divLoader").fadeOut(300);
                }
            });
        }
    });

    // download template file
    $('#download_CSV').click(function () {
        $.ajax({
            url: '/InputBlacklist.xlsx',
            method: 'GET',
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);
                a.href = url;
                a.download = 'InputBlacklist.xlsx';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });
});