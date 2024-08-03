$(document).ready(function () {
    $('.toggle-password').click(function () {
        var passwordField = $('#password');
        var fieldType = passwordField.attr('type');
        if (fieldType === 'password') {
            passwordField.attr('type', 'text');
            $(this).find('i').removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordField.attr('type', 'password');
            $(this).find('i').removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Handle keyboard interaction for accessibility
    $('.toggle-password').keypress(function (e) {
        var key = e.which || e.keyCode;
        if (key === 13 || key === 32) { // Enter or Spacebar
            $(this).click();
        }
    });
});