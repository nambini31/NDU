

var enCours = false;


function Load_index_crop_sanity_control(Step) {
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/Load_index_crop_sanity_control",
        type: "POST",
        dataType: "JSON",
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
            step: Step,
            batchnumber: $('#batchnumber').val(),
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {


            var tableElement = $("#table_index_crop_sanity_control");

            if ($.fn.DataTable.isDataTable(tableElement)) {
                tableElement.DataTable().clear().destroy();
            }

            tableElement.find('tbody').append(res.vue);

            // Réinitialiser DataTable avec les nouvelles données
            tableElement.DataTable({
                destroy: true, // S'assurer que l'ancienne instance est détruite
                ordering: true,
                order: [[5, "asc"]],
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
                    $('#searchbatch').on('keyup', function () {
                        tableElement.DataTable().search(this.value).draw();
                    });
                    checksteptoshowallbutton();

                }
            });

            createActionToShowDocument(Step, tableElement);

            $(document).off("click", ".view-full-text");
            $(document).on('click', '.view-full-text', function () {
                var fullText = $(this).data('fulltext');
                var fullText1 = $(this).data('fulltext1');
                $('#textModal #fullText').text(fullText);
                $('#textModal #fullText1').text(fullText1);
            });



            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}



function ExcelForBatch(Step) {
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/excel_for_batch",
        type: "POST",
        dataType: "JSON",
        data: {
            firstdate: $('#firstdate').val(),
            lastdate: $('#lastdate').val(),
            step: Step,
            batchnumber: $('#batchnumber').val(),
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            $("#elementOverlay").LoadingOverlay("hide", true);

        },
    });
}



function Load_document_index_crop_sanity_control() {
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/Load_document_index_crop_sanity_control",
        type: "POST",
        dataType: "JSON",
        data: {
            batchId: batchidclick,
            step: batchstep,
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            var tableElement = $("#table_document_index_crop_sanity_control");

            if ($.fn.DataTable.isDataTable(tableElement)) {
                tableElement.DataTable().clear().destroy();
            }
            var firstdoc = res.firstdoc;
            tableElement.find('tbody').append(res.vue);

            // Réinitialiser DataTable avec les nouvelles données
            tableElement.DataTable({
                destroy: true, // S'assurer que l'ancienne instance est détruite
                ordering: false,
                //order: [[0, "asc"]],
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
                dom: "Bfrtip",
                buttons: [
                    {
                        extend: "excelHtml5",
                        title: "Excel",
                        className: "btn btn-sm btn-primary",
                        text: "Excel",
                        exportOptions: {
                            columns: ":not(:last-child)"
                        }
                    }
                ],
                initComplete: function (settings, json) {
                    $('div.dataTables_wrapper div.dataTables_filter input').attr('placeholder', 'Search');
                    $("#elementOverlay").LoadingOverlay("hide", true);


                    $(document).on('click', '.view-full-text', function () {
                        var fullText = $(this).data('fulltext');
                        var fullText1 = $(this).data('fulltext1');
                        $('#textModal #fullText').text(fullText);
                        $('#textModal #fullText1').text(fullText1);
                    });



                    if (!IdeletePage) {
                        var btnshowdocument = $('.viewdocument');
                        clickedId = firstdoc;
                        if (firstdoc != null && btnshowdocument.length > 0) {
                            showdocument(firstdoc, 0);
                        }
                        checksteptoshowallbutton();

                    } else {
                        shownumberofsheetsandreferencenumber();
                    }
                }
            });


        },
    });

}

