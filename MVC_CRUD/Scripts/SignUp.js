$(document).ready(function () {
    
        // Toggle password visibility
        $('.toggle-password').click(function () {
            var toggleId = $(this).attr('toggle');
            var passwordField = $(toggleId);
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
                $(this).click(); // Trigger the click event
            }
        });
    

    $('#password').on('keyup', function () {
        validatePassword();
    });

    $('#confirmPassword').on('keyup', function () {
        validateConfirmPassword();
    });

    $('#mobileNo').on('keyup', function () {
        validateMobileNo();
    });

    $('#signupForm').submit(function (e) {
        if (!validatePassword() || !validateConfirmPassword() || !validateMobileNo()) {
            e.preventDefault();
            return false;
        }
        return true;
    });

    function validatePassword() {
        var password = $('#password').val();
        var isValid = true;
        var errorMessage = '';

        if (password.length < 8) {
            isValid = false;
            errorMessage += 'Password must be at least 8 characters long.<br />';
        }
        if (!/[A-Z]/.test(password)) {
            isValid = false;
            errorMessage += 'Password must contain at least one uppercase letter.<br />';
        }
        if (!/[a-z]/.test(password)) {
            isValid = false;
            errorMessage += 'Password must contain at least one lowercase letter.<br />';
        }
        if (!/[0-9]/.test(password)) {
            isValid = false;
            errorMessage += 'Password must contain at least one number.<br />';
        }
        if (!/[!@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
            isValid = false;
            errorMessage += 'Password must contain at least one special character.<br />';
        }

        $('#passwordError').html(errorMessage);

        if (!isValid) {
            $('#password').addClass('is-invalid');
        } else {
            $('#password').removeClass('is-invalid');
        }

        return isValid;
    }

    function validateConfirmPassword() {
        var password = $('#password').val();
        var confirmPassword = $('#confirmPassword').val();

        if (password !== confirmPassword) {
            $('#confirmPasswordError').text('Passwords do not match.');
            $('#confirmPassword').addClass('is-invalid');
            return false;
        } else {
            $('#confirmPasswordError').text('');
            $('#confirmPassword').removeClass('is-invalid');
            return true;
        }
    }

    function validateMobileNo() {
        var mobileNo = $('#mobileNo').val();
        var isValid = true;
        var errorMessage = '';

        // Pakistan mobile number validation: starts with 03 followed by 9 digits
        var pakistanMobileNoPattern = /^03\d{9}$/;

        if (!pakistanMobileNoPattern.test(mobileNo)) {
            isValid = false;
            errorMessage = 'Mobile number must be a valid Pakistan number (e.g., 03001234567).';
        }

        $('#mobileNoError').text(errorMessage);

        if (!isValid) {
            $('#mobileNo').addClass('is-invalid');
        } else {
            $('#mobileNo').removeClass('is-invalid');
        }

        return isValid;
    }
});
