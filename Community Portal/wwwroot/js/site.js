// Community Service Portal - small UI helpers

(function () {
    // Prevent double submissions: disable the submit button and show a spinner once a valid form is posted.
    document.querySelectorAll('form[data-loading-text]').forEach(function (form) {
        form.addEventListener('submit', function () {
            var valid = !window.jQuery || !window.jQuery.validator || window.jQuery(form).valid();
            if (!valid) { return; }
            var button = form.querySelector('button[type="submit"]');
            if (button) {
                button.disabled = true;
                button.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>' +
                    form.getAttribute('data-loading-text');
            }
        });
    });

    // Auto-dismiss success alerts after a few seconds.
    document.querySelectorAll('.alert-success[data-autodismiss]').forEach(function (alert) {
        setTimeout(function () {
            if (window.bootstrap) { window.bootstrap.Alert.getOrCreateInstance(alert).close(); }
        }, 8000);
    });

    // Live character counter for textareas that declare data-counter.
    document.querySelectorAll('textarea[data-counter]').forEach(function (area) {
        var target = document.getElementById(area.getAttribute('data-counter'));
        var max = area.getAttribute('maxlength') || '2000';
        var update = function () { if (target) { target.textContent = area.value.length + ' / ' + max; } };
        area.addEventListener('input', update);
        update();
    });

    // Copy reference number to the clipboard.
    document.querySelectorAll('[data-copy]').forEach(function (button) {
        button.addEventListener('click', function () {
            var text = button.getAttribute('data-copy');
            if (navigator.clipboard) {
                navigator.clipboard.writeText(text).then(function () {
                    var original = button.innerHTML;
                    button.innerHTML = '<i class="bi bi-check2"></i> Copied';
                    setTimeout(function () { button.innerHTML = original; }, 1800);
                });
            }
        });
    });
})();