function Load_document_index_crop_sanity_control_order() {
    $("#elementOverlay").LoadingOverlay("show");
    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/Load_document_index_crop_sanity_control_order",
        type: "POST",
        dataType: "JSON",
        data: {
            batchId: batchidclick,
            step: batchstep,
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            var tableElement = $("#table_document_index_crop_sanity_control_order");

            if ($.fn.DataTable.isDataTable(tableElement)) {
                tableElement.DataTable().clear().destroy();
            }
            var firstdoc = res.firstdoc;
            tableElement.find('tbody').append(res.vue);

            // Réinitialiser DataTable avec les nouvelles données
            tableElement.DataTable({
                destroy: true, // S'assurer que l'ancienne instance est détruite
                ordering: false,
                //order: [[0, "asc"]],
                responsive: true,
                info: false,
                deferRender: true,
                paging: false,
                searching: false,
                language: {
                    search: "",
                    zeroRecords: "No data",
                    paginate: {
                        previous: "Précédent",
                        next: "Suivant",
                    },
                },
                dom: "Bfrtip",
                buttons: [
                    {
                        extend: "excelHtml5",
                        title: "Excel",
                        className: "btn btn-sm btn-primary",
                        text: "Excel",
                        exportOptions: {
                            columns: ":not(:last-child)"
                        }
                    }
                ],
                initComplete: function (settings, json) {
                    $('div.dataTables_wrapper div.dataTables_filter input').attr('placeholder', 'Search');
                    $("#elementOverlay").LoadingOverlay("hide", true);

                    $(document).off("click", "#refresh");
                    $(document).on('click', '#refresh', function () {
                        Load_document_index_crop_sanity_control_order();


                    });

                    $(document).on('click', '.view-full-text', function () {
                        var fullText = $(this).data('fulltext');
                        var fullText1 = $(this).data('fulltext1');
                        $('#textModal #fullText').text(fullText);
                        $('#textModal #fullText1').text(fullText1);
                    });

                    $(document).off("click", "#saveordereddocuments");
                    $(document).on('click', '#saveordereddocuments', function () {


                        Saveordereddocuments();


                    });
                    showorhidesaveordered();
                    enableDragAndDrop();
                }
            });


        },
    });






}


function Saveordereddocuments() {

    $("#elementOverlay").LoadingOverlay("show");

    var table = $("#table_document_index_crop_sanity_control_order");

    let orderData = [];

    table.find("tr").each(function (index) {
        let numDoc = $(this).attr("data-numdoc");
        if (numDoc) {
            orderData.push({ order: index, idDoc: numDoc });
        }
    });

    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/Saveordereddocuments",
        type: "POST",
        dataType: "JSON",
        data: {
            orderData: orderData,
            batchId: batchidclick
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);
        }, success: function (res) {

            alertCustom("success", 'fa fa-check', "Save  successfully");


            Load_document_index_crop_sanity_control_order();

        },
    });


}

