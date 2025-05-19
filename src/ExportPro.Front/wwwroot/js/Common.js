function ShowModal(name) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(name)).show();
}

function HideModal(name) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(name)).hide();
}