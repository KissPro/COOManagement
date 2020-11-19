$(document).ready(function () {

    var t = $('#dataTable').DataTable({

        "ajax": {
            "url": "/boom-ecus/show-all",
            "type": "Post",
            "datatype": "json"
        },
        "searchDelay": 2000,
        "serverSide": true,
        "processing": true,
        //"deferLoading": 57,
        "columns": [
            {
                "data": "id",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "EmpID", "name": "EmpID" },
            { "data": "Name", "name": "Name" },
            { "data": "DepartmentName", "name": "DepartmentName" },
            { "data": "LeftValue", "name": "LeftValue" },
            { "data": "RightValue", "name": "RightValue" },
            { "data": "GateIP", "name": "GateIP" }
        ],

        responsive: true,
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": 0,
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 4,
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 5,
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 10,
        }
        ],
        "autoWidth": false,
        "language": {
            "processing": "Loading. Please wait..."
        }
    });
});