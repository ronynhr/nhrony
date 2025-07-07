// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



function applyReadOnlyMode(formId, isReadOnly) {
    const form = document.getElementById(formId);
    if (!form || !isReadOnly) return;

    form.querySelectorAll("input, select, textarea, button[type='submit']").forEach(el => {
        el.disabled = true;
    });

    // Re-enable anchor tags if needed
    form.querySelectorAll("a").forEach(link => {
        link.classList.remove("disabled");
        link.style.pointerEvents = "auto";
        link.style.opacity = 1;
    });
}

// Optional utility for numeric formatting (can be reused globally)
function formatToCurrency(value) {
    if (!value) return "";
    let num = parseFloat(value.replace(/[^\d.-]/g, ''));
    if (isNaN(num)) return "";
    return num.toLocaleString('en-BD', { style: 'currency', currency: 'BDT', minimumFractionDigits: 2 });
}

function stripFormatting(value) {
    return value.replace(/[^\d.-]/g, '');
}

function restrictNumericInput(e) {
    const allowedKeys = ["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete", "Home", "End"];
    if (allowedKeys.includes(e.key)) return;
    if (/^\d$/.test(e.key)) return;
    if (e.key === '.' && !e.target.value.includes('.')) return;
    e.preventDefault();
}

function setupFormattedInput(id, formId) {
    const input = document.getElementById(id);
    if (!input) return;

    input.addEventListener('focus', function () {
        this.value = stripFormatting(this.value);
    });

    input.addEventListener('blur', function () {
        this.value = formatToCurrency(this.value);
    });

    input.addEventListener('keydown', restrictNumericInput);

    const form = document.getElementById(formId);
    if (form) {
        form.addEventListener('submit', () => {
            input.value = stripFormatting(input.value);
        });
    }

    if (input.value.trim() !== '') {
        input.value = formatToCurrency(input.value);
    }
}
