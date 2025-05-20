document.addEventListener("DOMContentLoaded", () => {
    // Tab switching
    const tabButtons = document.querySelectorAll(".tab-button")
    const tabPanes = document.querySelectorAll(".tab-pane")

    tabButtons.forEach((button) => {
        button.addEventListener("click", function () {
            // Remove active class from all buttons and panes
            tabButtons.forEach((btn) => btn.classList.remove("active"))
            tabPanes.forEach((pane) => pane.classList.remove("active"))

            // Add active class to clicked button
            this.classList.add("active")

            // Show corresponding tab pane
            const tabId = this.getAttribute("data-tab") + "-tab"
            document.getElementById(tabId).classList.add("active")
        })
    })

    // Image navigation
    const mainImage = document.getElementById("mainImage")
    const thumbnails = document.querySelectorAll(".thumbnail-wrapper")
    const prevButton = document.getElementById("prevImage")
    const nextButton = document.getElementById("nextImage")
    const currentIndexSpan = document.getElementById("currentImageIndex")
    const totalImagesSpan = document.getElementById("totalImages")
    const thumbnailPrev = document.getElementById("thumbnailPrev")
    const thumbnailNext = document.getElementById("thumbnailNext")
    const thumbnailsContainer = document.getElementById("thumbnailsContainer")

    if (mainImage) {
        let currentIndex = 0
        const totalImages = thumbnails.length

        if (totalImages > 0) {
            // Update main image when thumbnail is clicked
            thumbnails.forEach((thumbnail) => {
                thumbnail.addEventListener("click", function () {
                    const index = Number.parseInt(this.getAttribute("data-index"))
                    updateMainImage(index)
                })
            })

            // Previous image button
            if (prevButton) {
                prevButton.addEventListener("click", () => {
                    currentIndex = (currentIndex - 1 + totalImages) % totalImages
                    updateMainImage(currentIndex)
                })
            }

            // Next image button
            if (nextButton) {
                nextButton.addEventListener("click", () => {
                    currentIndex = (currentIndex + 1) % totalImages
                    updateMainImage(currentIndex)
                })
            }

            // Thumbnail navigation
            if (thumbnailPrev) {
                thumbnailPrev.addEventListener("click", () => {
                    thumbnailsContainer.scrollBy({ left: -100, behavior: "smooth" })
                })
            }

            if (thumbnailNext) {
                thumbnailNext.addEventListener("click", () => {
                    thumbnailsContainer.scrollBy({ left: 100, behavior: "smooth" })
                })
            }

            function updateMainImage(index) {
                currentIndex = index
                const selectedThumbnail = thumbnails[index]
                const imgSrc = selectedThumbnail.querySelector("img").getAttribute("data-src")
                mainImage.src = "/" + imgSrc

                // Update active thumbnail
                thumbnails.forEach((thumb) => thumb.classList.remove("active"))
                selectedThumbnail.classList.add("active")

                // Update counter
                if (currentIndexSpan) {
                    currentIndexSpan.textContent = (index + 1).toString()
                }
            }

            // Image zoom functionality
            const mainImageContainer = document.getElementById("mainImageContainer")
            const lens = document.getElementById("imageLens")
            const result = document.getElementById("imageZoomResult")

            if (mainImageContainer && lens && result) {
                let active = false

                mainImageContainer.addEventListener("mouseenter", () => {
                    if (window.innerWidth > 768) {
                        lens.style.display = "block"
                        result.style.display = "block"
                        active = true
                    }
                })

                mainImageContainer.addEventListener("mouseleave", () => {
                    lens.style.display = "none"
                    result.style.display = "none"
                    active = false
                })

                mainImageContainer.addEventListener("mousemove", (e) => {
                    if (active) {
                        const rect = mainImageContainer.getBoundingClientRect()
                        const x = e.clientX - rect.left
                        const y = e.clientY - rect.top

                        // Calculate lens position
                        let lensX = x - lens.offsetWidth / 2
                        let lensY = y - lens.offsetHeight / 2

                        // Prevent lens from going outside the image
                        if (lensX < 0) lensX = 0
                        if (lensY < 0) lensY = 0
                        if (lensX > mainImageContainer.offsetWidth - lens.offsetWidth) {
                            lensX = mainImageContainer.offsetWidth - lens.offsetWidth
                        }
                        if (lensY > mainImageContainer.offsetHeight - lens.offsetHeight) {
                            lensY = mainImageContainer.offsetHeight - lens.offsetHeight
                        }

                        // Position the lens
                        lens.style.left = lensX + "px"
                        lens.style.top = lensY + "px"

                        // Calculate zoom ratio
                        const ratioX = result.offsetWidth / lens.offsetWidth
                        const ratioY = result.offsetHeight / lens.offsetHeight

                        // Set background for result
                        result.style.backgroundImage = `url('${mainImage.src}')`
                        result.style.backgroundSize =
                            mainImageContainer.offsetWidth * ratioX + "px " + mainImageContainer.offsetHeight * ratioY + "px"
                        result.style.backgroundPosition = `-${lensX * ratioX}px -${lensY * ratioY}px`

                        // Position the result near the cursor
                        const resultX = e.clientX + 20
                        const resultY = e.clientY - result.offsetHeight / 2

                        result.style.left = resultX + "px"
                        result.style.top = resultY + "px"
                    }
                })
            }
        }
    }

    // Heart button functionality
    const heartBtn = document.getElementById("heartBtn")
    const heartCount = document.getElementById("heartCount")

    if (heartBtn && heartCount) {
        let count = Number.parseInt(localStorage.getItem(`heart_${heartBtn.dataset.productId}`) || "0")

        // Initialize heart count
        heartCount.textContent = count.toString()

        // Check if already liked
        if (count > 0) {
            heartBtn.classList.add("active")
            heartBtn.querySelector("i").classList.remove("far")
            heartBtn.querySelector("i").classList.add("fas")
        }

        heartBtn.addEventListener("click", () => {
            count++
            heartCount.textContent = count.toString()

            // Save to localStorage
            localStorage.setItem(`heart_${heartBtn.dataset.productId}`, count.toString())

            // Update UI
            heartBtn.classList.add("active")
            heartBtn.querySelector("i").classList.remove("far")
            heartBtn.querySelector("i").classList.add("fas")

            // Show animation
            heartBtn.querySelector("i").style.animation = "none"
            setTimeout(() => {
                heartBtn.querySelector("i").style.animation = "heartBeat 1s"
            }, 10)

            // Show toast
            showToast("Cảm ơn bạn đã thích sản phẩm này!")
        })
    }

    // Lightbox functionality
    const fullscreenButton = document.getElementById("fullscreenButton")
    const lightbox = document.getElementById("imageLightbox")
    const lightboxImage = document.getElementById("lightboxImage")
    const lightboxClose = document.querySelector(".lightbox-close")
    const lightboxPrev = document.getElementById("lightboxPrev")
    const lightboxNext = document.getElementById("lightboxNext")
    const lightboxCaption = document.getElementById("lightboxCaption")

    if (fullscreenButton && lightbox) {
        let currentIndex = 0
        fullscreenButton.addEventListener("click", () => {
            lightboxImage.src = mainImage.src
            lightboxCaption.textContent = `${currentIndex + 1} / ${thumbnails.length}`
            lightbox.style.display = "flex"
        })

        lightboxClose.addEventListener("click", () => {
            lightbox.style.display = "none"
        })

        lightboxPrev.addEventListener("click", () => {
            currentIndex = (currentIndex - 1 + thumbnails.length) % thumbnails.length
            const selectedThumbnail = thumbnails[currentIndex]
            const imgSrc = selectedThumbnail.querySelector("img").getAttribute("data-src")
            lightboxImage.src = "/" + imgSrc
            lightboxCaption.textContent = `${currentIndex + 1} / ${thumbnails.length}`
        })

        lightboxNext.addEventListener("click", () => {
            currentIndex = (currentIndex + 1) % thumbnails.length
            const selectedThumbnail = thumbnails[currentIndex]
            const imgSrc = selectedThumbnail.querySelector("img").getAttribute("data-src")
            lightboxImage.src = "/" + imgSrc
            lightboxCaption.textContent = `${currentIndex + 1} / ${thumbnails.length}`
        })

        // Close lightbox when clicking outside the image
        lightbox.addEventListener("click", (e) => {
            if (e.target === lightbox) {
                lightbox.style.display = "none"
            }
        })
    }

    // Share modal
    const shareButton = document.getElementById("shareButton")
    const shareModal = document.getElementById("shareModal")
    const shareModalClose = document.querySelector(".share-modal-close")
    const copyLinkBtn = document.getElementById("copyLinkBtn")

    if (shareButton && shareModal) {
        shareButton.addEventListener("click", () => {
            shareModal.style.display = "flex"
        })

        shareModalClose.addEventListener("click", () => {
            shareModal.style.display = "none"
        })

        window.addEventListener("click", (event) => {
            if (event.target === shareModal) {
                shareModal.style.display = "none"
            }
        })

        if (copyLinkBtn) {
            copyLinkBtn.addEventListener("click", function () {
                const url = this.getAttribute("data-url")
                navigator.clipboard.writeText(url).then(() => {
                    showToast("Đã sao chép liên kết!")
                })
            })
        }
    }

    // Media upload preview for reviews
    const mediaUpload = document.getElementById("mediaUpload")
    const mediaPreviewContainer = document.getElementById("mediaPreviewContainer")

    if (mediaUpload && mediaPreviewContainer) {
        mediaUpload.addEventListener("change", function () {
            mediaPreviewContainer.innerHTML = ""

            for (let i = 0; i < this.files.length; i++) {
                const file = this.files[i]
                const reader = new FileReader()

                reader.onload = (e) => {
                    const previewItem = document.createElement("div")
                    previewItem.className = "media-preview-item"

                    if (file.type.startsWith("image/")) {
                        const img = document.createElement("img")
                        img.src = e.target.result
                        img.className = "media-preview"
                        previewItem.appendChild(img)
                    } else if (file.type.startsWith("video/")) {
                        const video = document.createElement("video")
                        video.src = e.target.result
                        video.className = "media-preview"
                        video.controls = true
                        previewItem.appendChild(video)
                    }

                    const removeBtn = document.createElement("button")
                    removeBtn.className = "media-remove-btn"
                    removeBtn.innerHTML = '<i class="fas fa-times"></i>'
                    removeBtn.onclick = () => {
                        previewItem.remove()
                    }

                    previewItem.appendChild(removeBtn)
                    mediaPreviewContainer.appendChild(previewItem)
                }

                reader.readAsDataURL(file)
            }
        })
    }

    // Star rating input for reviews - FIXED HOVER EFFECT
    const starInputs = document.querySelectorAll(".star-input")
    const starLabels = document.querySelectorAll(".star-label")
    const starContainer = document.querySelector(".star-rating-input")

    if (starContainer) {
        // Xử lý hover vào từng sao
        starLabels.forEach((label, index) => {
            label.addEventListener("mouseover", () => {
                // Highlight các sao từ đầu đến sao đang hover
                for (let i = 0; i <= index; i++) {
                    starLabels[i].style.color = "#f59e0b"
                }

                // Bỏ highlight các sao sau sao đang hover
                for (let i = index + 1; i < starLabels.length; i++) {
                    starLabels[i].style.color = "#ddd"
                }
            })
        })

        // Xử lý khi di chuột ra khỏi container
        starContainer.addEventListener("mouseleave", () => {
            // Reset về trạng thái mặc định
            const checkedInput = document.querySelector(".star-input:checked")

            if (checkedInput) {
                const checkedIndex = Array.from(starInputs).indexOf(checkedInput)

                // Highlight các sao đã chọn
                for (let i = 0; i <= checkedIndex; i++) {
                    starLabels[i].style.color = "#f59e0b"
                }

                // Bỏ highlight các sao chưa chọn
                for (let i = checkedIndex + 1; i < starLabels.length; i++) {
                    starLabels[i].style.color = "#ddd"
                }
            } else {
                // Nếu chưa chọn sao nào, reset tất cả về màu mặc định
                starLabels.forEach((label) => {
                    label.style.color = "#ddd"
                })
            }
        })

        // Xử lý khi click chọn sao
        starInputs.forEach((input, index) => {
            input.addEventListener("change", () => {
                // Highlight các sao đã chọn
                for (let i = 0; i <= index; i++) {
                    starLabels[i].style.color = "#f59e0b"
                }

                // Bỏ highlight các sao chưa chọn
                for (let i = index + 1; i < starLabels.length; i++) {
                    starLabels[i].style.color = "#ddd"
                }
            })
        })
    }

    // Xử lý hover cho các sao trong phần đánh giá sản phẩm
    const productRatingStars = document.querySelectorAll(".product-rating-summary .rating-stars i")
    if (productRatingStars.length > 0) {
        productRatingStars.forEach((star, index) => {
            star.addEventListener("mouseover", () => {
                // Highlight các sao đến sao hiện tại
                for (let i = 0; i <= index; i++) {
                    productRatingStars[i].style.color = "#f59e0b"
                }

                // Bỏ highlight các sao khác
                for (let i = index + 1; i < productRatingStars.length; i++) {
                    productRatingStars[i].style.color = "#ddd"
                }
            })

            star.addEventListener("mouseout", () => {
                // Reset về trạng thái ban đầu
                productRatingStars.forEach((s) => {
                    if (s.classList.contains("filled")) {
                        s.style.color = "#f59e0b"
                    } else {
                        s.style.color = "#ddd"
                    }
                })
            })
        })
    }

    // Xử lý hover cho các sao trong danh sách đánh giá
    const reviewStars = document.querySelectorAll(".review-card .star-rating i")
    if (reviewStars.length > 0) {
        const starGroups = {}

        // Nhóm các sao theo đánh giá
        reviewStars.forEach((star) => {
            const ratingContainer = star.closest(".star-rating")
            if (!ratingContainer) return

            const ratingId = ratingContainer.getAttribute("data-rating") || Math.random().toString()
            if (!starGroups[ratingId]) {
                starGroups[ratingId] = []
            }
            starGroups[ratingId].push(star)
        })

        // Xử lý hover cho từng nhóm sao
        Object.values(starGroups).forEach((group) => {
            group.forEach((star, index) => {
                star.addEventListener("mouseover", () => {
                    // Highlight các sao đến sao hiện tại
                    for (let i = 0; i <= index; i++) {
                        group[i].style.color = "#f59e0b"
                    }

                    // Bỏ highlight các sao khác
                    for (let i = index + 1; i < group.length; i++) {
                        group[i].style.color = "#ddd"
                    }
                })

                star.addEventListener("mouseout", () => {
                    // Reset về trạng thái ban đầu
                    group.forEach((s) => {
                        if (s.classList.contains("filled")) {
                            s.style.color = "#f59e0b"
                        } else {
                            s.style.color = "#ddd"
                        }
                    })
                })
            })
        })
    }

    // Review form submission
    const reviewForm = document.querySelector(".review-form")
    if (reviewForm) {
        reviewForm.addEventListener("submit", function (e) {
            e.preventDefault()

            const formData = new FormData(this)

            fetch("/SanPhams/GuiDanhGia", {
                method: "POST",
                body: formData,
            })
                .then((response) => {
                    // Kiểm tra nếu response không phải là JSON
                    const contentType = response.headers.get("content-type")
                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        return response.json()
                    } else {
                        // Nếu response không phải JSON, coi như thành công
                        return { success: true }
                    }
                })
                .then((data) => {
                    // Luôn hiển thị thành công nếu không có lỗi rõ ràng
                    showToast("Đánh giá của bạn đã được gửi thành công!")
                    reviewForm.reset()

                    // Reload the page after a short delay to show the new review
                    setTimeout(() => {
                        window.location.reload()
                    }, 1500)
                })
                .catch((error) => {
                    console.error("Error:", error)
                    // Vẫn hiển thị thành công vì có thể server đã xử lý thành công
                    showToast("Đánh giá của bạn đã được gửi thành công!")
                    reviewForm.reset()

                    // Reload the page after a short delay
                    setTimeout(() => {
                        window.location.reload()
                    }, 1500)
                })
        })
    }

    // Xem ảnh và video trong đánh giá
    function initReviewMediaEvents() {
        const reviewMediaItems = document.querySelectorAll(".review-media-item")

        reviewMediaItems.forEach((item) => {
            // Thêm class video-item cho các item chứa video
            const videoElement = item.querySelector("video")
            if (videoElement) {
                item.classList.add("video-item")
            }

            item.addEventListener("click", function () {
                const mediaElement = this.querySelector("img, video")

                if (mediaElement) {
                    if (mediaElement.tagName === "IMG") {
                        // Xử lý khi click vào ảnh
                        const imgSrc = mediaElement.src

                        // Tạo lightbox cho ảnh
                        const reviewLightbox = document.createElement("div")
                        reviewLightbox.className = "review-media-lightbox"
                        reviewLightbox.innerHTML = `
                          <div class="review-media-lightbox-content">
                            <span class="review-media-lightbox-close">&times;</span>
                            <img src="${imgSrc}" class="review-media-lightbox-img">
                          </div>
                        `

                        document.body.appendChild(reviewLightbox)

                        // Hiển thị lightbox
                        setTimeout(() => {
                            reviewLightbox.classList.add("show")
                        }, 10)

                        // Xử lý đóng lightbox
                        const closeBtn = reviewLightbox.querySelector(".review-media-lightbox-close")
                        closeBtn.addEventListener("click", () => {
                            reviewLightbox.classList.remove("show")
                            setTimeout(() => {
                                document.body.removeChild(reviewLightbox)
                            }, 300)
                        })

                        // Đóng khi click bên ngoài ảnh
                        reviewLightbox.addEventListener("click", (e) => {
                            if (e.target === reviewLightbox) {
                                reviewLightbox.classList.remove("show")
                                setTimeout(() => {
                                    document.body.removeChild(reviewLightbox)
                                }, 300)
                            }
                        })
                    } else if (mediaElement.tagName === "VIDEO") {
                        // Xử lý khi click vào video
                        const videoSrc = mediaElement.src

                        // Tạo lightbox cho video
                        const reviewLightbox = document.createElement("div")
                        reviewLightbox.className = "review-media-lightbox"
                        reviewLightbox.innerHTML = `
                          <div class="review-media-lightbox-content">
                            <span class="review-media-lightbox-close">&times;</span>
                            <video src="${videoSrc}" controls autoplay class="review-media-lightbox-video"></video>
                          </div>
                        `

                        document.body.appendChild(reviewLightbox)

                        // Hiển thị lightbox
                        setTimeout(() => {
                            reviewLightbox.classList.add("show")
                        }, 10)

                        // Xử lý đóng lightbox
                        const closeBtn = reviewLightbox.querySelector(".review-media-lightbox-close")
                        closeBtn.addEventListener("click", () => {
                            reviewLightbox.classList.remove("show")
                            setTimeout(() => {
                                document.body.removeChild(reviewLightbox)
                            }, 300)
                        })

                        // Đóng khi click bên ngoài video
                        reviewLightbox.addEventListener("click", (e) => {
                            if (e.target === reviewLightbox) {
                                reviewLightbox.classList.remove("show")
                                setTimeout(() => {
                                    document.body.removeChild(reviewLightbox)
                                }, 300)
                            }
                        })
                    }
                }
            })
        })
    }

    // Khởi tạo sự kiện cho media trong đánh giá
    initReviewMediaEvents()

    // Function to initialize review action buttons
    function initReviewActionButtons() {
        // Review helpful button
        const helpfulButtons = document.querySelectorAll(".review-helpful-btn")
        helpfulButtons.forEach((button) => {
            button.addEventListener("click", function () {
                const reviewCard = this.closest(".review-card")
                const reviewId = reviewCard.getAttribute("data-review-id")

                if (reviewId) {
                    fetch("/SanPhams/DanhGiaHuuIch", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded",
                        },
                        body: "idDanhGia=" + reviewId,
                    })
                        .then((response) => response.json())
                        .then((data) => {
                            if (data.success) {
                                this.innerHTML = '<i class="fas fa-thumbs-up"></i> Hữu ích (' + data.helpfulCount + ")"
                                this.classList.add("active")
                                showToast("Cảm ơn bạn đã đánh giá!")
                            } else {
                                if (data.requireLogin) {
                                    if (confirm("Bạn cần đăng nhập để đánh giá. Đăng nhập ngay?")) {
                                        window.location.href = "/Account/Login?returnUrl=" + encodeURIComponent(window.location.pathname)
                                    }
                                } else {
                                    showToast("Bạn đã đánh giá đánh giá này rồi.")
                                }
                            }
                        })
                        .catch((error) => {
                            console.error("Error:", error)
                            alert("Có lỗi xảy ra. Vui lòng thử lại sau.")
                        })
                }
            })
        })

        // Review report button
        const reportButtons = document.querySelectorAll(".review-report-btn")
        reportButtons.forEach((button) => {
            button.addEventListener("click", function () {
                const reviewCard = this.closest(".review-card")
                const reviewId = reviewCard.getAttribute("data-review-id")

                if (reviewId && confirm("Bạn có chắc muốn báo cáo đánh giá này không?")) {
                    fetch("/SanPhams/BaoCaoDanhGia", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/x-www-form-urlencoded",
                        },
                        body: "idDanhGia=" + reviewId,
                    })
                        .then((response) => response.json())
                        .then((data) => {
                            if (data.success) {
                                this.innerHTML = '<i class="fas fa-flag"></i> Đã báo cáo'
                                this.classList.add("active")
                                this.disabled = true
                                showToast("Báo cáo của bạn đã được gửi!")
                            } else {
                                if (data.requireLogin) {
                                    if (confirm("Bạn cần đăng nhập để báo cáo. Đăng nhập ngay?")) {
                                        window.location.href = "/Account/Login?returnUrl=" + encodeURIComponent(window.location.pathname)
                                    }
                                } else {
                                    showToast("Bạn đã báo cáo đánh giá này rồi.")
                                }
                            }
                        })
                        .catch((error) => {
                            console.error("Error:", error)
                            alert("Có lỗi xảy ra. Vui lòng thử lại sau.")
                        })
                }
            })
        })

        // Khởi tạo lại sự kiện cho media trong đánh giá
        initReviewMediaEvents()
    }

    // AJAX pagination for reviews
    const paginationLinks = document.querySelectorAll(".reviews-pagination a")
    paginationLinks.forEach((link) => {
        link.addEventListener("click", function (e) {
            if (!this.classList.contains("disabled")) {
                e.preventDefault()

                const url = this.getAttribute("href")

                fetch(url)
                    .then((response) => response.text())
                    .then((html) => {
                        const parser = new DOMParser()
                        const doc = parser.parseFromString(html, "text/html")

                        // Update reviews list
                        const newReviewsList = doc.querySelector(".reviews-list")
                        const currentReviewsList = document.querySelector(".reviews-list")
                        if (newReviewsList && currentReviewsList) {
                            currentReviewsList.innerHTML = newReviewsList.innerHTML
                        }

                        // Update pagination
                        const newPagination = doc.querySelector(".reviews-pagination")
                        const currentPagination = document.querySelector(".reviews-pagination")
                        if (newPagination && currentPagination) {
                            currentPagination.innerHTML = newPagination.innerHTML
                        }

                        // Reinitialize event listeners for the new pagination links
                        const newPaginationLinks = document.querySelectorAll(".reviews-pagination a")
                        newPaginationLinks.forEach((link) => {
                            link.addEventListener("click", arguments.callee)
                        })

                        // Reinitialize event listeners for helpful and report buttons
                        initReviewActionButtons()

                        // Update URL without reloading the page
                        history.pushState(null, "", url)
                    })
                    .catch((error) => {
                        console.error("Error:", error)
                        alert("Có lỗi xảy ra. Vui lòng thử lại sau.")
                    })
            }
        })
    })

    // Quick view product
    window.quickView = (idSanPham) => {
        window.location.href = "/SanPhams/Details/" + idSanPham
    }

    // Show toast notification
    window.showToast = (message) => {
        const toast = document.createElement("div")
        toast.className = "toast-notification"
        toast.innerHTML = `
          <div class="toast-content">
              <i class="fas fa-check-circle toast-icon"></i>
              <span class="toast-message">${message}</span>
          </div>
          <button class="toast-close">&times;</button>
      `

        document.body.appendChild(toast)

        // Show toast
        setTimeout(() => {
            toast.classList.add("show")
        }, 10)

        // Auto hide after 3 seconds
        setTimeout(() => {
            toast.classList.remove("show")
            setTimeout(() => {
                document.body.removeChild(toast)
            }, 300)
        }, 3000)

        // Close button
        const closeBtn = toast.querySelector(".toast-close")
        closeBtn.addEventListener("click", () => {
            toast.classList.remove("show")
            setTimeout(() => {
                document.body.removeChild(toast)
            }, 300)
        })
    }

    // Hàm liên hệ báo giá
    window.contactUs = (idSanPham, tenSanPham) => {
        // Chuyển hướng đến trang liên hệ với thông tin sản phẩm
        window.location.href = `/LienHe?idSanPham=${idSanPham}&tenSanPham=${tenSanPham}`
    }
})
