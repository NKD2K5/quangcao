//// Xử lý header cố định và hiệu ứng cuộn
//document.addEventListener("DOMContentLoaded", () => {
//    // Các biến cần thiết
//    const header = document.querySelector(".site-header");
//    const mainContent = document.querySelector(".main-content");
//    const menuToggle = document.getElementById("menuToggle");
//    const mobileNav = document.getElementById("mobileNav");
//    const accountDropdown = document.getElementById("accountDropdown");
//    const accountTrigger = accountDropdown?.querySelector(".account-trigger");
//    let lastScrollTop = 0;
//    let isScrolling = false;

//    // Điều chỉnh padding-top cho main-content dựa trên chiều cao của header
//    function adjustMainContentPadding() {
//        if (header && mainContent) {
//            const headerHeight = header.offsetHeight;
//            mainContent.style.paddingTop = headerHeight + "px";
//        }
//    }

//    // Xử lý sự kiện cuộn
//    function handleScroll() {
//        if (!isScrolling) {
//            isScrolling = true;

//            window.requestAnimationFrame(() => {
//                const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

//                // Thu gọn header khi cuộn xuống
//                if (scrollTop > 100) {
//                    header.classList.add("header-compact");

//                    // Thêm hiệu ứng ẩn/hiện header khi cuộn lên/xuống
//                    if (scrollTop > lastScrollTop && scrollTop > 300) {
//                        // Cuộn xuống - ẩn header
//                        header.classList.add("header-hidden");
//                    } else {
//                        // Cuộn lên - hiện header
//                        header.classList.remove("header-hidden");
//                    }
//                } else {
//                    // Trở lại bình thường khi ở đầu trang
//                    header.classList.remove("header-compact", "header-hidden");
//                }

//                lastScrollTop = scrollTop;

//                // Cập nhật padding cho main-content sau khi thay đổi header
//                adjustMainContentPadding();

//                isScrolling = false;
//            });
//        }
//    }

//    // Xử lý nút menu trên mobile
//    if (menuToggle && mobileNav) {
//        menuToggle.addEventListener("click", (e) => {
//            e.stopPropagation();
//            menuToggle.classList.toggle("active");
//            mobileNav.classList.toggle("active");
//        });
//    }

//    // Xử lý dropdown tài khoản
//    if (accountTrigger && accountDropdown) {
//        accountTrigger.addEventListener("click", (e) => {
//            e.stopPropagation();
//            accountDropdown.classList.toggle("open");
//        });
//    }

//    // Xử lý click bên ngoài để đóng các dropdown
//    document.addEventListener("click", (e) => {
//        // Đóng mobile nav khi click bên ngoài
//        if (mobileNav && mobileNav.classList.contains("active") &&
//            !e.target.closest("#menuToggle") && !e.target.closest("#mobileNav")) {
//            menuToggle.classList.remove("active");
//            mobileNav.classList.remove("active");
//        }

//        // Đóng account dropdown khi click bên ngoài
//        if (accountDropdown && accountDropdown.classList.contains("open") &&
//            !accountDropdown.contains(e.target)) {
//            accountDropdown.classList.remove("open");
//        }
//    });

//    // Thêm nút cuộn lên đầu trang
//    function addScrollToTopButton() {
//        // Kiểm tra xem nút đã tồn tại chưa
//        if (document.querySelector(".scroll-to-top")) return;

//        // Tạo nút
//        const scrollButton = document.createElement("button");
//        scrollButton.className = "scroll-to-top";
//        scrollButton.innerHTML = '<i class="fas fa-arrow-up"></i>';
//        scrollButton.setAttribute("aria-label", "Cuộn lên đầu trang");
//        document.body.appendChild(scrollButton);

//        // Xử lý hiển thị/ẩn nút
//        function toggleScrollButtonVisibility() {
//            if (window.pageYOffset > 300) {
//                scrollButton.classList.add("visible");
//            } else {
//                scrollButton.classList.remove("visible");
//            }
//        }

//        window.addEventListener("scroll", toggleScrollButtonVisibility);

//        // Xử lý sự kiện click
//        scrollButton.addEventListener("click", () => {
//            window.scrollTo({
//                top: 0,
//                behavior: "smooth",
//            });
//        });
//    }

//    // Thêm class active cho link hiện tại
//    function setActiveNavLinks() {
//        const currentLocation = window.location.pathname;
//        const navLinks = document.querySelectorAll('.nav-links a');
//        const mobileLinks = document.querySelectorAll('.mobile-links a');

//        function setActive(links) {
//            links.forEach(link => {
//                const href = link.getAttribute('href');
//                if (href === currentLocation || (href !== '/' && currentLocation.includes(href))) {
//                    link.classList.add('active');
//                }
//            });
//        }

//        setActive(navLinks);
//        setActive(mobileLinks);
//    }

