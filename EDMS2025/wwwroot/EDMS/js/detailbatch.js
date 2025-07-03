

var isRecto = false;
var idRecto = "";
var idVerso = "";
var init = true;
var flipX = 1
var flipY = 1;
var zoom = 0;
var cropped;
var croppedCanvas;
var isInitialized = false;
var isFlipped = false;
var rotate = 0;
var crop = false;
var BatchId;
var $image;
var isrescanall = true;
var numberofsheets = 0;
var numberofsheetspages = "";
var currentSheet = 1;
var docnextid;
var referenceNumber;
var fileTypeId;
var clickedId; // id de first doc showing
var isQualited = false; // id de first doc showing

//$("#elementOverlay").LoadingOverlay("show");

$(document).ready(function () {
    $("#firstdate").val(dateDebutISO);
    $("#lastdate").val(dateFinISO);

    var tabdocinbatch = $('#table_document_index_crop_sanity_control');
    var tabdocinbatchorder = $('#table_document_index_crop_sanity_control_order');

    if (tabdocinbatch.length > 0) {

        Load_document_index_crop_sanity_control();

        $('#table_document_index_crop_sanity_control').on('click', ".viewdocument", function (event) {
            event.preventDefault(); // Empêche le comportement par défaut du lien
            clickedId = $(this).data('batch');
            IdeletePage = false;
            showdocument(clickedId, 0);
        });


    }
    else if (tabdocinbatchorder.length > 0) {

        $("#saveordereddocuments").prop('disabled', true);
        Load_document_index_crop_sanity_control_order();

        $('#table_document_index_crop_sanity_control_order').on('click', ".viewdocument", function (event) {
            event.preventDefault(); // Empêche le comportement par défaut du lien
            clickedId = $(this).data('batch');
            IdeletePage = false;
            showdocument(clickedId, 1);
        });


    } else {
        $("#modalrejectalltab").hide();
        $("#modalacceptalltab").hide();




        // verif step and show or hide button who have all



        Load_document_review_modif_search(batchstep);

        $(document).on('click', '#modalrejectall', function () {

            isrescanall = true;
            var count = $('#table_document_review_modif_search').DataTable().rows().count();
            if (count != 0) {
                $("#RejectionModal").modal({ backdrop: 'static', keyboard: false }, 'show');
            }
        });

        $(document).on('click', '#modalacceptall', function () {

            var count = $('#table_document_review_modif_search').DataTable().rows().count();
            if (count != 0) {
                $("#SaveAllDocModal").modal({ backdrop: 'static', keyboard: false }, 'show');
            }
        });



        $("#filterIndexation").off("submit").on("submit", function (e) {
            e.preventDefault();
            Load_document_review_modif_search(batchstep);



        });


    }






});




function savealldocuments(batchstep) {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;

    $("#elementOverlay").LoadingOverlay("show");

    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/SaveAllDocumentsReview",
        type: "POST",
        dataType: "JSON",
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
            step: batchstep,
            batchnumber: $('#batchnumber').val(),
            documentnumber: $('#documentnumber').val()
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            alertCustom("success", 'fa fa-check', "Save all successfully");

            $("#SaveAllDocModal").modal('hide');
            Load_document_review_modif_search(batchstep);
            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}

function RescanAllDocumentsReview(batchstep) {

    if (enCours) return; // Empêche les clics multiples
    enCours = true;
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/RescanAllDocumentsReview",
        type: "POST",
        dataType: "JSON",
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
            step: batchstep,
            batchnumber: $('#batchnumber').val(),
            documentnumber: $('#documentnumber').val(),
            reason: $("#rejectiocode").val(),
            otherreason: $("#otherreason").val(),
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            alertCustom("success", 'fa fa-check', "Rescamn all successfully");
            $("#RejectionModal").modal('hide');
            Load_document_review_modif_search(batchstep);
            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}
