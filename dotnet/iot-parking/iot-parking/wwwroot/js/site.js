// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function changeHasOwner() {
    var check = document.getElementById("hasOwnerCheck");
    var inputs = Array.from(document.getElementsByClassName("owner"));

    if (!check.checked) {
        inputs.forEach(el => el.disabled = true);
    }
    else {
        inputs.forEach(el => el.disabled = false);
    }
}