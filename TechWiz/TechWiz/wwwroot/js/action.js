document.addEventListener("DOMContentLoaded", function () {
    const btnAddDestination = document.querySelector('.btn-add');
    const formActionAdd = document.getElementById('form-action-add');
    const formActionEdit = document.getElementById('form-action-edit');
    const formCloseAdd = document.querySelector('#form-action-add .form-close');
    const formCloseEdit = document.querySelector('#form-action-edit .form-close');
    const editButtons = document.querySelectorAll('.bx-edit');

    btnAddDestination.addEventListener('click', function (event) {
        event.preventDefault();
        formActionAdd.style.display = 'flex';
    });
    editButtons.forEach(function (editButton) {
        editButton.addEventListener('click', function (event) {
            event.preventDefault();
            formActionEdit.style.display = 'flex';
        });
    });
    formCloseAdd.addEventListener('click', function () {
        formActionAdd.style.display = 'none';
    });
    formCloseEdit.addEventListener('click', function () {
        formActionEdit.style.display = 'none';
    });
});
