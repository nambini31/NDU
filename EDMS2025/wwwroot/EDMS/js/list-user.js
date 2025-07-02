
function selectUser(userId) {
    const lines = $('.line-users.selected-user');
    if(lines) {
        lines.attr('class', 'line-users')
    }
    const selectedLine = document.getElementById(`line-${userId}`);
    if(selectedLine) {
        selectedLine.setAttribute('class', 'line-users selected-user');
    }
    const selectedUser = document.getElementById(`user-${userId}`);
    if(selectedUser) {
        selectedUser.checked = true;
    }
    const loaderRole = document.getElementById('loader_role');
    if(loaderRole) loaderRole.removeAttribute('style');
    const roleRadioButton = $('input[name=role]');
    if(roleRadioButton) {
        roleRadioButton.prop('checked', false);
        $.ajax({
                type: "GET",
                url: "/rolemanagement/LoadUserRole",
                data: {userId: userId},
                dataType: 'json',
                success: function (data) {
                    if (data.status === 1) {
                        const radioToCheck = document.getElementById(`role-${data.roleId}`);
                        if (radioToCheck) radioToCheck.checked = true;
                    } else {
                        roleRadioButton.prop('checked', false);
                    }
                    if (loaderRole) loaderRole.setAttribute('style', 'display: none');
                },
                error: function (err) {
                    if (loaderRole) loaderRole.setAttribute('style', 'display: none');
                    roleRadioButton.prop('checked', false);
                }
            }
        );
    }
}

function saveUserRole()
{
    const selectedRole = $('input[name=role]:checked');
    const selectedUser = $('input[name=selected-user]:checked');
    const saveButton = $(`#save-role`);
    if (selectedRole && saveButton && selectedUser) {
        const selectedRoleValue = selectedRole.val();
        const userId = selectedUser.val();
        saveButton.attr("class", "btn btn-primary");
        saveButton.html("<i class='fas fa-spinner fa-pulse'></i> Sauvegarde en cours...");
        $.ajax({
            type: "POST",
            url: "/rolemanagement/setuserrole",
            data: {roleId: selectedRoleValue, userId: userId},
            dataType: 'json',
            success: function (data) {
                if (data.status === 1) {
                    saveButton.html("<i class='fas fa-check-circle'></i> Enregistré");
                    saveButton.attr("class", "btn btn-success");
                    const tdRoleLabel = $('#label-role-' + userId);
                    if(tdRoleLabel) tdRoleLabel.html($('input[name=role]:checked + label').text());
                }
                else {
                    saveButton.html("<i class='fas fa-times-circle'></i> Échoué");
                    saveButton.attr("class", "btn btn-danger");
                }
            },
            error: function (err) {
                saveButton.html("<i class='fas fa-times-circle'></i> Échoué");
                saveButton.attr("class", "btn btn-danger");
            }}
        );
    }
}

function confirmDelete(userId) {
    Swal.fire({
        title: 'Confirmation',
        text: "Vous êtes sûr de vouloir supprimer?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Confirmer',
        cancelButtonText: 'Annuler'
    }).then((result) => {
        if (result.value) {
            deleteUser(userId);
        }
    })
}

function deleteUser(userId) {
    let buttonId = `#delete-user-button-${userId}`;
    if($(buttonId)) $(buttonId).html("<i class='fas fa-spinner fa-pulse'></i>");
    if(userId) {
        $.ajax({
            type: "POST",
            url: "/user/delete",
            data: {userId: userId },
            dataType: 'json',
            success: function (data) {
                if(data.status === 1) {
                    window.location = data.Url+"?deleted=true";
                }
                else {
                    if($(buttonId)) $(buttonId).html("<i class='fas fa-trash'></i>");
                    Swal.fire({
                        title: 'Echec',
                        text: "Nous n'avons pas pu supprimer l'utilisateur!",
                        icon: 'error',
                        showCancelButton: false,
                        confirmButtonText: 'OK',
                    });
                }
            },
            error: function (err) {
                if($(buttonId)) $(buttonId).html("<i class='fas fa-trash'></i>");
                Swal.fire({
                    title: 'Echec',
                    text: "Nous n'avons pas pu supprimer l'utilisateur!",
                    icon: 'error',
                    showCancelButton: false,
                    confirmButtonText: 'OK',
                });
            }}
        );
    }
    else {
        
    }
}

function showSuccess() {
    Swal.fire({
        title: 'Succès',
        text: "L'utilisateur est supprimé avec succès!",
        icon: 'success',
        showCancelButton: false,
        confirmButtonText: 'OK',
    });
}