﻿$(() => {
    $(".form-control").on('keyup', function () {
        ensureFormValidity();
    })
    function ensureFormValidity() {
        const isValid = isFormValid();
        $("#submit").prop('disabled', !isValid);
    }
    function isFormValid() {
        const title = $("#title").val();
        const text = $("#body").val();
        return title && text;
    }
})