function enableDragAndDrop() {

    var table = document.getElementById("table_document_index_crop_sanity_control_order");
    // 🔥 Ajout du Drag & Drop après l'initialisation de DataTables

    let draggingEle;
    let draggingRowIndex;
    let placeholder;
    let list;
    let isDraggingStarted = false;

    // The current position of mouse relative to the dragging element
    let x = 0;
    let y = 0;

    // Swap two nodes
    const swap = function (nodeA, nodeB) {
        const parentA = nodeA.parentNode;
        const siblingA = nodeA.nextSibling === nodeB ? nodeA : nodeA.nextSibling;

        // Move nodeA to before the nodeB
        nodeB.parentNode.insertBefore(nodeA, nodeB);

        // Move nodeB to before the sibling of nodeA
        parentA.insertBefore(nodeB, siblingA);
    };

    // Check if nodeA is above nodeB
    const isAbove = function (nodeA, nodeB) {
        // Get the bounding rectangle of nodes
        const rectA = nodeA.getBoundingClientRect();
        const rectB = nodeB.getBoundingClientRect();

        return rectA.top + rectA.height / 2 < rectB.top + rectB.height / 2;
    };

    const cloneTable = function () {
        const rect = table.getBoundingClientRect();
        const width = parseInt(window.getComputedStyle(table).width);

        list = document.createElement('div');
        list.classList.add('clone-list');
        list.style.position = 'absolute';
        /*list.style.left = rect.left + 'px';
        list.style.top = rect.top + 'px';*/
        table.parentNode.insertBefore(list, table);

        // Hide the original table
        table.style.visibility = 'hidden';

        table.querySelectorAll('tr').forEach(function (row) {
            // Create a new table from given row
            const item = document.createElement('div');
            item.classList.add('draggable');

            const newTable = document.createElement('table');
            newTable.setAttribute('class', 'clone-table');
            newTable.style.width = width + 'px';

            const newRow = document.createElement('tr');
            const cells = [].slice.call(row.children);
            cells.forEach(function (cell) {
                const newCell = cell.cloneNode(true);
                newCell.style.width = parseInt(window.getComputedStyle(cell).width) + 'px';
                newRow.appendChild(newCell);
            });

            newTable.appendChild(newRow);
            item.appendChild(newTable);
            list.appendChild(item);
        });
    };

    const mouseDownHandler = function (e) {



        document.querySelectorAll('tr.parent').forEach(row => {
            row.classList.remove('parent'); // Supprime la classe parent
            let detailRow = row.nextElementSibling;
            if (detailRow && detailRow.classList.contains('child')) {
                detailRow.remove(); // Supprime complètement la ligne enfant
            }
        });

        // Get the original row
        const originalRow = e.target.parentNode;
        draggingRowIndex = [].slice.call(table.querySelectorAll('tr')).indexOf(originalRow);

        // Determine the mouse position
        x = e.clientX;
        y = e.clientY;

        // Attach the listeners to document
        document.addEventListener('mousemove', mouseMoveHandler);
        document.addEventListener('mouseup', mouseUpHandler);
    };

    const mouseMoveHandler = function (e) {
        if (!isDraggingStarted) {
            isDraggingStarted = true;

            cloneTable();

            draggingEle = [].slice.call(list.children)[draggingRowIndex];
            draggingEle.classList.add('dragging');

            // Let the placeholder take the height of dragging element
            // So the next element won't move up
            placeholder = document.createElement('div');
            placeholder.classList.add('placeholder');
            draggingEle.parentNode.insertBefore(placeholder, draggingEle.nextSibling);
            placeholder.style.height = draggingEle.offsetHeight + 'px';
        }

        // Set position for dragging element
        draggingEle.style.position = 'absolute';
        draggingEle.style.top = (draggingEle.offsetTop + e.clientY - y) + 'px';
        draggingEle.style.left = (draggingEle.offsetLeft + e.clientX - x) + 'px';

        // Reassign the position of mouse
        x = e.clientX;
        y = e.clientY;

        // The current order
        // prevEle
        // draggingEle
        // placeholder
        // nextEle
        const prevEle = draggingEle.previousElementSibling;
        const nextEle = placeholder.nextElementSibling;

        // The dragging element is above the previous element
        // User moves the dragging element to the top
        // We don't allow to drop above the header
        // (which doesn't have previousElementSibling)
        if (prevEle && prevEle.previousElementSibling && isAbove(draggingEle, prevEle)) {
            // The current order    -> The new order
            // prevEle              -> placeholder
            // draggingEle          -> draggingEle
            // placeholder          -> prevEle
            swap(placeholder, draggingEle);
            swap(placeholder, prevEle);
            return;
        }

        // The dragging element is below the next element
        // User moves the dragging element to the bottom
        if (nextEle && isAbove(nextEle, draggingEle)) {
            // The current order    -> The new order
            // draggingEle          -> nextEle
            // placeholder          -> placeholder
            // nextEle              -> draggingEle
            swap(nextEle, placeholder);
            swap(nextEle, draggingEle);
        }
    };

    const mouseUpHandler = function () {

        if (placeholder != null && placeholder.parentNode != null) {


            placeholder && placeholder.parentNode.removeChild(placeholder);

            draggingEle.classList.remove('dragging');
            draggingEle.style.removeProperty('top');
            draggingEle.style.removeProperty('left');
            draggingEle.style.removeProperty('position');

            // Get the end index
            const endRowIndex = [].slice.call(list.children).indexOf(draggingEle);

            // Remove the placeholder


            isDraggingStarted = false;

            // Remove the list element
            list.parentNode.removeChild(list);

            // Move the dragged row to endRowIndex
            let rows = [].slice.call(table.querySelectorAll('tr'));
            draggingRowIndex > endRowIndex
                ? rows[endRowIndex].parentNode.insertBefore(rows[draggingRowIndex], rows[endRowIndex])
                : rows[endRowIndex].parentNode.insertBefore(
                    rows[draggingRowIndex],
                    rows[endRowIndex].nextSibling
                );

            // Bring back the table
            table.style.removeProperty('visibility');

            // Remove the handlers of mousemove and mouseup
            document.removeEventListener('mousemove', mouseMoveHandler);
            document.removeEventListener('mouseup', mouseUpHandler);

            //if (draggingRowIndex !== endRowIndex) {


            table.querySelectorAll('tr').forEach(function (row, index) {
                if (index === 0) return; // Ignore la première ligne (supposée être l'en-tête)

                var indexoftr = row.getAttribute('index');

                if (index == indexoftr) {

                    row.classList.remove('moved-row');

                } else {

                    row.classList.add('moved-row');
                }
            });




            // Supprimer la classe après un certain temps
            /* setTimeout(() => {
                 movedRow.classList.remove('moved-row');
             }, 2000);*/
            //}

            showorhidesaveordered();

        }
    };

    table.querySelectorAll('tr').forEach(function (row, index) {
        // Ignore the header
        // We don't want user to change the order of header
        if (index === 0) {
            return;
        }

        const secondCell = row.children[1]; // Sélectionne la deuxième colonne
        if (secondCell) { // Vérifie si la cellule existe
            secondCell.classList.add('draggable');
            secondCell.addEventListener('mousedown', mouseDownHandler);
        }



    });

}