function Load_document_review_modif_search(batchstep) {
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/getAllDocumentsReviewModificationSearch",
        type: "POST",
        dataType: "JSON",
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
            step: batchstep,
            batchnumber: $('#batchnumber').val(),
            documentnumber: $('#documentnumber').val()
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {


            var tableElement = $("#table_document_review_modif_search");

            if ($.fn.DataTable.isDataTable(tableElement)) {
                tableElement.DataTable().clear().destroy();
            }

            tableElement.find('tbody').append(res.vue);

            // Réinitialiser DataTable avec les nouvelles données
            tableElement.DataTable({
                destroy: true, // S'assurer que l'ancienne instance est détruite
                ordering: true,
                order: [[4, "asc"]],
                responsive: true,
                info: false,
                paging: true,
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
                    $('div.dataTables_wrapper div.dataTables_filter input').attr('placeholder', 'Search');
                    $('#searchalldocs').on('keyup', function () {
                        tableElement.DataTable().search(this.value).draw();
                    });


                    checksteptoshowallbutton();



                }
            });

            $(document).on('click', '.view-full-text', function () {
                var fullText = $(this).data('fulltext');
                var fullText1 = $(this).data('fulltext1');
                $('#textModal #fullText').text(fullText);
                $('#textModal #fullText1').text(fullText1);
            });


            $('#table_document_review_modif_search').on('click', ".viewdocument", function (event) {
                event.preventDefault(); // Empêche le comportement par défaut du lien
                clickedId = $(this).data('batch');
                IdeletePage = false;

                showdocument(clickedId, 0);
            });



            $(document).on('click', '#savealldocument', function () {
                savealldocuments(batchstep);

            });

            $(document).off("click", "[data-SendForRescan='atat']");
            $(document).on('click', "[data-SendForRescan='atat']", function () {

                if (isrescanall) {
                    RescanAllDocumentsReview(batchstep);
                } else {
                    saveDocumentIndex(3);
                }

            });



            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}



/**
 *  reordering or not : reorder : 1 ; not reorder : 0 ;
 * @param {any} docid
 * @returns
 */
