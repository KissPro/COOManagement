var report = {
    init: function () {
        report.regEvents();
    },
    //Show detail images
    regEvents: function () {

        $('#btnExport').off('click').on('click', function (e) {
            if (confirm('Do you want export this table?')) {
                table.button('.buttons-excel').trigger();
            }
        });

        // Search confirmdate range
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = new Date($('.form_datetime').val());
                var max = new Date($('.form_datetime1').val());
                var date = new Date(data[9]) || 0; // use data for the age column
                var date1 = new Date(data[9]) || 0;
                max.setHours(23, 59, 59, 999);
                if ((isNaN(min) && isNaN(max)) ||
                    (isNaN(min) && date1 <= max) ||
                    (min <= date && isNaN(max)) ||
                    (min <= date && date1 <= max)) {
                    return true;
                }
                return false;
            }
        );

        $('tbody').scroll(function (e) {
            $('thead').css("left", -$("tbody").scrollLeft()); //fix the thead relative to the body scrolling
            $('thead th:nth-child(1)').css("left", $("tbody").scrollLeft()); //fix the first cell of the header
            $('tbody td:nth-child(1)').css("left", $("tbody").scrollLeft()); //fix the first column of tdbody
        });

        // Show STT
        var date = new Date().toLocaleDateString();
        var t = $('#dataTable').DataTable({
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0
            }],
            buttons: [
                {
                    extend: 'excel',
                    filename: "Taxi Management System-" + date,
                }
            ],
        });

        $('#dataTable tbody').on('click', 'tr', function () {
            $(this).toggleClass('table-primary');
        });

        //t.on('order.dt search.dt', function () {
        //    t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
        //        cell.innerHTML = i + 1;
        //    });
        //}).draw();

        // Select Type

        // Event listener to the two range filtering inputs to redraw on input
        var table = $('#dataTable').DataTable()
        $('#btnFilter').off('click').on('click', function () {
            var dep = $('#selectDepartment').val();
            table.column(3).every(function () {
                var that = this;
                if (that.search() !== dep) {
                    that
                        .search(dep)
                }
            });
            table.draw();
        });

        $('#btnClear').off('click').on('click', function () {
            $(".form_datetime").datepicker().val('');
            $(".form_datetime1").datepicker().val('');
            $('#selectDepartment').val('');
            $('#btnFilter').click();
            $('.clearInput').val('');
        });

        $('#dataTable tfoot th').each(function (i) {
            var title = $(this).text();
            $(this).html('<input type="text" style="padding:0; width:100%"/>');

            $('input', this).on('keyup change', function () {
                if (table.column(i).search() !== this.value) {
                    table
                        .column(i)
                        .search(this.value)
                        .draw();
                }
            });
        });
    }
}
report.init();