function showorhidesaveordered() {
    var totalmove = $('.moved-row').length;

    if (totalmove == 0) $("#saveordereddocuments").prop('disabled', true);
    else $("#saveordereddocuments").prop('disabled', false);
}

// create action for batch index or sanity or quality .....
function createActionToShowDocument(step, tableElement) {

    tableElement.off("click", ".rejectinbatch");
    tableElement.off("click", ".showdocumentinbatch");
    tableElement.off("click", ".orderdocumentinbatch");

    tableElement.on("click", ".rejectinbatch", function (ev) {

        let id = $(this).data("batch");


        $("#RejectionModal").modal({ backdrop: 'static', keyboard: false }, 'show');

        $("[data-SendForRejectetd='atat']").off('click').on('click', function (event) {

            RejectBatch(id, step);
        });

    });


    tableElement.on("click", ".showdocumentinbatch", function (ev) {
        if (enCours) return; // Empêche les clics multiples
        enCours = true;

        $("#elementOverlay").LoadingOverlay("show");
        let id = $(this).data("batch");

        $.ajax({
            beforeSend: function () {
                $("#elementOverlay").LoadingOverlay("show");
            },
            url: "/EDMS/IsBatchLocked",
            type: "POST",
            dataType: "JSON",
            complete: function () {
                enCours = false; // Réinitialisation après la requête (réussie ou non)
            },
            data: {
                step: step,
                batchId: id,
            },
            error: function (xhr, status, error) {
                alertCustom("danger", 'fa fa-close', error);
                $("#elementOverlay").LoadingOverlay("hide", true);
            }, success: function (res) {

                if (res.isLocked) {
                    alert("batch locked by " + res.userlock);
                    $("#elementOverlay").LoadingOverlay("hide", true);
                } else {
                    submitFormWithPost(res.urlredirect, {
                        step: step,
                        batchId: id,
                    });
                }
            },
        });

    });
    tableElement.on("click", ".orderdocumentinbatch", function (ev) {
        if (enCours) return; // Empêche les clics multiples
        enCours = true;

        $("#elementOverlay").LoadingOverlay("show");
        let id = $(this).data("batch");

        $.ajax({
            beforeSend: function () {
                $("#elementOverlay").LoadingOverlay("show");
            },
            url: "/EDMS/IsBatchLockedOrder",
            type: "POST",
            dataType: "JSON",
            complete: function () {
                enCours = false; // Réinitialisation après la requête (réussie ou non)
            },
            data: {
                step: step,
                batchId: id,
            },
            error: function (xhr, status, error) {
                alertCustom("danger", 'fa fa-close', error);
                $("#elementOverlay").LoadingOverlay("hide", true);
            }, success: function (res) {

                if (res.isLocked) {
                    alert("batch locked by " + res.userlock);
                    $("#elementOverlay").LoadingOverlay("hide", true);
                } else {
                    submitFormWithPost(res.urlredirect, {
                        step: step,
                        batchId: id,
                    });
                }
            },
        });

    });


}