function showdocument(docid, reorder) {

    if (docid != "" && docid != null) {
        if (enCours) return; // Empêche les clics multiples
        enCours = true;
        $("#elementOverlay").LoadingOverlay("show");
        $.ajax({
            beforeSend: function () {
            },
            url: "/EDMS/ViewDocumentIndexing",
            type: "POST",
            dataType: "JSON",
            data: {
                docId: docid,
                batchstep: batchstep,
                reorder: reorder
            },
            complete: function () {
                enCours = false; // Réinitialisation après la requête (réussie ou non)
            },
            error: function (xhr, status, error) {
                alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
                $("#elementOverlay").LoadingOverlay("hide", true);
            }, success: function (res) {


                idRecto = res.data.doccumentImage.idRecto;
                idVerso = res.data.doccumentImage.idVerso;

                isQualited = res.data.doccumentImage.isQualited;

                // number of sheets
                if (!IdeletePage) {
                    numberofsheets = parseInt(res.data.doccumentImage.docsCount);
                    currentSheet = 1;

                }

                $("#Showdocumentindex").html(res.vue);
                $("#filenumbertxt").text(res.data.doccumentImage.fileNumber);

                $(".my_select_box").chosen({ max_selected_options: 1, no_results_text: "Oops, nothing found!", width: "100%" });


                if (res.data.doccumentImage.pathVerso != "" && res.data.doccumentImage.pathRecto != "") {

                    $("#rectoverso").show();
                    isRecto = true;


                } else {
                    $("#rectoverso").hide();
                    if (res.data.doccumentImage.pathRecto != "") {

                        isRecto = true;

                    } else {
                        isRecto = false;
                    }
                }

                $('.acceptcrop').hide();

                SwitchRectoVerso();


                $("#Showdocumentindex").modal({ backdrop: 'static', keyboard: false }, 'show');

                $(".btn_save_reject_to_hide").prop('disabled', true);


                //if ($("#previousbtn").length > 0 && $("#nextbtn").length > 0) {


                //    $("#previousbtn").prop('disabled', true);

                //    if (parseInt(numberofsheets) > 1) {

                //        $("#nextbtn").prop('disabled', false);

                //    } else {

                //        $("#nextbtn").prop('disabled', true);
                //    }

                //}


                $('#rectoverso').on('click', function (event) {

                    rectoversoshortcut();

                });

                if (res.data.doccumentImage.rejectioncodeid != null) {

                    $('#rejectiocode').val(res.data.doccumentImage.rejectioncodeid);

                    setFocusTonextAfterEnter();


                } else {

                    // Ajouter l'événement pour sauter de champ en champ
                    setFocusTonextAfterEnter();
                }

                if (res.data.doccumentImage.otherreason != null) {

                    $('#otherreason').val(res.data.doccumentImage.otherreason);
                }

                docnextid = res.data.doccumentImage.idEncrypt;
                referenceNumber = res.data.doccumentImage.documentNumber;
                fileTypeId = res.data.doccumentImage.fileTypeId;

                // Initialiser les boutons au chargement
                updateButtons();

                // Gérer le clic sur "Next"
                $("[data-documentnext='" + res.data.doccumentImage.idEncrypt + "']").off('click').on('click', function () {
                    viewnextimage();
                });

                // Gérer le clic sur "Previous"
                $("[data-documentprev='" + res.data.doccumentImage.idEncrypt + "']").off('click').on('click', function () {
                    viewpreviousimage();
                });

                $("[data-save='" + res.data.doccumentImage.idEncrypt + "']").off('click').on('click', function (event) {
                    saveDocumentIndex(1);
                });

                let fields = document.querySelector('input.chosen-search-input');

                fields.focus();

                $(document).on('keydown', function (e) {
                    const key = e.key.toLowerCase();
                    var prev = $("[data-documentprev='" + res.data.doccumentImage.idEncrypt + "']");
                    var next = $("[data-documentnext='" + res.data.doccumentImage.idEncrypt + "']");
                    var save = $("[data-save='" + res.data.doccumentImage.idEncrypt + "']");
                    var rectoverso = $("#rectoverso");
                    var delet = $("#deletepages");




                    if (!$('#deletethispage').is(':visible') && key === "arrowright" && next.length > 0 && !next.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        viewnextimage();
                    } else if (!$('#deletethispage').is(':visible') && key === "arrowleft" && prev.length > 0 && !prev.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        viewpreviousimage();
                    }
                    else if (!$('#deletethispage').is(':visible') && key === "tab" && rectoverso.length > 0 && !rectoverso.is(':disabled') && rectoverso.is(':visible') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        rectoversoshortcut();
                    }
                    else if (!$('#deletethispage').is(':visible') && key === "enter" && save.length > 0 && !save.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        saveDocumentIndex(1);
                    } else if (!$('#deletethispage').is(':visible') && key === "d" && delet.length > 0 && !delet.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        $("#deletethispage").modal('show');

                    }
                    else if ($('#deletethispage').is(':visible') && key === "n" && delet.length > 0 && !delet.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        $("#deletethispage").modal('hide');

                    } else if ($('#deletethispage').is(':visible') && key === "enter" && delet.length > 0 && !delet.is(':disabled') && (fields.length === 0 || !document.activeElement.matches("input.chosen-search-input"))) {
                        deletepages();

                    }
                });

                $("[data-SendForPoliceReview='" + res.data.doccumentImage.idEncrypt + "']").off('click').on('click', function (event) {
                    $("#RejectionModal").modal({ backdrop: 'static', keyboard: false }, 'show');
                    //saveDocumentIndex(2);
                });

                $("[data-SendForPoliceReview='atat']").off('click').on('click', function (event) {
                    //$("#RejectionModal").modal({ backdrop: 'static', keyboard: false }, 'show');
                    saveDocumentIndex(2);
                });

                isrescanall = false;

                shownumberofsheetsandreferencenumber();

                cropperimage();
                savecroppedevent();

                select_option();

                $('select[name="Folio"]').on('change', function () {
                    select_option();
                });


                $("#elementOverlay").LoadingOverlay("hide", true);

            },
        });
    }

}

function select_option() {
    const selectedOption = $('select[name="Folio"]').find('option:selected');
    const folioData = selectedOption.data('folio');
    $("#filenumbertxt").text(folioData);
}

function rectoversoshortcut() {
    isRecto = isRecto ? false : true;
    SwitchRectoVerso();
}

function viewpreviousimage() {
    if (enCours) return; // Empêche les clics multiples
    if (currentSheet > 1) {
        currentSheet--;
        showOtherNextDocument("prev");
        updateButtons();
    }
}

function viewnextimage() {
    if (enCours) return; // Empêche les clics multiples

    if (currentSheet < numberofsheets) {
        currentSheet++;
        showOtherNextDocument("next");
        updateButtons();
    }
}

function setFocusTonextAfterEnter() {

    var btnsaveandreject = $('.btn_save_reject_to_hide');
    var my_select_box = $('.my_select_box');




    let field = document.querySelector('input.chosen-search-input');
    field.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault(); // Empêcher le comportement par défaut de "Entrée"

            if (btnsaveandreject.length > 0 && !btnsaveandreject.is(':disabled')) {
                saveDocumentIndex(1);
            }
        }
    });

    my_select_box.on('change', function (e) {
        field.blur();
    });
}

