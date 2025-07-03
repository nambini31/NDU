var firstdate ;
var lastdat ;
var documentnumber ;
var batchnumber ;
var docid;
var batchid;
var type;
var IdeletePage = false; // si on supprime une page ,
var enCours = false;


var aujourdHui = new Date();

// Soustrayez 30 jours à la date d'aujourd'hui
aujourdHui.setDate(aujourdHui.getDate() - 0);

var dateDebut = aujourdHui;
var dateFin = new Date();
var dateFinTi = new Date();

// Convertissez les dates en format ISO (YYYY-MM-DD)
var dateDebutISO = dateDebut.toISOString().split('T')[0];
var dateFinISO = dateFin.toISOString().split('T')[0];



function alertCustom(type_message, ft_icon, message) {
    var id = "alertdialog";
    var alert =
        `
                    <div style="z-index: 999999999" class="alert bg-` +
        type_message +
        ` alert-icon-left alert-arrow-left alert-dismissible mb-2" role="alert">
                        <span class="alert-icon"><i class="` +
        ft_icon +
        `"></i></span>
                        <button type="button" class="close" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <strong>` +
        message +
        `</strong>
                    </div>
                `;

    var $alertElement = $(alert);

    $alertElement.find(".close").on("click", function () {
        $alertElement.hide();
    });

    $("#alert_place").append($alertElement);

    setTimeout(function () {
        $alertElement.hide();
    }, 6000);
}

function checksteptoshowallbutton() {
    var totalYesAndNo = $('.yes, .no').length;
    var totalYes = $('.yes').length;
    var totalnonExport = $('.nonexport').length;
    var totalNo = $('.no').length;

    switch (batchstep) {
        case 8:
            if (totalYes == 0) $("#modalacceptall").prop('disabled', true);
            else $("#modalacceptall").prop('disabled', false);
            if (totalNo == 0) $("#modalrejectall").prop('disabled', true);
            else $("#modalrejectall").prop('disabled', false);
            break;
        case 14:
            //if (totalnonExport == 0) $("#modalexport").prop('disabled', true);
            //else 
            $("#modalexport").prop('disabled', false);
            break;
        case 11:
            if (totalYes == 0) $("#modalunlockall").prop('disabled', true);
            else $("#modalunlockall").prop('disabled', false);
            break;
        case 12:
            if (totalNo == 0) $("#modalbatchcompletedall").prop('disabled', true);
            else $("#modalbatchcompletedall").prop('disabled', false);
            break;
        case 13:
            if (totalNo == 0) $("#modaldocumentcompletedall").prop('disabled', true);
            else $("#modaldocumentcompletedall").prop('disabled', false);
            break;
        default:
            break;
    }
}

$(document).on('click', '.modalexport', function () {

    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("export all batch non-exported");

     firstdate = $('#firstdate').val();
     lastdat = $('#lastdate').val();
     documentnumber = $('#documentnumber').val();
    docid = null;
    batchnumber = $('#batchnumber').val();
    batchid = null;
    type = 7;

    // export all batch not completed
});

$(document).on('click', '.documentallmarkcomplete', function () {

    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("mark all document as completed");

     firstdate = $('#firstdate').val();
     lastdat = $('#lastdate').val();
     documentnumber = $('#documentnumber').val();
    docid = null;
    batchnumber = $('#batchnumber').val();
    batchid = null;
    type = 2;

    // complete all document not completed
});

$(document).on('click', '.documentonemarkcomplete', function () {
    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("mark this document as completed");

     firstdate = null;
     lastdat = null;
     documentnumber = null;
     docid = $(this).data('doc');
     batchnumber = null;
    type = 1;
     batchid = null;

    // complete one document 

});

$(document).on('click', '.batchallmarkcomplete', function () {
    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("mark all batch as completed");

    firstdate = $('#firstdate').val();
    lastdat = $('#lastdate').val();
    documentnumber =null;
    docid = null;
    batchnumber = $('#batchnumber').val();
    batchid = null;
    type = 3;

    //complete all batch
});

$(document).on('click', '.batchallunlock', function () {
    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("unlock all batch ");

    firstdate = $('#firstdate').val();
    lastdat = $('#lastdate').val();
    documentnumber =null;
    docid = null;
    batchnumber = $('#batchnumber').val();
    batchid = null;
    type = 5;

    //unlock all batch
});

$(document).on('click', '.batchoneunlock', function () {
    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("unlock this batch");

    firstdate = null;
    lastdat = null;
    documentnumber = null;
    docid = null;
    batchnumber = null;
    batchid = $(this).data('batch');
    type = 6;

    //unlock one batch
});

$(document).on('click', '.batchonemarkcomplete', function () {
    $("#MakeCompletedModal").modal({ backdrop: 'static', keyboard: false }, 'show');
    $(".txtstart").text("mark this batch  as completed");

     firstdate = null;
     lastdat = null;
     documentnumber = null;
     docid = null;
     batchnumber = null;
     batchid = $(this).data('batch');
    type = 4;

    // complete one batch

});

function MakeCompletedBatch() {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;
    $("#elementOverlay").LoadingOverlay("show");

    $.ajax({
        url: "/EDMS/MakeCompletedBatch",
        type: "POST",
        dataType: "JSON",
        data: {
            batchId: batchid,
            docId: docid,
            type: type,
            firstdate: firstdate,
            lastdate: lastdat,
            batchnumber: batchnumber,
            documentnumber: documentnumber

        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        },
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        success: function (res) {
            if (res.status == "success") {
                $("#MakeCompletedModal").modal('hide');
                
                alertCustom("success", 'fa fa-check', "Success");
                var tabdocinbatch = $('#table_index_crop_sanity_control');
                if (tabdocinbatch.length > 0) {
                    Load_index_crop_sanity_control(batchstep)
                } else {
                    $("#Showdocumentindex").modal('hide');
                    Load_document_review_modif_search(batchstep);
                }
                //Load_index_crop_sanity_control(step);
            }
        },
    });
}

$(document).off("click", "#markdocumentdocument");
$(document).on('click', "#markdocumentdocument", function () {

    MakeCompletedBatch();

});

$(document).off("click", "#deletepages");
$(document).on('click', "#deletepages", function () {

    deletepages();

});

function deletepages() {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;
    $("#elementOverlay").LoadingOverlay("show");

    $.ajax({
        url: "/EDMS/DeletePages",
        type: "POST",
        dataType: "JSON",
        data: {
            idRecto: idRecto,
            idVerso: idVerso,
            isRecto: isRecto,
            step: batchstep,
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        },
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        success: function (res) {

            $('.acceptcrop').hide();

            if (res.status == "success") {
                $("#deletethispage").modal('hide');


                alertCustom("success", 'fa fa-check', "Success");
                enCours = false;

                var tabdocinbatch = $('#table_document_index_crop_sanity_control');

                if (tabdocinbatch.length > 0) {
                    Load_document_index_crop_sanity_control();
                } else {

                    Load_document_review_modif_search(batchstep);
                }

                IdeletePage = true;

                if (res.retour == 1) {
                    if (numberofsheets == currentSheet) {
                        numberofsheets--;
                        currentSheet--;
                        showOtherNextDocument("prev");
                    } else {
                        numberofsheets--;
                        showOtherNextDocument("next");
                    }
                    if (numberofsheets <= 0) {

                        window.location.href = res.link;
                    }

                } else {
                    showdocument(docnextid, 0);
                }
            }
            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}