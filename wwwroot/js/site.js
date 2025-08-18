// Document ready function
document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // File upload preview
    const fileUploads = document.querySelectorAll('.custum-file-upload input[type="file"]');
    fileUploads.forEach(function (upload) {
        upload.addEventListener('change', function (e) {
            const container = this.closest('.custum-file-upload');
            const textSpan = container.querySelector('.text span');

            if (this.files.length > 0) {
                textSpan.textContent = this.files[0].name;
                container.style.borderColor = '#0071E3';
            } else {
                textSpan.textContent = 'Click to upload file';
                container.style.borderColor = '#D2D2D7';
            }
        });
    });

    // Button ripple effect
    const buttons = document.querySelectorAll('button');
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
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });
});