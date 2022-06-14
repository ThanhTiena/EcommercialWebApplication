// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let password = document.getElementById("register-password");
let repeatPass = document.getElementById("register-repeat-password");
let errorSpan = document.getElementById("repeat-pasword-error");
errorSpan.style.display = "none";
let btn = document.getElementById("registerSubmit");
console.log(repeatPass.nodeValue);

function checkRepeat() {
    if (repeatPass.value !== password.value) {
        errorSpan.style.display = "block";
        btn.disabled = true;

    } else {
        errorSpan.style.display = "none";
        btn.disabled = false;
    }
}