function shownumberofsheetsandreferencenumber() {
    numberofsheetspages = $("#docId" + clickedId).text();
    var filenumbername = "";
    var filenumber = $("#filenumbername").val();
    if (filenumber != null && filenumber != "") {
        filenumbername = "  --  File Number : " + filenumber;
    }

    $(".txtStart").text(referenceNumber + (isRecto ? " [ RECTO ]" : " [ VERSO ]") + "" + " -- Sheets(pages) : " + numberofsheetspages);
}
function saveDocumentIndex(type) {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;

    $("#elementOverlay").LoadingOverlay("show");
    var Project_folio_id;

    if ($('.acceptcrop').is(':visible')) {

        alert("You haven't finished cropping yet");

        $("#elementOverlay").LoadingOverlay("hide");

        enCours = false;

    } else {

        if (type == 1) {


            var my_select_box = $('.my_select_box');
            var input = $('.chosen-container');
            Project_folio_id = my_select_box.val()[0];


            var hasError = false;

            if (Project_folio_id == null) {

                input.css('border' , "2px solid red");

                if (!hasError) {
                    hasError = true;

                   
                }


            } else {
                input.css('border', "2px solid red");

            }


            if (hasError) {
                let fields = document.querySelector('input.chosen-search-input');
                $("#elementOverlay").LoadingOverlay("hide", true);
                setTimeout(function myfunction() {

                    input.trigger("mousedown");

                },200);
                enCours = false;


                return;
            }


        }



        $.ajax({
            beforeSend: function () {
            },
            url: "/EDMS/SaveIndexation",
            type: "POST",
            dataType: "JSON",
            data: {
                docId: docnextid,
                referenceNumber: referenceNumber,
                fileTypeId: fileTypeId,
                Project_folio_id: Project_folio_id,
                reason: $("#rejectiocode").val(),
                otherreason: $("#otherreason").val(),
                step: batchstep,
                type: type
            },
            complete: function () {
                enCours = false; // Réinitialisation après la requête (réussie ou non)
            },
            error: function (xhr, status, error) {
                
                alertCustom("danger", 'fa fa-close', "Une erreur s'est produite"); console.log(error);
                $("#elementOverlay").LoadingOverlay("hide", true);

            }, success: function (res) {

                $("#elementOverlay").LoadingOverlay("hide", true);
                var tabdocinbatch = $('#table_document_index_crop_sanity_control');
                if (res.indexdocrestant.isReturn == 1) {
                    let firstField = document.querySelector('[name="DocumentIndexes.index.IndexValue"]');
                    if (firstField) firstField.focus();
                    alert("The field " + res.indexdocrestant.champunique + " must be unique for the document " + res.indexdocrestant.documentnumberEdited + ". Here is the reference of its occurrence: " + res.indexdocrestant.documentnumberOccurrence);

                } else {

                    if (res.indexdocrestant.indexdocument == 1 && tabdocinbatch.length > 0) {
                        window.location.href = res.link;
                    } else {

                        $("#RejectionModal").modal('hide');



                        if (tabdocinbatch.length > 0) {
                            if (type == 1) {

                                alertCustom("success", 'fa fa-check', "Save successfully");

                            } else if (type == 2) {

                                alertCustom("success", 'fa fa-check', "Reject successfully");

                            } else {

                                alertCustom("success", 'fa fa-check', "Rescan successfully");

                            }
                            Load_document_index_crop_sanity_control();
                        } else {
                            if (type == 1) {

                                alertCustom("success", 'fa fa-check', "Save successfully");

                            } else if (type == 2) {

                                alertCustom("success", 'fa fa-check', "Reject successfully");

                            } else {

                                alertCustom("success", 'fa fa-check', "Rescan successfully");

                            }
                            $("#Showdocumentindex").modal("hide");
                            Load_document_review_modif_search(batchstep);
                        }


                    }

                }



            },
        });
    }




}


