document.addEventListener('DOMContentLoaded', function () {
    var successMessage = document.getElementById('successMessage');
    if (successMessage) {
        setTimeout(function () {
            successMessage.style.display = 'none';
        }, 2000); // 4 seconds
    }
});