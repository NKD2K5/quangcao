// Optimized Product Detail JavaScript
class ProductDetailManager {
    // ...
    initThumbnailNavButtons() {
        const thumbnailsContainer = document.getElementById("thumbnailsContainer");
        const nextBtn = document.getElementById("thumbnailNext");
        const prevBtn = document.getElementById("thumbnailPrev");
        if (!thumbnailsContainer || !nextBtn || !prevBtn) return;

        const scrollStep = 80; // px, mỗi lần bấm sẽ cuộn ngang 1 thumbnail
        nextBtn.addEventListener("click", (e) => {
            e.preventDefault();
            thumbnailsContainer.scrollBy({ left: scrollStep, behavior: "smooth" });
        });
        prevBtn.addEventListener("click", (e) => {
            e.preventDefault();
            thumbnailsContainer.scrollBy({ left: -scrollStep, behavior: "smooth" });
        });
    }

    constructor() {
        this.currentImageIndex = 0
        this.totalImages = 0
        this.thumbnails = []
        this.videoState = {
            src: null,
            currentTime: 0,
            isPlaying: false,
        }

        this.init()
    }

    init() {
        this.bindEvents()
        this.initializeComponents()
    }

    bindEvents() {
        const runInitializations = () => {
            console.log("DOM Ready: Running initializations...")
            this.initTabSwitching()
            this.initImageGallery()
            this.initHeartButton()
            this.initLightbox()
            this.initShareFunctionality()
            this.initReviewSystem()
            this.initVideoThumbnails()
        }

        if (document.readyState === "loading" || document.readyState === "interactive") {
            document.addEventListener("DOMContentLoaded", runInitializations)
        } else {
            console.log("DOM Already Ready: Running initializations immediately.")
            runInitializations()
        }
    }

    // Tab Switching với cải thiện performance
    initTabSwitching() {
        const tabButtons = document.querySelectorAll(".tab-button")
        const tabPanes = document.querySelectorAll(".tab-pane")

        if (!tabButtons.length || !tabPanes.length) return

        const tabContainer = document.querySelector(".tabs-header")
        if (!tabContainer) return

        tabContainer.addEventListener("click", (e) => {
            const button = e.target.closest(".tab-button")
            if (!button) return

            e.preventDefault()

            tabButtons.forEach((btn) => btn.classList.remove("active"))
            tabPanes.forEach((pane) => pane.classList.remove("active"))

            button.classList.add("active")

            const tabId = button.getAttribute("data-tab") + "-tab"
            const targetPane = document.getElementById(tabId)
            if (targetPane) {
                targetPane.classList.add("active")
                this.handleTabContentLoad(tabId)
            }

            this.handleVideoStateOnTabSwitch(tabId)
        })
    }

    handleTabContentLoad(tabId) {
        if (tabId === "reviews-tab") {
            this.loadReviewsIfNeeded()
        }
    }

    handleVideoStateOnTabSwitch(tabId) {
        const productImagesContainer = document.querySelector(".product-images")
        if (!productImagesContainer) return

        const mainImageContainer = productImagesContainer.querySelector("#mainImageContainer")
        const mainVideo = mainImageContainer ? mainImageContainer.querySelector("video.main-image") : null

        if (tabId === "reviews-tab") {
            if (mainVideo) {
                this.videoState = {
                    src: mainVideo.src,
                    currentTime: mainVideo.currentTime,
                    isPlaying: !mainVideo.paused,
                }
                mainVideo.pause()
            }
        } else {
            productImagesContainer.style.display = ""
            if (mainVideo && this.videoState && mainVideo.src === this.videoState.src) {
                mainVideo.currentTime = this.videoState.currentTime
                if (this.videoState.isPlaying) {
                    mainVideo.play().catch((err) => console.error("Cannot play video:", err))
                }
            }
        }
    }

    // Improved Image Gallery
    initImageGallery() {
        const mainImage = document.getElementById("mainImage");
        this.thumbnails = document.querySelectorAll(".thumbnail-wrapper");
        console.log('[Gallery] initImageGallery: thumbnails found =', this.thumbnails.length);

        if (!mainImage || !this.thumbnails.length) {
            console.warn('[Gallery] Không tìm thấy mainImage hoặc thumbnails!');
            return;
        }

        this.totalImages = this.thumbnails.length;
        this.currentImageIndex = 0;

        if (this.totalImages < 2) {
            console.warn('[Gallery] Chỉ có 1 ảnh/video, không cần điều hướng!');
        }

        const thumbnailsContainer = document.getElementById("thumbnailsContainer");
        if (thumbnailsContainer) {
            thumbnailsContainer.addEventListener("click", (e) => {
                const thumbnail = e.target.closest(".thumbnail-wrapper");
                if (!thumbnail) return;

                const index = Number.parseInt(thumbnail.getAttribute("data-index"));
                if (!isNaN(index)) {
                    this.updateMainMedia(index);
                }
            });
        }

        this.initNavigationButtons();
        this.initKeyboardNavigation();
        this.initThumbnailNavButtons();
    }

    initNavigationButtons() {
        const prevButton = document.getElementById("prevImage")
        const nextButton = document.getElementById("nextImage")

        if (prevButton) {
            prevButton.addEventListener("click", () => {
                console.log('[Gallery] Prev button clicked, current index:', this.currentImageIndex);
                this.navigateImage(-1);
            });
        }

        if (nextButton) {
            nextButton.addEventListener("click", () => {
                console.log('[Gallery] Next button clicked, current index:', this.currentImageIndex);
                this.navigateImage(1);
            });
        }
    }