function RejectBatch(batchid, step) {

    if (enCours) return; // Empêche les clics multiples
    enCours = true;
    $("#elementOverlay").LoadingOverlay("show");

    $.ajax({
        beforeSend: function () {
        },
        url: "/EDMS/RejectBatch",
        type: "POST",
        dataType: "JSON",
        data: {
            reason: $("#rejectiocode").val(),
            otherreason: $("#otherreason").val(),
            batchid: batchid
        },
        complete: function () {
            enCours = false; // Réinitialisation après la requête (réussie ou non)
        },
        error: function (xhr, status, error) {
            alertCustom("danger", 'fa fa-close', error);
            $("#elementOverlay").LoadingOverlay("hide", true);

        }, success: function (res) {

            $("#RejectionModal").modal('hide');
            alertCustom("success", 'fa fa-check', "Rejection completed successfully");
            $("#elementOverlay").LoadingOverlay("hide", true);
            Load_index_crop_sanity_control(step);


        },
    });


}

function submitFormWithPost(actionUrl, data) {
    // Créer le formulaire
    let form = $('<form>', {
        method: 'POST',
        action: actionUrl,
    });

    // Ajouter dynamiquement les champs d'entrée
    for (const [key, value] of Object.entries(data)) {
        form.append($('<input>', {
            type: 'hidden',
            name: key,
            value: value,
        }));
    }

    // Ajouter le formulaire au DOM, le soumettre et le supprimer
    form.appendTo('body').submit().remove();
}

//function SetCropper(initialise) {

//    if (initialise) {
//        $('#cropper-img').addClass('ready');
//        if (isInitialized === true) {
//            $('#zoom-slider').val(0);
//            cropper.destroy();
//        }

//        initCropper();


//    }
//    else {
//        cropper.rotate(rotate);
//        cropper.scaleX(flipX);
//        cropper.scaleY(flipY);
//        cropper.zoom(zoom);

//    }

//    return false;
//}


//function initCropper() {
//    if ($('#cropper-imgrecto').css('display') === 'none') {
//        var vEl = document.getElementById('cropper-imgverso');
//    } else {
//        var vEl = document.getElementById('cropper-imgrecto');
//    }
//    if (vEl != null) {

//        cropper = new Cropper(vEl, {
//            viewMode: 2,
//            dragMode: 'move',
//            aspectRatio: 0,
//            checkOrientation: false,
//            cropBoxMovable: true,
//            cropBoxResizable: true,
//            zoomOnTouch: false,
//            zoomOnWheel: true,
//            guides: false,
//            highlight: false,
//            ready: function (e) {

//                var cropper = this.cropper;
//                var imageData = cropper.getImageData();
//                var minSliderZoom = imageData.width / imageData.naturalWidth;
//                cropper.width = 500;
//                cropper.naturalHeight = 500;


//                //remove crop box if not cropped
//                if (!crop) {
//                    if (rotate > 0 || zoom != 0 || isFlipped) {
//                        $("div").remove(".cropper-crop-box");
//                    }
//                }

//                cropper.rotate(rotate);
//                cropper.scaleX(flipX);
//                cropper.zoom(zoom);
//                zoom = 0;
//                rotate = 0;
//                isFlipped = false;
//            }
//        });
//        isInitialized = true;
//    }
//}

