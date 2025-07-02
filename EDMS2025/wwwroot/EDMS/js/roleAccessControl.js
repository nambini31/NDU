$(document).ready(function (){
    initAccessibleMenu();

    var ElementMyId = null;

    $("#update-access-boutton").attr("disabled", true)



    $('#folderForm').on('submit', function (e) {
        e.preventDefault();
        const folderPath = $('#folderPath').val().trim();

        if (!folderPath) {
            alert("Veuillez entrer le chemin du dossier.");
            return;
        }

        $("#elementOverlay").LoadingOverlay("show");
        $.ajax({
            url: '/EDMS/GetAllTrademarksWithPages', // Action du contrôleur
            type: 'POST',
            data: { folderPath: folderPath },
            success: function (res) {
                //$('#resultMessage').html("✅ " + response.message);
                $("#elementOverlay").LoadingOverlay("hide", true);
                var tableElement = $("#table_report");

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
                    dom: "Bfrtip",

                    initComplete: function (settings, json) {
                        $('div.dataTables_wrapper div.dataTables_filter input').attr('placeholder', 'Search');
                        $('#searchalldocs').on('keyup', function () {
                            tableElement.DataTable().search(this.value).draw();
                        });


                    }
                });
            },
            error: function (xhr, status, error) {
                $('#resultMessage').html("❌ Erreur AJAX : " + error);
                alertCustom("danger", 'fa fa-close', error);
                $("#elementOverlay").LoadingOverlay("hide", true);
            }
        });
    });


    $('#excelfilesuploads').on('submit', function (e) {
        e.preventDefault();

        var fileInput = $('#excelFile')[0];
        var file = fileInput.files[0];

        if (!file || !(/\.(xls|xlsx)$/i).test(file.name)) {
            alert("Veuillez sélectionner un fichier Excel valide (.xls ou .xlsx).");
            return;
        }

        $("#elementOverlay").LoadingOverlay("show");


        var formData = new FormData();
        formData.append('excelFile', file);

        $.ajax({
            url: '/EDMS/upload_excel', // <-- ton endpoint serveur
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                alertCustom("success", 'fa fa-check', "Success");
                $("#elementOverlay").LoadingOverlay("hide", true);

            },
            error: function (xhr, status, error) {
                $('#resultMessage').html("❌ Erreur AJAX : " + error);
                alertCustom("danger", 'fa fa-close', error);
                $("#elementOverlay").LoadingOverlay("hide", true);
            }
        });

    });


});

function updateAccessRole() {
    let roleId;
    const updateButton = $('#update-role');
    if(updateButton) {
        updateButton.html("<i class='fa fa-spinner fa-pulse'></i> Updating...");
    }
    const roleIdDOM = $('#RoleId');
    const selectedMenu = $('.menu-checkbox');
    if(selectedMenu && roleIdDOM) {
        roleId = roleIdDOM.val();
        let menu = [];
       for(let i = 0; i < selectedMenu.length; i++) {
           if(selectedMenu[i].checked) {
               menu.push(selectedMenu[i].value);
           }
       }
        $.ajax({
            type: "POST",
            url: "/rolemanagement/updateroleaccess",
            data: {roleId: roleId, menu: menu},
            dataType: 'json',
            success: function (data) {
                if (data.status === 1) {
                    updateButton.html("<i class='fa fa-check-circle'></i> Updated");
                    updateButton.attr("class", "btn btn-success float-right");
                    location.reload();
                }
                else {
                    updateButton.html("<i class='fa fa-times-circle'></i> Failed");
                    updateButton.attr("class", "btn btn-danger float-right");
                }
            },
            error: function (err) {
                updateButton.html("<i class='fa fa-times-circle'></i> Failed");
                updateButton.attr("class", "btn btn-danger float-right");
            }}
        );
    }
}