    initKeyboardNavigation() {
        document.addEventListener("keydown", (e) => {
            if (document.querySelector(".lightbox")?.style.display === "flex") {
                switch (e.key) {
                    case "ArrowLeft":
                        e.preventDefault()
                        this.navigateImage(-1)
                        break
                    case "ArrowRight":
                        e.preventDefault()
                        this.navigateImage(1)
                        break
                    case "Escape":
                        e.preventDefault()
                        this.closeLightbox()
                        break
                }
            }
        })
    }

    navigateImage(direction) {
        if (!this.thumbnails || this.thumbnails.length === 0) {
            console.warn('[Gallery] thumbnails empty!');
            return;
        }
        const oldIndex = this.currentImageIndex;
        this.currentImageIndex = (this.currentImageIndex + direction + this.totalImages) % this.totalImages;
        console.log(`[Gallery] navigateImage: direction=${direction}, oldIndex=${oldIndex}, newIndex=${this.currentImageIndex}, totalImages=${this.totalImages}`);
        this.updateMainMedia(this.currentImageIndex);
    }

    updateMainMedia(index) {
        if (!this.thumbnails || this.thumbnails.length === 0) {
            console.warn('[Gallery] updateMainMedia: thumbnails empty!');
            return;
        }
        if (index < 0 || index >= this.totalImages) {
            console.warn(`[Gallery] updateMainMedia: invalid index ${index}, totalImages=${this.totalImages}`);
            return;
        }
        console.log(`[Gallery] updateMainMedia: index=${index}, totalImages=${this.totalImages}`);
        this.currentImageIndex = index;
        const selectedThumbnail = this.thumbnails[index];
        if (!selectedThumbnail) {
            console.warn(`[Gallery] updateMainMedia: selectedThumbnail not found at index ${index}`);
            return;
        }

        const thumbnailVideo = selectedThumbnail.querySelector("video");
        const thumbnailImage = selectedThumbnail.querySelector("img");
        const mainImageContainer = document.getElementById("mainImageContainer");

        if (!mainImageContainer) {
            console.warn('[Gallery] updateMainMedia: mainImageContainer not found!');
            return;
        }

        // Clean up old media
        const oldMainVideo = mainImageContainer.querySelector("video.main-image");
        if (oldMainVideo) {
            oldMainVideo.pause();
            oldMainVideo.remove();
        }

        const oldMainImage = mainImageContainer.querySelector("img.main-image");
        if (oldMainImage) {
            oldMainImage.remove();
        }

        // Create new media element
        if (thumbnailVideo) {
            this.createMainVideo(thumbnailVideo, mainImageContainer);
        } else if (thumbnailImage) {
            this.createMainImage(thumbnailImage, mainImageContainer);
        }

        this.updateThumbnailActive(index);
        this.updateCounter(index);
        this.updateLightboxContent(thumbnailVideo || thumbnailImage);
    }

    createMainVideo(thumbnailVideo, container) {
        const mainVideo = document.createElement("video")
        mainVideo.src = thumbnailVideo.getAttribute("src")
        mainVideo.className = "main-image"
        mainVideo.controls = true
        mainVideo.preload = "metadata"
        mainVideo.style.cssText = "background: #000; max-width: 100%; max-height: 400px;"

        mainVideo.onerror = () => {
            console.error("Error loading video:", mainVideo.src)
            mainVideo.innerHTML = '<p style="color: white; text-align: center; padding: 20px;">Cannot load video</p>'
        }

        container.prepend(mainVideo)
    }

    createMainImage(thumbnailImage, container) {
        const mainImage = document.createElement("img")
        mainImage.src = thumbnailImage.src
        mainImage.className = "main-image"
        mainImage.alt = thumbnailImage.alt
        mainImage.loading = "lazy"

        container.prepend(mainImage)
    }

    updateThumbnailActive(index) {
        this.thumbnails.forEach((thumb, i) => {
            thumb.classList.toggle("active", i === index)
        })
    }

    updateCounter(index) {
        const currentIndexSpan = document.getElementById("currentImageIndex")
        if (currentIndexSpan) {
            currentIndexSpan.textContent = (index + 1).toString()
        }
    }

    // Heart Button with debouncing
    initHeartButton() {
        const heartBtn = document.getElementById("heartBtn")
        const heartCount = document.getElementById("heartCount")

        if (!heartBtn || !heartCount) return

        const productId = heartBtn.dataset.productId
        if (!productId) return

        let count = Number.parseInt(localStorage.getItem(`heart_${productId}`) || "0")
        heartCount.textContent = count.toString()

        if (count > 0) {
            this.setHeartActive(heartBtn, true)
        }

        let clickTimeout
        heartBtn.addEventListener("click", () => {
            if (clickTimeout) return

            clickTimeout = setTimeout(() => {
                clickTimeout = null
            }, 1000)

            count++
            heartCount.textContent = count.toString()
            localStorage.setItem(`heart_${productId}`, count.toString())

            this.setHeartActive(heartBtn, true)
            this.animateHeart(heartBtn)
            this.showToast("Cảm ơn bạn đã thích sản phẩm này!")
        })
    }

    setHeartActive(heartBtn, active) {
        const icon = heartBtn.querySelector("i")
        if (!icon) return

        heartBtn.classList.toggle("active", active)

        if (active) {
            icon.classList.remove("far")
            icon.classList.add("fas")
        } else {
            icon.classList.remove("fas")
            icon.classList.add("far")
        }
    }

