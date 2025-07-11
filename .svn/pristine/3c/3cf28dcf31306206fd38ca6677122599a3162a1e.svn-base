$(document).ready(function () {

    $("#firstdate").val(dateDebutISO);
    $("#lastdate").val(dateFinISO);

    //Load_dashboard();

    $("#filterIndexation").off("submit").on("submit", function (e) {
        e.preventDefault();
        Load_dashboard();

    });

   


});
var enCours = false;
function Load_dashboard() {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;

    $("#elementOverlay").LoadingOverlay("show");

    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/Load_dashboard",
        type: "POST",
        dataType: "JSON",
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
        },
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            $('.idfordashboard').html(res.vue);

            var tableElement = $("#usersdashboard");

            // Réinitialiser DataTable avec les nouvelles données
            tableElement.DataTable({
                destroy: true, // S'assurer que l'ancienne instance est détruite
                ordering: true,
                order: [[0, "desc"]],
                responsive: true,
                info: false,
                paging: false,
                searching: false,
                deferRender: true,
                pageLength: 10,
                language: {
                    search: "",
                    zeroRecords: "No data",
                    paginate: {
                        previous: "Précédent",
                        next: "Suivant",
                    },
                },
                dom: "rtip",

                initComplete: function (settings, json) {
                  

                }
            });

            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}



