// wwwroot/js/site.js

// Document ready function
(function () {
    'use strict';

    // Initialize the page
    document.addEventListener('DOMContentLoaded', function () {
        console.log('Application initialized');

        // Add active class to current nav link
        var currentPath = window.location.pathname;
        var navLinks = document.querySelectorAll('.nav-link');

        navLinks.forEach(function (link) {
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
            }
        });

        // Initialize tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Home page specific functionality
        if (currentPath === '/') {
            initializeHomePage();
        }
    });

    /**
     * Initializes home page specific functionality
     */
    function initializeHomePage() {
        console.log('Initializing home page features');

        // Add animation to feature cards
        var cards = document.querySelectorAll('.feature-card');

        cards.forEach(function (card, index) {
            // Add delay based on index for staggered animation
            card.style.transitionDelay = (index * 0.1) + 's';
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';

            // Animate in
            setTimeout(function () {
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, 300 + (index * 100));
        });

        // Add click tracking for analytics (example)
        document.querySelectorAll('.btn-primary').forEach(function (button) {
            button.addEventListener('click', function () {
                console.log('Button clicked: ' + button.textContent.trim());
            });
        });
    }
})();