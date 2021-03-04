$(() => {
    $(".form-control").on('keyup', function () {
        ensureFormValidity();
    })
    function ensureFormValidity() {
        const isValid = isFormValid();
        $("#submit").prop('disabled', !isValid);
    }
    function isFormValid() {
        const name = $("#name").val();
        const text = $("#body").val();
        return name && text;
    }
})