//    // Xử lý dropdown trên mobile
//    function setupMobileDropdowns() {
//        const mobileDropdownToggles = document.querySelectorAll('.mobile-dropdown-toggle');
//        mobileDropdownToggles.forEach(toggle => {
//            toggle.addEventListener('click', function (e) {
//                e.preventDefault();
//                e.stopPropagation();
//                const parent = this.closest('.mobile-dropdown');
//                parent.classList.toggle('open');
//            });
//        });
//    }

//    // Xử lý hover container (báo giá)
//    function setupHoverContainers() {
//        const hoverContainers = document.querySelectorAll(".bao-gia-hover-container");
//        const isMobile = () => window.matchMedia("(max-width: 767px)").matches;

//        hoverContainers.forEach((container) => {
//            const title = container.querySelector(".hover-title");
//            const list = container.querySelector(".bao-gia-list");

//            if (!title || !list) return;

//            if (!title.hasAttribute("tabindex")) {
//                title.setAttribute("tabindex", "0");
//                title.setAttribute("role", "button");
//                title.setAttribute("aria-expanded", "false");
//                title.setAttribute("aria-haspopup", "true");
//            }

//            const toggleList = (show) => {
//                if (show === undefined) {
//                    list.classList.toggle("show");
//                    const isExpanded = list.classList.contains("show");
//                    title.setAttribute("aria-expanded", isExpanded.toString());

//                    // Toggle icon rotation
//                    const icon = title.querySelector("i");
//                    if (icon) {
//                        icon.style.transform = isExpanded ? "rotate(-180deg)" : "rotate(0)";
//                    }
//                } else {
//                    if (show) {
//                        list.classList.add("show");
//                        title.setAttribute("aria-expanded", "true");
//                        const icon = title.querySelector("i");
//                        if (icon) icon.style.transform = "rotate(-180deg)";
//                    } else {
//                        list.classList.remove("show");
//                        title.setAttribute("aria-expanded", "false");
//                        const icon = title.querySelector("i");
//                        if (icon) icon.style.transform = "rotate(0)";
//                    }
//                }
//            };

//            title.addEventListener("click", (e) => {
//                if (isMobile()) {
//                    e.preventDefault();
//                    toggleList();
//                }
//            });

//            title.addEventListener("keydown", (e) => {
//                if (e.key === "Enter" || e.key === " ") {
//                    e.preventDefault();
//                    toggleList();
//                } else if (e.key === "Escape" && list.classList.contains("show")) {
//                    toggleList(false);
//                }
//            });

//            if (!isMobile()) {
//                container.addEventListener("mouseenter", () => {
//                    if (!isMobile()) toggleList(true);
//                });

//                container.addEventListener("mouseleave", () => {
//                    if (!isMobile()) toggleList(false);
//                });
//            }

//            document.addEventListener("click", (e) => {
//                if (!container.contains(e.target) && list.classList.contains("show")) {
//                    toggleList(false);
//                }
//            });

//            // Add hover effect for the items in the list
//            const items = container.querySelectorAll('.mega-menu-list li a');
//            items.forEach(item => {
//                item.addEventListener('mouseenter', () => {
//                    item.classList.add('hover');
//                });

//                item.addEventListener('mouseleave', () => {
//                    item.classList.remove('hover');
//                });
//            });
//        });
//    }

//    // Đặt năm hiện tại cho footer
//    function setCurrentYear() {
//        const currentYear = document.getElementById('currentYear');
//        if (currentYear) {
//            currentYear.textContent = new Date().getFullYear();
//        }
//    }

//    // Khởi tạo tất cả các chức năng
//    adjustMainContentPadding();
//    setActiveNavLinks();
//    setupMobileDropdowns();
//    setupHoverContainers();
//    setCurrentYear();
//    addScrollToTopButton();

//    // Thêm các event listener
//    window.addEventListener("resize", adjustMainContentPadding);
//    window.addEventListener("scroll", handleScroll);
//});

// Testimonials Slideshow
document.addEventListener('DOMContentLoaded', function () {
    const slides = document.querySelectorAll('.testimonial-card');
    const dotsContainer = document.querySelector('.slider-dots');
    let currentSlide = 0;

    // Create dots
    slides.forEach((_, index) => {
        const dot = document.createElement('div');
        dot.classList.add('slider-dot');
        if (index === 0) dot.classList.add('active');
        dot.addEventListener('click', () => goToSlide(index));
        dotsContainer.appendChild(dot);
    });

    // Update dots
    function updateDots() {
        document.querySelectorAll('.slider-dot').forEach((dot, index) => {
            dot.classList.toggle('active', index === currentSlide);
        });
    }

    // Go to specific slide
    function goToSlide(index) {
        slides[currentSlide].classList.remove('active');
        currentSlide = index;
        if (currentSlide >= slides.length) currentSlide = 0;
        if (currentSlide < 0) currentSlide = slides.length - 1;
        slides[currentSlide].classList.add('active');
        updateDots();
    }

    // Next slide
    window.nextSlide = function () {
        goToSlide(currentSlide + 1);
    };

    // Previous slide
    window.prevSlide = function () {
        goToSlide(currentSlide - 1);
    };

    // Auto advance slides
    setInterval(nextSlide, 5000);
});