    animateHeart(heartBtn) {
        const icon = heartBtn.querySelector("i")
        if (!icon) return

        icon.style.animation = "none"
        requestAnimationFrame(() => {
            icon.style.animation = "heartBeat 1s ease-in-out"
        })
    }

    // Enhanced Lightbox
    initLightbox() {
        const fullscreenButton = document.getElementById("fullscreenButton")
        const lightbox = document.getElementById("imageLightbox")

        if (!fullscreenButton || !lightbox) {
            console.warn("Không tìm thấy fullscreenButton hoặc imageLightbox khi khởi tạo!");
            return;
        }

        fullscreenButton.addEventListener("click", () => {
            console.log("[ProductDetail] Fullscreen button clicked");
            this.openLightbox();
        });

        lightbox.addEventListener("click", (e) => {
            const lightboxContent = lightbox.querySelector(".lightbox-content")

            if (e.target === lightbox) {
                this.closeLightbox()
                return
            }

            const clickedMediaElement = e.target.closest(".lightbox-media")
            if (lightboxContent && clickedMediaElement && lightboxContent.contains(clickedMediaElement)) {
                this.closeLightbox()
            }
        })

        const closeBtn = lightbox.querySelector(".lightbox-close")
        if (closeBtn) {
            closeBtn.addEventListener("click", () => this.closeLightbox())
        }
    }

    openLightbox() {
        const lightbox = document.getElementById("imageLightbox");
        if (!lightbox) {
            console.warn("Không tìm thấy imageLightbox khi mở lightbox!");
            return;
        }

        const mainVideo = document.querySelector("video.main-image");
        const mainImg = document.querySelector("img.main-image");
        const lightboxContent = document.querySelector(".lightbox-content");

        if (mainVideo) {
            console.log("[ProductDetail] Open lightbox: Hiển thị VIDEO chính");
            this.updateLightboxContent(mainVideo, true);
        } else if (mainImg) {
            console.log("[ProductDetail] Open lightbox: Hiển thị ẢNH chính");
            this.updateLightboxContent(mainImg, false);
        } else {
            console.warn("Không tìm thấy media chính để hiển thị trong lightbox!");
            if (lightboxContent) {
                lightboxContent.innerHTML = '<div style="color:red;padding:2rem">Không tìm thấy ảnh hoặc video chính!</div>';
            }
        }

        lightbox.style.display = "flex";
        document.body.style.overflow = "hidden";
    }

    closeLightbox() {
        const lightbox = document.getElementById("imageLightbox")
        if (!lightbox) return

        lightbox.style.display = "none"
        document.body.style.overflow = ""

        const lightboxVideo = lightbox.querySelector("video")
        if (lightboxVideo) {
            lightboxVideo.pause()
        }
    }

    updateLightboxContent(mediaElement, isVideo = false) {
        const lightboxContent = document.querySelector(".lightbox-content")
        if (!lightboxContent) return

        const src = mediaElement.src || mediaElement.getAttribute("src")
        if (!src) return

        lightboxContent.innerHTML = ""

        const closeButton = this.createLightboxCloseButton()
        lightboxContent.appendChild(closeButton)

        if (this.totalImages > 1) {
            const navButtons = this.createLightboxNavigation()
            lightboxContent.appendChild(navButtons.prev)
            lightboxContent.appendChild(navButtons.next)
        }

        const mediaEl = this.createLightboxMedia(src, isVideo)
        lightboxContent.appendChild(mediaEl)

        const caption = this.createLightboxCaption()
        lightboxContent.appendChild(caption)
    }

    createLightboxCloseButton() {
        const closeButton = document.createElement("span")
        closeButton.className = "lightbox-close"
        closeButton.innerHTML = "&times;"
        closeButton.onclick = () => this.closeLightbox()
        return closeButton
    }

    createLightboxNavigation() {
        const prevButton = document.createElement("button")
        prevButton.className = "lightbox-nav prev"
        prevButton.innerHTML = '<i class="fas fa-chevron-left"></i>'
        prevButton.onclick = () => this.navigateImage(-1)

        const nextButton = document.createElement("button")
        nextButton.className = "lightbox-nav next"
        nextButton.innerHTML = '<i class="fas fa-chevron-right"></i>'
        nextButton.onclick = () => this.navigateImage(1)

        return { prev: prevButton, next: nextButton }
    }

    createLightboxMedia(src, isVideo) {
        if (isVideo) {
            const video = document.createElement("video")
            video.className = "lightbox-media"
            video.src = src
            video.controls = true
            video.autoplay = true
            video.style.cssText = "max-width: 100%; max-height: 80vh;"
            return video
        } else {
            const image = document.createElement("img")
            image.className = "lightbox-media"
            image.src = src
            image.style.cssText = "max-width: 100%; max-height: 80vh;"
            return image
        }
    }

    createLightboxCaption() {
        const caption = document.createElement("div")
        caption.className = "lightbox-caption"
        caption.textContent = `${this.currentImageIndex + 1} / ${this.totalImages}`
        return caption
    }

    // Enhanced Share Functionality
    initShareFunctionality() {
        const shareButton = document.getElementById("shareButton")
        if (!shareButton) return

        shareButton.addEventListener("click", (e) => {
            e.preventDefault()
            e.stopPropagation()
            this.showShareModal()
        })
    }

    showShareModal() {
        const existingModal = document.getElementById("customShareModal")
        if (existingModal) {
            existingModal.remove()
        }

        const productUrl = window.location.href
        const productTitle = document.querySelector(".product-name")?.textContent || "Sản phẩm từ In Vĩnh Phát"
        const productImage = document.querySelector(".main-image")?.src || ""

        const modal = this.createShareModal(productUrl, productTitle, productImage)
        document.body.appendChild(modal)

        requestAnimationFrame(() => {
            modal.style.opacity = "1"
            modal.querySelector(".share-modal-content").style.transform = "scale(1)"
        })
    }

