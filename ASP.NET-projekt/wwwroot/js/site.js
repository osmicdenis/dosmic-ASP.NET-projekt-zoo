(function () {
  function initAutocomplete(control) {
    const searchUrl = control.dataset.autocompleteSearchUrl || control.dataset.searchUrl;
    const valueField = control.dataset.valueField || 'id';
    const labelField = control.dataset.labelField || 'name';
    const queryParam = control.dataset.queryParam || 'query';
    const minCharacters = Number(control.dataset.minCharacters || 1);

    const hiddenInput = control.querySelector('[data-autocomplete-value]');
    const textInput = control.querySelector('[data-autocomplete-input]');
    const dropdown = control.querySelector('[data-autocomplete-dropdown]');
    const clearButton = control.querySelector('[data-autocomplete-clear]');

    let timerId = null;
    let items = [];
    let activeIndex = -1;

    function hideDropdown() {
      dropdown.hidden = true;
      dropdown.innerHTML = '';
      activeIndex = -1;
    }

    function setSelected(item) {
      hiddenInput.value = item?.[valueField] ?? '';
      textInput.value = item?.[labelField] ?? '';
      hideDropdown();
    }

    function renderItems(resultItems) {
      dropdown.innerHTML = '';
      activeIndex = -1;

      if (!resultItems.length) {
        const emptyItem = document.createElement('button');
        emptyItem.type = 'button';
        emptyItem.className = 'list-group-item list-group-item-action disabled';
        emptyItem.textContent = 'No matches found';
        dropdown.appendChild(emptyItem);
        dropdown.hidden = false;
        return;
      }

      resultItems.forEach((item, index) => {
        const optionButton = document.createElement('button');
        optionButton.type = 'button';
        optionButton.className = 'list-group-item list-group-item-action autocomplete-option';
        optionButton.textContent = item?.[labelField] ?? '';
        optionButton.dataset.index = String(index);
        optionButton.addEventListener('click', () => setSelected(item));
        dropdown.appendChild(optionButton);
      });

      dropdown.hidden = false;
    }

    function highlightActive() {
      const optionButtons = Array.from(dropdown.querySelectorAll('.autocomplete-option'));
      optionButtons.forEach((button, index) => {
        button.classList.toggle('active', index === activeIndex);
      });
      if (activeIndex >= 0 && optionButtons[activeIndex]) {
        optionButtons[activeIndex].scrollIntoView({ block: 'nearest' });
      }
    }

    function moveActive(step) {
      const optionButtons = Array.from(dropdown.querySelectorAll('.autocomplete-option'));
      if (!optionButtons.length) {
        return;
      }

      activeIndex = (activeIndex + step + optionButtons.length) % optionButtons.length;
      highlightActive();
    }

    function fetchResults(query) {
      if (query.length < minCharacters) {
        hideDropdown();
        return;
      }

      fetch(`${searchUrl}?${encodeURIComponent(queryParam)}=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(resultItems => {
          items = Array.isArray(resultItems) ? resultItems : [];
          renderItems(items);
        })
        .catch(() => {
          hideDropdown();
        });
    }

    textInput.addEventListener('input', () => {
      hiddenInput.value = '';
      window.clearTimeout(timerId);
      timerId = window.setTimeout(() => fetchResults(textInput.value.trim()), 200);
    });

    textInput.addEventListener('focus', () => {
      if (textInput.value.trim().length >= minCharacters) {
        fetchResults(textInput.value.trim());
      }
    });

    textInput.addEventListener('keydown', event => {
      if (dropdown.hidden) {
        return;
      }

      if (event.key === 'ArrowDown') {
        event.preventDefault();
        moveActive(1);
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        moveActive(-1);
      } else if (event.key === 'Enter') {
        const selectedItem = activeIndex >= 0 ? items[activeIndex] : items[0];
        if (selectedItem) {
          event.preventDefault();
          setSelected(selectedItem);
        }
      } else if (event.key === 'Escape') {
        hideDropdown();
      }
    });

    clearButton.addEventListener('click', () => {
      hiddenInput.value = '';
      textInput.value = '';
      hideDropdown();
      textInput.focus();
    });

    document.addEventListener('click', event => {
      if (!control.contains(event.target)) {
        hideDropdown();
      }
    });
  }

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-autocomplete-control]').forEach(initAutocomplete);
  });
})();

(function () {
  function getLocaleConfig(locale) {
    const localePrefix = locale.split('-')[0].toLowerCase();

    if (localePrefix === 'hr') {
      return {
        locale: 'hr',
        dateFormat: 'd.m.Y',
        timeFormat: 'H:i',
        time24hr: true,
        shorthand: false,
      };
    }

    // Default to English
    return {
      locale: 'en',
      dateFormat: 'm/d/Y',
      timeFormat: 'h:i K',
      time24hr: false,
      amPM: true,
      shorthand: false,
    };
  }

  function initDateTimeControl(pickerInput) {
    const fieldName = pickerInput.dataset.fieldName;
    const hiddenInput = document.querySelector(`[data-datetime-value][id="${fieldName}"]`);
    const enableTime = pickerInput.dataset.enableTime === 'true';

    if (!hiddenInput) {
      console.warn(`Hidden input for field ${fieldName} not found`);
      return;
    }

    // Detect browser locale
    const browserLocale = navigator.language || 'en-US';
    const localeConfig = getLocaleConfig(browserLocale);

    // Get initial value if exists
    const initialValue = hiddenInput.value ? new Date(hiddenInput.value) : null;

    // Initialize Flatpickr
    flatpickr(pickerInput, {
      enableTime: enableTime,
      dateFormat: localeConfig.dateFormat,
      timeFormat: localeConfig.timeFormat,
      time24hr: localeConfig.time24hr,
      locale: localeConfig.locale,
      defaultDate: initialValue,
      minuteIncrement: 1,
      onClose: (selectedDates) => {
        if (selectedDates.length > 0) {
          const date = selectedDates[0];
          // Format for server: YYYY-MM-DD HH:mm or YYYY-MM-DD depending on enableTime
          const year = date.getFullYear();
          const month = String(date.getMonth() + 1).padStart(2, '0');
          const day = String(date.getDate()).padStart(2, '0');
          const hours = String(date.getHours()).padStart(2, '0');
          const minutes = String(date.getMinutes()).padStart(2, '0');

          if (enableTime) {
            hiddenInput.value = `${year}-${month}-${day} ${hours}:${minutes}`;
          } else {
            hiddenInput.value = `${year}-${month}-${day}`;
          }
        }
      },
    });
  }

  document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-datetime-picker]').forEach(initDateTimeControl);
  });
})();