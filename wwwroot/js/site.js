// Document ready function
document.addEventListener('DOMContentLoaded', function () {
    // Set current year in footer
    setCurrentYear();

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // File upload preview
    const fileUploads = document.querySelectorAll('.file-upload input[type="file"]');
    fileUploads.forEach(function (upload) {
        upload.addEventListener('change', function (e) {
            const container = this.closest('.file-upload');
            const textSpan = container.querySelector('.text span');

            if (this.files.length > 0) {
                const fileNames = Array.from(this.files).map(file => file.name).join(', ');
                textSpan.textContent = `${this.files.length} file(s) selected`;
                container.style.borderColor = 'var(--system-blue)';
            } else {
                textSpan.textContent = 'Click to upload file';
                container.style.borderColor = 'var(--system-border)';
            }
        });
    });

    // Button ripple effect
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(function (button) {
        button.addEventListener('click', function (e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const ripple = document.createElement('span');
            ripple.classList.add('ripple');
            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;

            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 1000);
        });
    });

    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });
});

// Set current year in footer
function setCurrentYear() {
    const yearElements = document.querySelectorAll('.copyright');
    const currentYear = new Date().getFullYear();

    yearElements.forEach(element => {
        element.textContent = element.textContent.replace('2025', currentYear);
    });
}