    createShareModal(productUrl, productTitle, productImage) {
        const modal = document.createElement("div")
        modal.id = "customShareModal"
        modal.style.cssText = `
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background-color: rgba(0,0,0,0.7); display: flex;
            justify-content: center; align-items: center; z-index: 9999;
            opacity: 0; transition: opacity 0.3s ease;
        `

        const content = document.createElement("div")
        content.className = "share-modal-content"
        content.style.cssText = `
            background: white; padding: 25px; border-radius: 10px;
            max-width: 500px; width: 90%; position: relative;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            transform: scale(0.9); transition: transform 0.3s ease;
        `

        content.innerHTML = this.getShareModalHTML(productUrl, productTitle, productImage)
        modal.appendChild(content)

        this.attachShareModalEvents(modal, productUrl)

        return modal
    }

    getShareModalHTML(productUrl, productTitle, productImage) {
        const platforms = [
            {
                name: "Facebook",
                icon: "fab fa-facebook-f",
                color: "#3b5998",
                url: `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(productUrl)}`,
            },
            {
                name: "Twitter",
                icon: "fab fa-twitter",
                color: "#1da1f2",
                url: `https://twitter.com/intent/tweet?url=${encodeURIComponent(productUrl)}&text=${encodeURIComponent(productTitle)}`,
            },
            {
                name: "LinkedIn",
                icon: "fab fa-linkedin-in",
                color: "#0077b5",
                url: `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(productUrl)}`,
            },
            {
                name: "Pinterest",
                icon: "fab fa-pinterest-p",
                color: "#bd081c",
                url: `https://pinterest.com/pin/create/button/?url=${encodeURIComponent(productUrl)}&media=${encodeURIComponent(productImage)}&description=${encodeURIComponent(productTitle)}`,
            },
            {
                name: "Email",
                icon: "fas fa-envelope",
                color: "#ea4335",
                url: `mailto:?subject=${encodeURIComponent(productTitle)}&body=${encodeURIComponent("Xem sản phẩm này tại: " + productUrl)}`,
            },
            {
                name: "WhatsApp",
                icon: "fab fa-whatsapp",
                color: "#25d366",
                url: `https://wa.me/?text=${encodeURIComponent(productTitle + " - " + productUrl)}`,
            },
        ]

        const platformButtons = platforms
            .map(
                (platform) => `
            <a href="${platform.url}" target="_blank" class="share-option" 
               style="background-color: ${platform.color}; color: white; text-decoration: none;
                      padding: 15px 10px; border-radius: 8px; display: flex; flex-direction: column;
                      align-items: center; justify-content: center; text-align: center;
                      transition: transform 0.2s, opacity 0.2s;"
               onmouseover="this.style.transform='translateY(-3px)'; this.style.opacity='0.9';"
               onmouseout="this.style.transform='translateY(0)'; this.style.opacity='1';">
                <i class="${platform.icon}" style="font-size: 24px; margin-bottom: 8px;"></i>
                <span style="font-size: 12px;">${platform.name}</span>
            </a>
        `,
            )
            .join("")

        return `
            <span style="position: absolute; top: 15px; right: 15px; font-size: 24px; 
                         cursor: pointer; color: #999;" onclick="this.closest('#customShareModal').remove()">&times;</span>
            <h3 style="margin-top: 0; margin-bottom: 20px; color: #333; font-size: 20px; text-align: center;">
                Chia sẻ sản phẩm
            </h3>
            <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; margin-bottom: 20px;">
                ${platformButtons}
            </div>
            <div style="display: flex; margin-bottom: 20px; border: 1px solid #ddd; border-radius: 6px; overflow: hidden;">
                <input type="text" value="${productUrl}" readonly 
                       style="flex: 1; padding: 12px; border: none; font-size: 14px; background-color: #f5f5f5;">
                <button onclick="this.previousElementSibling.select(); navigator.clipboard.writeText('${productUrl}').then(() => window.productDetailManager.showToast('Đã sao chép liên kết!'))"
                        style="background-color: #4caf50; color: white; border: none; padding: 0 15px; cursor: pointer; display: flex; align-items: center; font-size: 14px;">
                    <i class="fas fa-copy" style="margin-right: 5px;"></i> Sao chép
                </button>
            </div>
            <div style="text-align: center;">
                <img src="https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${encodeURIComponent(productUrl)}" 
                     alt="QR Code" style="border: 1px solid #ddd; border-radius: 8px;">
                <p style="margin-top: 10px; color: #666; font-size: 14px;">Quét mã QR để xem trên điện thoại</p>
            </div>
        `
    }

    attachShareModalEvents(modal, productUrl) {
        modal.addEventListener("click", (e) => {
            if (e.target === modal) {
                modal.remove()
            }
        })

        const escHandler = (e) => {
            if (e.key === "Escape") {
                modal.remove()
                document.removeEventListener("keydown", escHandler)
            }
        }
        document.addEventListener("keydown", escHandler)
    }

    // Enhanced Review System
    initReviewSystem() {
        console.log("initReviewSystem CALLED")
        this.initReviewForm()
        this.initReviewActions()
        this.initStarRatings()
        this.initReviewMedia()
        this.initReviewPagination()
        this.initReviewMediaGalleries()
    }

