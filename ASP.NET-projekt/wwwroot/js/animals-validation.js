(function () {
  // Validate fields on blur and when custom controls update hidden inputs
  function validateOnBlur() {
    $(document).on('blur', 'form input, form select, form textarea', function () {
      var $el = $(this);

      // Autocomplete: validate the hidden input that carries the real value
      if ($el.is('[data-autocomplete-input]')) {
        var hiddenId = $el.attr('id').replace('__input', '');
        var $hidden = $('#' + hiddenId);
        if ($hidden.length) { $hidden.valid(); return; }
      }

      // DateTime picker: validate the hidden input bound to the picker
      if ($el.is('[data-datetime-picker]')) {
        var fieldName = $el.data('field-name');
        if (fieldName) { $('#' + fieldName).valid(); return; }
      }

      // Default: validate the element itself
      if ($el.length && typeof $el.valid === 'function') {
        $el.valid();
        $el.attr('aria-invalid', $el.hasClass('input-validation-error') ? 'true' : 'false');
      }
    });

    // When hidden inputs changed by custom controls are updated, validate them
    $(document).on('change', 'input[data-datetime-value], input[data-autocomplete-value]', function () {
      var $this = $(this);
      if (typeof $this.valid === 'function') {
        $this.valid();
        $this.attr('aria-invalid', $this.hasClass('input-validation-error') ? 'true' : 'false');
      }
    });

    $(document).on('submit', 'form', function () {
      var $form = $(this);
      var validator = $form.data('validator');

      if (validator && !validator.form()) {
        var $firstInvalid = $form.find('.input-validation-error').first();
        if ($firstInvalid.length) {
          $firstInvalid.trigger('focus');
        }
        return false;
      }

      return true;
    });
  }

  // Initialize when DOM ready and when unobtrusive validation is present
  $(function () {
    $('.validation-summary-errors').attr({ role: 'alert', 'aria-live': 'polite' });
    $('.field-validation-error').attr('role', 'alert');

    // Wait briefly to ensure jquery.validate/unobtrusive hooked up
    setTimeout(validateOnBlur, 10);
  });
})();