function updateAccessBoutton() {
    const updateButton = $('#update-access-boutton');
    if (updateButton) {
        updateButton.html("<i class='fa fa-spinner fa-pulse'></i> Updating...");
    }
    const selectedButton = $(".acces-boutton-checkbox");
    if (selectedButton) {
        let notSelectedBnts = []; 
        let selectedBtns = [];
        for (let i = 0; i < selectedButton.length; i++) {
            if (!selectedButton[i].checked) {
                notSelectedBnts.push(selectedButton[i].value); // Add the value of unchecked elements
            }
        }
        for (let i = 0; i < selectedButton.length; i++) {
            if (selectedButton[i].checked) {
                selectedBtns.push(selectedButton[i].value);
            }
        }
        $.ajax({
            type: "POST",
            url: "/rolemanagement/UpdateAccessButton",
            data: { MyId: ElementMyId, selectedBtns: selectedBtns, notSelectedBnts: notSelectedBnts },
            dataType: 'json',
            success: function (data) {
                if (data.status === 1) {
                    updateButton.html("<i class='fa fa-check-circle'></i> Updated");
                    updateButton.attr("class", "btn btn-success float-right");
                    location.reload();
                }
                else {
                    updateButton.html("<i class='fa fa-times-circle'></i> Failed");
                    updateButton.attr("class", "btn btn-danger float-right");
                }
            },
            error: function (err) {
                updateButton.html("<i class='fa fa-times-circle'></i> Failed");
                updateButton.attr("class", "btn btn-danger float-right");
            }
        }
        );
    }
}

function initAccessibleMenu() {
    const updateButton = $('#update-role');
    if(updateButton) {
        updateButton.html("<i class='fa fa-save'></i> Update");
        updateButton.attr("class", "btn btn-info float-right");
    }
    const selectedRoleId = $('#RoleId').val();
    if(selectedRoleId) {
        $.ajax({
                type: "GET",
                url: "/rolemanagement/getroleaccessiblemenu",
                data: { roleId: selectedRoleId, },
                dataType: 'json',
                success: function (data) {
                    setSelectedMenu(data);
                },
                error: function (err) {
                    alert('An error has occured: ' + err);
                }
            }
        );
    }
}

function setSelectedMenu(data) {
    uncheckAllMenu();
    for(let i = 0; i < data.data.length; i++) {
        const menuId = "menu-"+data.data[i];
        let menu = document.getElementById(menuId);
        if(menu) {
            menu.checked = true;
        }
    }
}

function uncheckAllMenu() {
    const selectedMenu = $('.menu-checkbox');
    if(selectedMenu) {
        for(let i = 0; i < selectedMenu.length; i++) {
            selectedMenu[i].checked = false;
        }
    }
}

function toggleActive(elementId, MyId) {
    const listItems = document.querySelectorAll('.list-item');
    listItems.forEach(item => item.classList.remove('active'));
    const listItem = document.getElementById(`list-item-${elementId}`);
    if (listItem) {
        listItem.classList.toggle('active');
        ElementMyId = MyId;
        selectedBoutton(MyId);
        const selectedMenu = $('.menu-checkbox');
        $("#update-access-boutton").attr("disabled", false)
    }
}

function selectedBoutton(MyId) {
    $.ajax({
        type: "GET",
        url: "/rolemanagement/getAccesBouttonByElemntMyId",
        data: { MyId: MyId, },
        dataType: 'json',
        success: function (data) {
            uncheckAllBoutton()
            for (let i = 0; i < data.data.length; i++) {
                const bouttonId = "acces-button-" + data.data[i];
                let boutton = document.getElementById(bouttonId);
                if (boutton) {
                    boutton.checked = true;
                }
            }
        },
        error: function (err) {
            alert('An error has occured: ' + err);
        }
    }
    );
}

function uncheckAllBoutton() {
    const selectedMenu = $('.acces-boutton-checkbox');
    if (selectedMenu) {
        for (let i = 0; i < selectedMenu.length; i++) {
            selectedMenu[i].checked = false;
        }
    }
}