function cropperimage() {



    $(function () {
        'use strict';

        var console = window.console || { log: function () { } };


        var URL = window.URL || window.webkitURL;


        if (isRecto) {
            $image = $('#cropper-imgrecto');
        } else {
            $image = $('#cropper-imgverso');
        }




        var options = {
            viewMode: 2,
            autoCropArea: 1,
            preview: '.img-preview',
            crop: function (e) {
            }
        };
        var originalImageURL = $image.attr('src');
        var uploadedImageType = 'image/' + imageextension;
        var uploadedImageURL;

        // Tooltip
        $('[data-toggle="tooltip"]').tooltip();

        if (!$.isFunction(document.createElement('canvas').getContext)) {
            $('button[data-method="getCroppedCanvas"]').prop('disabled', true);
        }

        if (typeof document.createElement('cropper').style.transition === 'undefined') {
            $('button[data-method="rotate"]').prop('disabled', true);
            $('button[data-method="scale"]').prop('disabled', true);
        }

        // Options
        $('.docs-toggles').on('change', 'input', function () {


            var $this = $(this);
            var name = $this.attr('name');
            var type = $this.prop('type');
            var cropBoxData;
            var canvasData;

            if (!$image.data('cropper')) {
                return;
            }

            if (type === 'checkbox') {
                options[name] = $this.prop('checked');
                cropBoxData = $image.cropper('getCropBoxData');
                canvasData = $image.cropper('getCanvasData');

                options.ready = function () {
                    $image.cropper('setCropBoxData', cropBoxData);
                    $image.cropper('setCanvasData', canvasData);
                };
            } else if (type === 'radio') {
                options[name] = $this.val();
            }

            $image.cropper('destroy').cropper(options);
        });

        // Methods
        $('.docs-buttons').on('click', '[data-method]', function () {

            updateburronsaftercropacceptorcancel(true);


            if (isRecto) {
                $image = $('#cropper-imgrecto');
            } else {
                $image = $('#cropper-imgverso');
            }

            croppedCanvas = $image.on({

                ready: function (e) {
                    var cropData = $(this).cropper('getCropBoxData');

                    if (cropData.width <= 1 || cropData.height <= 1) {
                        $('.acceptcrop').hide();
                    } else {
                        $('.acceptcrop').show();
                    }
                },
                cropstart: function (e) {
                    var cropData = $(this).cropper('getCropBoxData');

                    if (cropData.width <= 1 || cropData.height <= 1) {
                        $('.acceptcrop').hide();
                    } else {
                        $('.acceptcrop').show();
                    }
                },
                cropend: function (e) {
                    var cropData = $(this).cropper('getCropBoxData');

                    if (cropData.width <= 1 || cropData.height <= 1) {
                        $('.acceptcrop').hide();
                    } else {
                        $('.acceptcrop').show();
                    }
                }


            }).cropper(options);


            var $this = $(this);
            var data = $this.data();
            var cropper = $image.data('cropper');

            var $target;
            var result;

            if ($this.prop('disabled') || $this.hasClass('disabled')) {
                return;
            }

            if (cropper && data.method) {
                data = $.extend({}, data); // Clone a new one

                if (typeof data.target !== 'undefined') {
                    $target = $(data.target);

                    if (typeof data.option === 'undefined') {
                        try {
                            data.option = JSON.parse($target.val());
                        } catch (e) {
                            console.log(e.message);
                        }
                    }
                }

                cropped = cropper.cropped;

                switch (data.method) {
                    case 'rotate':
                        if (cropped && options.viewMode > 0) {
                            $image.cropper('clear');
                        }

                        break;

                    case 'getCroppedCanvas':
                        if (uploadedImageType === 'image/' + imageextension) {
                            if (!data.option) {
                                data.option = {};
                            }

                            data.option.fillColor = '#fff';
                        }

                        break;
                }

                result = $image.cropper(data.method, data.option, data.secondOption);

                switch (data.method) {
                    case 'rotate':
                        if (cropped && options.viewMode > 0) {
                            $image.cropper('crop');
                        }

                        break;
                    case 'left90':
                        $image.cropper('rotate', -90);
                        var imageData = $image.cropper('getImageData'); // Récupérer la taille réelle de l'image
                        $image.cropper('setCropBoxData', {
                            left: 0,
                            top: 0,
                            width: imageData.naturalWidth,
                            height: imageData.naturalHeight
                        });
                        break;

                    case 'right90':
                        $image.cropper('rotate', 90);
                        var imageData = $image.cropper('getImageData'); // Récupérer la taille réelle de l'image
                        $image.cropper('setCropBoxData', {
                            left: 0,
                            top: 0,
                            width: imageData.naturalWidth,
                            height: imageData.naturalHeight
                        });
                        break;

                    case 'scaleX':
                        var option = parseFloat(data.option) * -1; // Convertir et inverser
                        $(this).data('option', option);
                        $image.cropper('scaleX', option); // Appliquer l'inversion
                        $('.acceptcrop').show();
                        break;
                    case 'scaleY':
                        var option = parseFloat(data.option) * -1; // Convertir et inverser
                        $(this).data('option', option);
                        $image.cropper('scaleY', option); // Appliquer l'inversion
                        $('.acceptcrop').show();
                        break;

                    case 'getCroppedCanvas':
                        if (result) {

                            $('#getCroppedCanvasModal').modal().find('.modal-body').html(result);

                        }

                        break;

                    case 'destroy':
                        if (uploadedImageURL) {
                            URL.revokeObjectURL(uploadedImageURL);
                            uploadedImageURL = '';
                            $image.attr('src', originalImageURL);

                        }
                        $('.acceptcrop').hide();

                        updateburronsaftercropacceptorcancel(false);


                        break;
                }

                if ($.isPlainObject(result) && $target) {
                    try {
                        $target.val(JSON.stringify(result));
                    } catch (e) {
                        console.log(e.message);
                    }
                }
            }
        });

        // Keyboard
        $(document.body).on('keydown', function (e) {
            if (e.target !== this || !$image.data('cropper') || this.scrollTop > 300) {
                return;
            }

            switch (e.which) {
                case 37:
                    e.preventDefault();
                    $image.cropper('move', -1, 0);
                    break;

                case 38:
                    e.preventDefault();
                    $image.cropper('move', 0, -1);
                    break;

                case 39:
                    e.preventDefault();
                    $image.cropper('move', 1, 0);
                    break;

                case 40:
                    e.preventDefault();
                    $image.cropper('move', 0, 1);
                    break;
            }
        });


    });


}
function updateburronsaftercropacceptorcancel(boolean) {
    $(".class_global_for_all_btn").prop('disabled', boolean);
    $(".btn_save_reject_to_hide").prop('disabled', true);
    if (!boolean) updateButtons();
}

