window.ShowToastr = function (message, type) {
    if (type === 'success') {
        console.log(message);
        toastr.success(message);
    } else if (type === 'error') {
        console.log(message);
        toastr.error(message);
    } 
};

