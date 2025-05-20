document.addEventListener('DOMContentLoaded', function () {
    // Get carousel elements
    const slides = document.querySelectorAll('.team-slide');
    const prevButton = document.getElementById('prevSlide');
    const nextButton = document.getElementById('nextSlide');

    // Set initial slide index
    let currentSlideIndex = 0;
    let slideInterval;

    // Function to show a specific slide
    function showSlide(index) {
        // Hide all slides
        slides.forEach(slide => {
            slide.classList.remove('active');
        });

        // Show the current slide
        slides[index].classList.add('active');
    }

    // Function to show the next slide
    function nextSlide() {
        currentSlideIndex++;
        if (currentSlideIndex >= slides.length) {
            currentSlideIndex = 0;
        }
        showSlide(currentSlideIndex);
    }

    // Function to show the previous slide
    function prevSlide() {
        currentSlideIndex--;
        if (currentSlideIndex < 0) {
            currentSlideIndex = slides.length - 1;
        }
        showSlide(currentSlideIndex);
    }

    // Add event listeners to buttons
    if (prevButton) {
        prevButton.addEventListener('click', function () {
            prevSlide();
            resetInterval();
        });
    }

    if (nextButton) {
        nextButton.addEventListener('click', function () {
            nextSlide();
            resetInterval();
        });
    }

    // Function to reset the auto-rotation interval
    function resetInterval() {
        clearInterval(slideInterval);
        startInterval();
    }

    // Function to start the auto-rotation interval
    function startInterval() {
        slideInterval = setInterval(nextSlide, 8000);
    }

    // Start the auto-rotation
    startInterval();
});