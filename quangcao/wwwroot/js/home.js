document.addEventListener('DOMContentLoaded', function () {
    // Cache DOM elements
    const testimonialsSlider = document.getElementById('testimonialsSlider');
    const testimonialPrev = document.getElementById('testimonialPrev');
    const testimonialNext = document.getElementById('testimonialNext');
    const testimonials = document.querySelectorAll('.testimonial-card');
    const contactForm = document.getElementById('contactForm');
    const starsContainer = document.getElementById('stars');

    // Testimonials slider
    if (testimonialsSlider && testimonialPrev && testimonialNext && testimonials.length > 0) {
        let currentIndex = 0;
        let autoplayInterval;

        // Hiển thị testimonial hiện tại
        function showTestimonial(index) {
            testimonials.forEach((testimonial, i) => {
                const isVisible = i === index;
                testimonial.style.display = isVisible ? 'block' : 'none';
                testimonial.setAttribute('aria-hidden', !isVisible);
            });
        }

        // Chuyển đến testimonial tiếp theo
        function nextTestimonial() {
            currentIndex = (currentIndex + 1) % testimonials.length;
            showTestimonial(currentIndex);
        }

        // Chuyển đến testimonial trước đó
        function prevTestimonial() {
            currentIndex = (currentIndex - 1 + testimonials.length) % testimonials.length;
            showTestimonial(currentIndex);
        }

        // Bắt đầu autoplay
        function startAutoplay() {
            stopAutoplay();
            autoplayInterval = setInterval(nextTestimonial, 5000);
        }

        // Dừng autoplay
        function stopAutoplay() {
            if (autoplayInterval) {
                clearInterval(autoplayInterval);
            }
        }

        // Khởi tạo hiển thị
        function initTestimonials() {
            if (window.innerWidth < 768) {
                showTestimonial(currentIndex);
                startAutoplay();
            } else {
                testimonials.forEach(testimonial => {
                    testimonial.style.display = 'block';
                    testimonial.setAttribute('aria-hidden', 'false');
                });
                stopAutoplay();
            }
        }

        // Xử lý nút next
        testimonialNext.addEventListener('click', function () {
            if (window.innerWidth < 768) {
                nextTestimonial();
                stopAutoplay();
                startAutoplay();
            }
        });

        // Xử lý nút prev
        testimonialPrev.addEventListener('click', function () {
            if (window.innerWidth < 768) {
                prevTestimonial();
                stopAutoplay();
                startAutoplay();
            }
        });

        // Xử lý responsive với debounce
        let resizeTimeout;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(initTestimonials, 200);
        });

        // Khởi tạo
        initTestimonials();

        // Dừng autoplay khi hover
        testimonialsSlider.addEventListener('mouseenter', stopAutoplay);
        testimonialsSlider.addEventListener('mouseleave', function () {
            if (window.innerWidth < 768) {
                startAutoplay();
            }
        });
    }

    // Form validation
    if (contactForm) {
        contactForm.addEventListener('submit', function (e) {
            const name = document.getElementById('name');
            const phone = document.getElementById('phone');
            const subject = document.getElementById('subject');
            const message = document.getElementById('message');
            let isValid = true;

            // Reset previous error messages
            document.querySelectorAll('.error-message').forEach(el => el.remove());

            // Validate required fields
            [name, phone, subject, message].forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    showError(field, 'Trường này không được để trống');
                }
            });

            // Validate phone number
            if (phone.value.trim()) {
                const phoneRegex = /^[0-9]{10,11}$/;
                if (!phoneRegex.test(phone.value.replace(/\s/g, ''))) {
                    isValid = false;
                    showError(phone, 'Số điện thoại không hợp lệ');
                }
            }

            if (!isValid) {
                e.preventDefault();
            }
        });

        // Helper function to show error message
        function showError(field, message) {
            const errorElement = document.createElement('div');
            errorElement.className = 'error-message';
            errorElement.textContent = message;
            errorElement.style.color = 'red';
            errorElement.style.fontSize = '0.875rem';
            errorElement.style.marginTop = '5px';
            field.parentNode.appendChild(errorElement);
            field.classList.add('error');
            field.focus();
        }

        // Clear error on input
        contactForm.querySelectorAll('input, textarea').forEach(field => {
            field.addEventListener('input', function () {
                this.classList.remove('error');
                const errorMessage = this.parentNode.querySelector('.error-message');
                if (errorMessage) {
                    errorMessage.remove();
                }
            });
        });
    }

    // Stars animation
    if (starsContainer) {
        // Tạo ngôi sao với kích thước ngẫu nhiên
        for (let i = 0; i < 50; i++) {
            const star = document.createElement('div');
            star.className = 'star';

            // Kích thước ngẫu nhiên
            const size = Math.random() * 2 + 1; // 1-3px
            star.style.width = `${size}px`;
            star.style.height = `${size}px`;

            // Vị trí ngẫu nhiên
            star.style.top = `${Math.random() * 100}%`;
            star.style.left = `${Math.random() * 100}%`;

            // Độ trễ animation ngẫu nhiên
            star.style.animationDelay = `${Math.random() * 10}s`;

            // Thời gian animation ngẫu nhiên
            star.style.animationDuration = `${Math.random() * 3 + 2}s`; // 2-5s

            starsContainer.appendChild(star);
        }
    }
        // Khởi tạo Swiper cho tin tức
        if (document.getElementById('newsSlider')) {
            const newsSwiper = new Swiper('#newsSlider', {
                slidesPerView: 1,
                spaceBetween: 30,
                loop: true,
                autoplay: {
                    delay: 5000,
                    disableOnInteraction: false,
                },
                pagination: {
                    el: '.swiper-pagination',
                    clickable: true,
                },
                navigation: {
                    nextEl: '.swiper-button-next',
                    prevEl: '.swiper-button-prev',
                },
                breakpoints: {
                    640: {
                        slidesPerView: 1,
                        spaceBetween: 20,
                    },
                    768: {
                        slidesPerView: 2,
                        spaceBetween: 30,
                    },
                    1024: {
                        slidesPerView: 3,
                        spaceBetween: 30,
                    },
                }
            });

            // Dừng autoplay khi hover
            const newsSlider = document.getElementById('newsSlider');
            newsSlider.addEventListener('mouseenter', function () {
                newsSwiper.autoplay.stop();
            });

            newsSlider.addEventListener('mouseleave', function () {
                newsSwiper.autoplay.start();
            });
        }
    });