    initReviewForm() {
        const reviewForm = document.querySelector(".review-form")
        if (!reviewForm) return

        reviewForm.addEventListener("submit", async (e) => {
            e.preventDefault()

            const submitBtn = reviewForm.querySelector('button[type="submit"]')
            const originalText = submitBtn.innerHTML

            try {
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang gửi...'
                submitBtn.disabled = true

                const formData = new FormData(reviewForm)

                const response = await fetch("/SanPhams/GuiDanhGia", {
                    method: "POST",
                    body: formData,
                })

                if (response.ok) {
                    this.showToast("Đánh giá của bạn đã được gửi thành công!")
                    reviewForm.reset()
                    this.resetStarRating()
                    this.clearMediaPreview()

                    setTimeout(() => window.location.reload(), 1500)
                } else {
                    let errorMessage = "Có lỗi xảy ra khi gửi đánh giá."
                    try {
                        const errorData = await response.json()
                        if (errorData && errorData.message) {
                            errorMessage = errorData.message
                        } else if (response.statusText) {
                            errorMessage = `Lỗi ${response.status}: ${response.statusText}`
                        }
                    } catch (jsonError) {
                        // Ignore if response is not JSON
                    }
                    this.showToast(errorMessage, "error")
                }
            } catch (error) {
                console.error("Error submitting review:", error)
                this.showToast("Có lỗi xảy ra. Vui lòng thử lại sau.", "error")
            } finally {
                submitBtn.innerHTML = originalText
                submitBtn.disabled = false
            }
        })
    }

    initReviewActions() {
        const reviewsContainer = document.getElementById("reviews-tab")
        if (!reviewsContainer) return

        reviewsContainer.addEventListener("click", async (e) => {
            const target = e.target.closest(".review-helpful-btn, .review-report-btn, .review-delete-btn")
            if (!target) return

            e.preventDefault()
            e.stopPropagation()

            const reviewCard = target.closest(".review-card")
            const reviewId = reviewCard?.getAttribute("data-review-id")

            if (!reviewId) return

            if (target.classList.contains("review-helpful-btn")) {
                await this.handleHelpfulReview(target, reviewId)
            } else if (target.classList.contains("review-report-btn")) {
                await this.handleReportReview(target, reviewId)
            } else if (target.classList.contains("review-delete-btn")) {
                await this.handleDeleteReview(target, reviewId, reviewCard)
            }
        })
    }

    async handleHelpfulReview(button, reviewId) {
        try {
            const response = await fetch("/SanPhams/DanhGiaHuuIch", {
                method: "POST",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: `idDanhGia=${reviewId}`,
            })

            const data = await response.json()

            if (data.success) {
                button.innerHTML = `<i class="fas fa-thumbs-up"></i> Hữu ích (${data.helpfulCount})`
                button.classList.add("active")
                button.disabled = true
                this.showToast("Cảm ơn bạn đã đánh giá!")
            } else {
                this.handleReviewActionError(data)
            }
        } catch (error) {
            console.error("Error marking review as helpful:", error)
            this.showToast("Có lỗi xảy ra. Vui lòng thử lại sau.", "error")
        }
    }

    async handleReportReview(button, reviewId) {
        if (!confirm("Bạn có chắc muốn báo cáo đánh giá này không?")) return

        try {
            const response = await fetch("/SanPhams/BaoCaoDanhGia", {
                method: "POST",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: `idDanhGia=${reviewId}`,
            })

            const data = await response.json()

            if (data.success) {
                button.innerHTML = '<i class="fas fa-flag"></i> Đã báo cáo'
                button.classList.add("active")
                button.disabled = true
                this.showToast("Báo cáo của bạn đã được gửi!")
            } else {
                this.handleReviewActionError(data)
            }
        } catch (error) {
            console.error("Error reporting review:", error)
            this.showToast("Có lỗi xảy ra. Vui lòng thử lại sau.", "error")
        }
    }

    async handleDeleteReview(button, reviewId, reviewCard) {
        if (!confirm("Bạn có chắc chắn muốn xóa bình luận này không?")) return

        const originalHTML = button.innerHTML

        try {
            button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xóa...'
            button.disabled = true

            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value

            const response = await fetch("/SanPhams/XoaDanhGia", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "X-CSRF-TOKEN": token,
                },
                body: `idDanhGia=${reviewId}`,
            })

            const data = await response.json()

