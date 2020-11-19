var home = {
    init: function () {
        home.regEvents();
    },
    //Show detail images
    regEvents: function () {

        var date = new Date().toLocaleDateString();

        //Show STT
        var t = $('#dataTable').DataTable({
            responsive: true,
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0,
            }
            ],
            buttons: [
                {
                    extend: 'excel',
                    filename: "DownloadList-" + date
                }
            ],
            "autoWidth": false,
            "initComplete": () => { $("#dataTable").show(); $("#divLoader").fadeOut(300); }
        });
        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();

        // Select Type
        // Event listener to the two range filtering inputs to redraw on input
        var table = $('#dataTable').DataTable()
        // Export excel
        $('#btnExport').off('click').on('click', function (e) {
            if (confirm('Do you want export this table?')) {
                table.button('.buttons-excel').trigger();
            }
        });
        $('#dataTable tbody').on('click', 'tr', function () {
            $(this).toggleClass('table-primary');
        });

        // Edit
        //$('#dataTable tbody').on('click', '.btn-edit', function (e) {
        //    $("#divLoader").show();
        //    e.preventDefault();
        //    e.stopPropagation();
        //    var id = $(this).data('id');
        //    $.ajax({
        //        data: { id: id },
        //        url: '/Switch/Edit',
        //        type: 'GET',
        //        success: function (res) {
        //            $('#myModalEdit').html(res);
        //            $('#myModalEdit .clickModel').click(); // Trick for show
        //            $("#divLoader").fadeOut(300);
        //        }
        //    })
        //});
    }
}
home.init();