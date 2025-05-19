function ShowModal(name) {
    console.log(name);
    bootstrap.Modal.getOrCreateInstance(document.getElementById(name)).show();
}

function HideModal(name) {
    console.log(name);
    bootstrap.Modal.getOrCreateInstance(document.getElementById(name)).hide();
}