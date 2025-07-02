$(document).ready(function () {

    $("#firstdate").val(dateDebutISO);
    $("#lastdate").val(dateFinISO);
    Load_index_crop_sanity_control(batchstep);

    $("#filterIndexation").off("submit").on("submit", function (e) {
        e.preventDefault();
        Load_index_crop_sanity_control(batchstep);

    });

   


});




