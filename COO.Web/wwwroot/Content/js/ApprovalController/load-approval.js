var home = {
    init: function () {
        home.regEvents();
    },
    //Show detail images
    regEvents: function () {
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

        //Show STT
        var t = $('#dataTable').DataTable({
            rowReorder: {
                selector: 'td:nth-child(2)'
            },
            responsive: true,
            "columnDefs": [{
                "searchable": false,
                "orderable": false,
                "targets": 0
            }],
            "order": [[1, 'asc']],
            "autoWidth": false
        });


        t.on('order.dt search.dt', function () {
            t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
            //t.column(10, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            //    //cell.innerHTML = i + 1;
            //    console.log(cell.val);
            ////});
        }).draw();


        //$('#dataTable tbody').on('click', '.btnDetail', function () {
        //    var id = $(this).data('id');
        //    console.log(id);
        //});

        // radio button
        $('#dataTable tbody').on('click', '.event-radio', function () {

            // active approval
            $('#approval').find('input:submit').attr('disabled', false);

            // Show check when mobile view new
            var inputNew = $('label[for="' + this.id + '"]').closest('div').find('input');
            inputNew.prop('checked', true);


            // Old 
            //var approveOld = $('tr[data-id="' + this.id + '"] td:nth-child(3)').find('input');
            var typeInitOld = $('tr[data-id="' + $(this).data('id') + '"]').find('input[id="' + this.id + '"]').data('type');
            var typeOld = $('tr[data-id="' + $(this).data('id') + '"] td:last-child').find('input:last-child');
            var commentOld = $('tr[data-id="' + $(this).data('id') + '"] td:last-child').find('textarea');



            // New
            var typeNew = inputNew.closest('li').find('span:first-child').text();
            var commentNew = inputNew.closest('li').next().find('textarea');

            if (typeNew == "Reject" || typeInitOld != null) {
                typeOld.val(-1);
                if (typeNew == "") {
                    commentOld.prop('required', true);
                    commentOld.focus()
                }
                else {
                    commentNew.prop('required', true);
                    commentNew.focus();
                }
            }
            else {
                typeOld.val(1);
                if (typeNew == "") {
                    commentOld.removeAttr('required');
                }
                else {
                    commentNew.removeAttr('required');
                }
            }
        });


        // Comment change
        // update double textbox - textbox when lend card
        $(document).on('keyup', 'textarea', function () {
            var id = this.id;
            var value = $(this).val();
            $('textarea#' + id).each(function () {
                $(this).val(value);
            });
        });


        // Submit all page in datatables js
        $('#approval').submit(function () {
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

        $(".btnReset").on('click', function (e) {
            $('.typeApproval').val('');
            $("#approval").trigger('reset');
            $('#approval').find('input:submit').attr('disabled', true);
            $('tr td textarea').prop('required', false);
        });
    }
}
home.init();