            if (data.success) {
                reviewCard.style.animation = "fadeOut 0.5s"
                setTimeout(() => {
                    reviewCard.remove()
                    this.updateReviewStats(data)
                    this.showToast("Đã xóa bình luận thành công!")
                }, 500)
            } else {
                throw new Error(data.message || "Delete failed")
            }
        } catch (error) {
            console.error("Error deleting review:", error)
            button.innerHTML = originalHTML
            button.disabled = false
            this.showToast("Có lỗi xảy ra khi xóa bình luận!", "error")
        }
    }

    handleReviewActionError(data) {
        if (data.requireLogin) {
            if (confirm("Bạn cần đăng nhập để thực hiện hành động này. Đăng nhập ngay?")) {
                window.location.href = `/Account/Login?returnUrl=${encodeURIComponent(window.location.pathname)}`
            }
        } else {
            this.showToast(data.message || "Có lỗi xảy ra.", "error")
        }
    }

    updateReviewStats(data) {
        const totalReviewsElement = document.querySelector(".total-reviews span")
        if (totalReviewsElement && data.remainingCount !== undefined) {
            totalReviewsElement.textContent = `${data.remainingCount} đánh giá`
        }

        const avgRatingElement = document.querySelector(".average-rating .rating-number")
        if (avgRatingElement && data.averageRating !== undefined) {
            avgRatingElement.textContent = data.averageRating.toFixed(1)
        }

        this.updateRatingStars(data.averageRating)

        if (data.remainingCount === 0) {
            const reviewsList = document.querySelector(".reviews-list")
            if (reviewsList) {
                reviewsList.innerHTML = `
                    <div class="no-reviews">
                        <i class="fas fa-comment-slash"></i>
                        <p>Chưa có đánh giá nào.</p>
                    </div>
                `
            }
        }
    }

    updateRatingStars(averageRating) {
        const ratingStars = document.querySelectorAll(".rating-overview .rating-stars .fa-star")
        if (ratingStars.length > 0 && averageRating !== undefined) {
            const avgRating = Math.round(averageRating)
            ratingStars.forEach((star, index) => {
                star.classList.toggle("filled", index < avgRating)
            })
        }
    }

    // Enhanced Star Rating System
    initStarRatings() {
        this.initReviewFormStars()
    }

    initReviewFormStars() {
        const starContainer = document.querySelector(".star-rating-input")
        if (!starContainer) return

        const starInputs = starContainer.querySelectorAll(".star-input")
        const starLabels = starContainer.querySelectorAll(".star-label")

        starLabels.forEach((label, index) => {
            label.addEventListener("mouseenter", () => {
                this.highlightStars(starLabels, index, true)
            })
        })

        starContainer.addEventListener("mouseleave", () => {
            this.resetStarsToSelected(starInputs, starLabels)
        })

        starInputs.forEach((input, index) => {
            input.addEventListener("change", () => {
                this.highlightStars(starLabels, index, false)
            })
        })
    }

    highlightStars(starLabels, upToIndex, isHover) {
        starLabels.forEach((label, i) => {
            const icon = label.querySelector("i")
            if (!icon) return

            if (i <= upToIndex) {
                icon.style.color = "#f59e0b"
                if (!isHover) {
                    icon.style.transform = "scale(1.1)"
                    setTimeout(() => {
                        icon.style.transform = "scale(1)"
                    }, 150)
                }
            } else {
                icon.style.color = "#ddd"
            }
        })
    }

    resetStarsToSelected(starInputs, starLabels) {
        const checkedInput = Array.from(starInputs).find((input) => input.checked)

        if (checkedInput) {
            const checkedIndex = Array.from(starInputs).indexOf(checkedInput)
            this.highlightStars(starLabels, checkedIndex, false)
        } else {
            starLabels.forEach((label) => {
                const icon = label.querySelector("i")
                if (icon) icon.style.color = "#ddd"
            })
        }
    }

    resetStarRating() {
        const starInputs = document.querySelectorAll(".star-input")
        const starLabels = document.querySelectorAll(".star-label")

        starInputs.forEach((input) => (input.checked = false))
        starLabels.forEach((label) => {
            const icon = label.querySelector("i")
            if (icon) icon.style.color = "#ddd"
        })
    }

    // Enhanced Media Upload
    initReviewMedia() {
        const mediaUpload = document.getElementById("mediaUpload")
        const mediaPreviewContainer = document.getElementById("mediaPreviewContainer")

        if (!mediaUpload || !mediaPreviewContainer) return

        mediaUpload.addEventListener("change", (e) => {
            this.handleMediaUpload(e.target.files, mediaPreviewContainer)
        })

        this.initDragAndDrop(mediaPreviewContainer.parentElement, mediaUpload)
    }

    handleMediaUpload(files, container) {
        container.innerHTML = ""

        const maxFiles = 5
        const maxSize = 10 * 1024 * 1024 // 10MB

        const validFiles = Array.from(files).slice(0, maxFiles)

        validFiles.forEach((file, index) => {
            if (file.size > maxSize) {
                this.showToast(`File ${file.name} quá lớn (tối đa 10MB)`, "error")
                return
            }

            const reader = new FileReader()
            reader.onload = (e) => {
                const previewItem = this.createMediaPreviewItem(file, e.target.result, index)
                container.appendChild(previewItem)
            }
            reader.readAsDataURL(file)
        })
    }

    createMediaPreviewItem(file, src, index) {
        const previewItem = document.createElement("div")
        previewItem.className = "media-preview-item"
        previewItem.style.position = "relative"

        let mediaElement
        if (file.type.startsWith("image/")) {
            mediaElement = document.createElement("img")
            mediaElement.src = src
        } else if (file.type.startsWith("video/")) {
            mediaElement = document.createElement("video")
            mediaElement.src = src
            mediaElement.controls = true
        }

        if (mediaElement) {
            mediaElement.className = "media-preview"
            mediaElement.style.cssText = "width: 100%; height: 100%; object-fit: cover;"
            previewItem.appendChild(mediaElement)
        }

        const removeBtn = document.createElement("button")
        removeBtn.className = "media-remove-btn"
        removeBtn.innerHTML = '<i class="fas fa-times"></i>'
        removeBtn.style.cssText = `
            position: absolute; top: 5px; right: 5px; width: 20px; height: 20px;
            border-radius: 50%; background-color: rgba(0,0,0,0.7); color: white;
            border: none; cursor: pointer; display: flex; align-items: center;
            justify-content: center; font-size: 10px;
        `

        removeBtn.onclick = () => {
            previewItem.remove()
            this.updateFileInput(index)
        }

        previewItem.appendChild(removeBtn)
        return previewItem
    }

    updateFileInput(removedIndex) {
        const mediaUpload = document.getElementById("mediaUpload")
        if (!mediaUpload) return

        const dt = new DataTransfer()
        Array.from(mediaUpload.files).forEach((file, index) => {
            if (index !== removedIndex) {
                dt.items.add(file)
            }
        })
        mediaUpload.files = dt.files
    }

    clearMediaPreview() {
        const mediaPreviewContainer = document.getElementById("mediaPreviewContainer")
        if (mediaPreviewContainer) {
            mediaPreviewContainer.innerHTML = ""
        }

        const mediaUpload = document.getElementById("mediaUpload")
        if (mediaUpload) {
            mediaUpload.value = ""
        }
    }

    initDragAndDrop(dropZone, fileInput) {
        ;["dragenter", "dragover", "dragleave", "drop"].forEach((eventName) => {
            dropZone.addEventListener(eventName, (e) => {
                e.preventDefault()
                e.stopPropagation()
            })
        })
            ;["dragenter", "dragover"].forEach((eventName) => {
                dropZone.addEventListener(eventName, () => {
                    dropZone.classList.add("drag-over")
                })
            })
            ;["dragleave", "drop"].forEach((eventName) => {
                dropZone.addEventListener(eventName, () => {
                    dropZone.classList.remove("drag-over")
                })
            })

        dropZone.addEventListener("drop", (e) => {
            const files = e.dataTransfer.files
            fileInput.files = files
            fileInput.dispatchEvent(new Event("change"))
        })
    }

    // Enhanced Video Thumbnails
    initVideoThumbnails() {
        const videoThumbnails = document.querySelectorAll(".video-thumbnail")

        videoThumbnails.forEach((thumbnail) => {
            const video = thumbnail.querySelector("video")
            if (!video) return

            let hoverTimeout

            thumbnail.addEventListener("mouseenter", () => {
                clearTimeout(hoverTimeout)
                hoverTimeout = setTimeout(() => {
                    if (video.paused) {
                        video.currentTime = 0
                        video.play().catch((err) => console.error("Cannot play video:", err))
                    }
                }, 200)
            })

            thumbnail.addEventListener("mouseleave", () => {
                clearTimeout(hoverTimeout)
                if (!video.paused) {
                    video.pause()
                    video.currentTime = 0
                }
            })

            thumbnail.addEventListener("click", () => {
                const wrapper = thumbnail.closest(".thumbnail-wrapper")
                if (wrapper) {
                    const index = Number.parseInt(wrapper.getAttribute("data-index"))
                    if (!isNaN(index)) {
                        this.updateMainMedia(index)
                    }
                }
            })
        })
    }

    // Enhanced Pagination
    initReviewPagination() {
        const paginationContainer = document.querySelector(".reviews-pagination")
        if (!paginationContainer) return

        paginationContainer.addEventListener("click", async (e) => {
            const link = e.target.closest("a")
            if (!link || link.classList.contains("disabled")) return

            e.preventDefault()

            const url = link.getAttribute("href")
            await this.loadReviewPage(url)
        })
    }

    async loadReviewPage(url) {
        try {
            const response = await fetch(url)
            const html = await response.text()

            const parser = new DOMParser()
            const doc = parser.parseFromString(html, "text/html")

            const newReviewsList = doc.querySelector(".reviews-list")
            const currentReviewsList = document.querySelector(".reviews-list")
            if (newReviewsList && currentReviewsList) {
                currentReviewsList.innerHTML = newReviewsList.innerHTML
            }

            const newPagination = doc.querySelector(".reviews-pagination")
            const currentPagination = document.querySelector(".reviews-pagination")
            if (newPagination && currentPagination) {
                currentPagination.innerHTML = newPagination.innerHTML
            }

            history.pushState(null, "", url)

            document.getElementById("reviews-tab")?.scrollIntoView({
                behavior: "smooth",
                block: "start",
            })
        } catch (error) {
            console.error("Error loading review page:", error)
            this.showToast("Có lỗi xảy ra khi tải trang. Vui lòng thử lại sau.", "error")
        }
    }

    loadReviewsIfNeeded() {
        const reviewsList = document.querySelector(".reviews-list")
        if (reviewsList && !reviewsList.hasAttribute("data-loaded")) {
            reviewsList.setAttribute("data-loaded", "true")
            console.log("loadReviewsIfNeeded: Attempting to load reviews...")

            const productId = reviewsList.dataset.productId
            if (!productId) {
                console.error("Product ID not found for loading reviews.")
                return
            }

            // You would implement your actual AJAX logic here
            // This is just a placeholder
            console.log("Would load reviews for product:", productId)
        }
    }

    // Method to initialize review media galleries
    initReviewMediaGalleries(parentElement = document) {
        console.log("initReviewMediaGalleries CALLED")
        const galleries = parentElement.querySelectorAll(".review-media-gallery")
        galleries.forEach((gallery) => {
            const thumbnailsContainer = gallery.querySelector(".review-media-thumbnails")
            const displayArea = gallery.querySelector(".review-media-display-area")

            if (!thumbnailsContainer || !displayArea) return

            thumbnailsContainer.addEventListener("click", (event) => {
                console.log("Thumbnail CLICKED in gallery:", gallery)
                const thumbnail = event.target.closest(".review-thumbnail-item")
                if (!thumbnail) return

                const isActive = thumbnail.classList.contains("active")
                const isDisplayVisible = window.getComputedStyle(displayArea).display === "block"

                if (isActive && isDisplayVisible) {
                    displayArea.style.display = "none"
                    thumbnail.classList.remove("active")
                    displayArea.innerHTML = ""
                } else {
                    thumbnailsContainer.querySelectorAll(".review-thumbnail-item").forEach((t) => t.classList.remove("active"))

                    thumbnail.classList.add("active")

                    const src = thumbnail.dataset.src
                    const type = thumbnail.dataset.type

                    displayArea.innerHTML = ""

                    if (type === "video") {
                        const videoElement = document.createElement("video")
                        videoElement.src = src
                        videoElement.controls = true
                        videoElement.preload = "metadata"
                        videoElement.classList.add("review-displayed-media")
                        displayArea.appendChild(videoElement)
                    } else {
                        const imgElement = document.createElement("img")
                        imgElement.src = src
                        imgElement.alt = "Hình ảnh đánh giá"
                        imgElement.loading = "lazy"
                        imgElement.classList.add("review-displayed-media")
                        displayArea.appendChild(imgElement)
                    }
                    displayArea.style.display = "block"
                }
            })
        })
    }

    // Utility Methods
    initializeComponents() {
        this.initTooltips()
        this.initLazyLoading()
    }

    initTooltips() {
        const tooltipElements = document.querySelectorAll("[data-tooltip]")
        tooltipElements.forEach((element) => {
            element.addEventListener("mouseenter", (e) => {
                this.showTooltip(e.target, e.target.getAttribute("data-tooltip"))
            })

            element.addEventListener("mouseleave", () => {
                this.hideTooltip()
            })
        })
    }

    showTooltip(element, text) {
        const tooltip = document.createElement("div")
        tooltip.className = "custom-tooltip"
        tooltip.textContent = text
        tooltip.style.cssText = `
            position: absolute; background: #333; color: white; padding: 5px 10px;
            border-radius: 4px; font-size: 12px; z-index: 10000; pointer-events: none;
            white-space: nowrap;
        `

        document.body.appendChild(tooltip)

        const rect = element.getBoundingClientRect()
        tooltip.style.left = rect.left + rect.width / 2 - tooltip.offsetWidth / 2 + "px"
        tooltip.style.top = rect.top - tooltip.offsetHeight - 5 + "px"
    }

    hideTooltip() {
        const tooltip = document.querySelector(".custom-tooltip")
        if (tooltip) {
            tooltip.remove()
        }
    }

    initLazyLoading() {
        const images = document.querySelectorAll("img[data-src]")

        if ("IntersectionObserver" in window) {
            const imageObserver = new IntersectionObserver((entries) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        const img = entry.target
                        img.src = img.dataset.src
                        img.removeAttribute("data-src")
                        imageObserver.unobserve(img)
                    }
                })
            })

            images.forEach((img) => imageObserver.observe(img))
        } else {
            images.forEach((img) => {
                img.src = img.dataset.src
                img.removeAttribute("data-src")
            })
        }
    }

    // Enhanced Toast Notifications
    showToast(message, type = "success") {
        document.querySelectorAll(".toast-notification").forEach((toast) => toast.remove())

        const toast = document.createElement("div")
        toast.className = "toast-notification"

        const iconClass = type === "success" ? "fas fa-check-circle" : "fas fa-exclamation-circle"
        const bgColor = type === "success" ? "#10b981" : "#ef4444"

        toast.innerHTML = `
            <div class="toast-content" style="display: flex; align-items: center; gap: 10px;">
                <i class="${iconClass}" style="color: ${bgColor}; font-size: 18px;"></i>
                <span class="toast-message">${message}</span>
            </div>
            <button class="toast-close" style="background: none; border: none; color: #6b7280; cursor: pointer; font-size: 16px;">&times;</button>
        `

        toast.style.cssText = `
            position: fixed; bottom: 20px; right: 20px; background: white;
            border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            padding: 15px; display: flex; align-items: center; justify-content: space-between;
            gap: 10px; z-index: 10000; transform: translateY(100px); opacity: 0;
            transition: all 0.3s ease; max-width: 400px; border-left: 4px solid ${bgColor};
        `

        document.body.appendChild(toast)

        requestAnimationFrame(() => {
            toast.style.transform = "translateY(0)"
            toast.style.opacity = "1"
        })

        const autoHideTimeout = setTimeout(() => {
            this.hideToast(toast)
        }, 4000)

        const closeBtn = toast.querySelector(".toast-close")
        closeBtn.addEventListener("click", () => {
            clearTimeout(autoHideTimeout)
            this.hideToast(toast)
        })

        toast.addEventListener("click", () => {
            clearTimeout(autoHideTimeout)
            this.hideToast(toast)
        })
    }

    hideToast(toast) {
        toast.style.transform = "translateY(100px)"
        toast.style.opacity = "0"
        setTimeout(() => {
            if (toast.parentNode) {
                toast.remove()
            }
        }, 300)
    }

    // Error Handling
    handleError(error, context = "") {
        console.error(`Error in ${context}:`, error)

        if (window.errorReporting) {
            window.errorReporting.captureException(error, { context })
        }

        this.showToast("Có lỗi xảy ra. Vui lòng thử lại sau.", "error")
    }
}

// Initialize the product detail manager
window.productDetailManager = new ProductDetailManager()

// Global functions for backward compatibility
window.showToast = (message, type) => window.productDetailManager.showToast(message, type)
window.quickView = (idSanPham) => (window.location.href = `/SanPhams/Details/${idSanPham}`)
window.contactUs = (idSanPham, tenSanPham) => {
    window.location.href = `/LienHe?idSanPham=${idSanPham}&tenSanPham=${tenSanPham}`
}

// Service Worker registration for better performance (optional)
if ("serviceWorker" in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker
            .register("/sw.js")
            .then((registration) => console.log("SW registered"))
            .catch((error) => console.log("SW registration failed"))
    })
}