function fitImage() {
    var cropper = $image.data('cropper');
    if (cropper) {
        var containerData = $image.cropper('getContainerData');
        var imageData = $image.cropper('getImageData');

        var scaleX = containerData.width / imageData.naturalWidth;
        var scaleY = containerData.height / imageData.naturalHeight;
        var scale = Math.max(scaleX, scaleY); // Assure un ajustement complet

        $image.cropper('setCanvasData', {
            left: (containerData.width - imageData.width * scale) / 2,
            top: (containerData.height - imageData.height * scale) / 2,
            width: imageData.naturalWidth * scale,
            height: imageData.naturalHeight * scale
        });
    }
}
function savecroppedevent() {


    $('#acceptcropped').off("click").on('click', function (event) {

        if (enCours) return; // Empêche les clics multiples
        enCours = true;

        $("#elementOverlay").LoadingOverlay("show");

        const canvas = $image.cropper('getCroppedCanvas');

        // Convertir le canvas en un fichier Blob (sans réduction de taille)
        canvas.toBlob(function (blob) {
            let formData = new FormData();
            formData.append("docId", docnextid);
            formData.append("referenceNumber", referenceNumber);
            formData.append("fileTypeId", fileTypeId);
            formData.append("image", blob, "cropped.jpg"); // Fichier envoyé au serveur
            formData.append("isRecto", isRecto);
            formData.append("BatchId", BatchId);

            $.ajax({
                url: "/EDMS/cropimage",
                type: "POST",
                data: formData,
                processData: false,  // Important pour FormData
                contentType: false,  // Important pour envoyer un fichier
                complete: function () {
                    enCours = false; // Réinitialisation après la requête (réussie ou non)
                },
                success: function (res) {
                    if ($image != null) {
                        $image.cropper('destroy');
                        $('.acceptcrop').hide();
                    }


                    updateburronsaftercropacceptorcancel(false);

                    const path = `${res.data}?t=${new Date().getTime()}`;

                    if (isRecto) {
                        $("#cropper-imgrecto").attr("src", path);
                    } else {
                        $("#cropper-imgverso").attr("src", path);
                    }


                    $("#elementOverlay").LoadingOverlay("hide", true);
                },
                error: function (xhr, status, error) {
                    alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
                    $("#elementOverlay").LoadingOverlay("hide", true);
                }
            });

        }, 'image/' + imageextension);

    });
}

function showOtherNextDocument(nextorprev) {
    if (enCours) return; // Empêche les clics multiples
    enCours = true;
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/ShowOtherNextDocument",
        type: "POST",
        dataType: "JSON",
        data: {
            docId: docnextid,
            referenceNumber: referenceNumber,
            fileTypeId: fileTypeId,
            nextorprev: nextorprev,
            step: batchstep
        },
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', "Une erreur s'est produite");console.log(error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {



            idRecto = res.data.doccumentImage.idRecto;
            idVerso = res.data.doccumentImage.idVerso;

            if (res.data.doccumentImage.pathVerso != "" && res.data.doccumentImage.pathRecto != "") {

                $("#rectoverso").show();
                isRecto = true;


            } else {
                $("#rectoverso").hide();
                if (res.data.doccumentImage.pathRecto != "") {

                    isRecto = true;

                } else {
                    isRecto = false;
                }
            }

            SwitchRectoVerso();

            if ($image != null) {
                $image.cropper('destroy');
            }

            if (res.data.isPath == 0) {

                const rectoPath = `${res.data.doccumentImage.pathRecto}?t=${new Date().getTime()}`;
                const versoPath = `${res.data.doccumentImage.pathVerso}?t=${new Date().getTime()}`;

                $("#cropper-imgrecto").attr("src", rectoPath);
                $("#cropper-imgverso").attr("src", versoPath);
            }
            else {

                const rectoBase64 = `${res.data.doccumentImage.imageContentStr}?t=${new Date().getTime()}`;
                const versoBase64 = `${res.data.doccumentImage.imageVersoContentStr}?t=${new Date().getTime()}`;

                $("#cropper-imgrecto").attr("src", rectoBase64);
                $("#cropper-imgverso").attr("src", versoBase64);
            }




            //if ($("#previousbtn").length > 0 && $("#nextbtn").length > 0) {


            //    if (nextorprev == "next") {

            //        if (docnextid == res.data.doccumentImage.idEncrypt) {

            //            $("#previousbtn").prop('disabled', false);
            //            $("#nextbtn").prop('disabled', true);

            //        } else {
            //            $("#previousbtn").prop('disabled', false);
            //            $("#nextbtn").prop('disabled', false);
            //        }

            //    } else {

            //        if (docnextid == res.data.doccumentImage.idEncrypt) {

            //            $("#previousbtn").prop('disabled', true);
            //            $("#nextbtn").prop('disabled', false);

            //        } else {

            //            $("#previousbtn").prop('disabled', false);
            //            $("#nextbtn").prop('disabled', false);
            //        }
            //    }

            //}



            docnextid = res.data.doccumentImage.idEncrypt;
            referenceNumber = res.data.doccumentImage.documentNumber;
            fileTypeId = res.data.doccumentImage.fileTypeId;


            shownumberofsheetsandreferencenumber();

            updateButtons();

            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}

function updateButtons() {

    $("#previousbtn").prop('disabled', currentSheet === 1);
    $("#nextbtn").prop('disabled', currentSheet >= numberofsheets);
    let nextbtn = $("#nextbtn");
    let firstField = document.querySelector('[name="Folio"]');

    if (currentSheet >= numberofsheets || isQualited) {
        $(".btn_save_reject_to_hide").prop('disabled', false);
    }
    /*if(nextbtn.is(':disabled') && nextbtn.is(':visible') && firstField ){
        firstField.focus();
    }*/

}

function SwitchRectoVerso() {

    if ($('.acceptcrop').is(':visible')) {

        alert("You haven't finished cropping yet");


    } else {

        if (isRecto == false) {
            if ($image != null) {
                $image.cropper('destroy');
            }
            $("#cropper-imgrecto").hide();
            $("#cropper-imgverso").show();

            $(".txtStart").text($(".txtStart").text().replace("[ RECTO ]", "[ VERSO ]"));

        } else {
            if ($image != null) {
                $image.cropper('destroy');
            }
            $("#cropper-imgrecto").show();
            $("#cropper-imgverso").hide();
            $(".txtStart").text($(".txtStart").text().replace("[ VERSO ]", "[ RECTO ]"));
        }
        $('.acceptcrop').hide();

    }
}
