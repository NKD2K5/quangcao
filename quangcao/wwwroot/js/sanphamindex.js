document.addEventListener("DOMContentLoaded", () => {
    // Initialize Quick View Modal
    initQuickViewModal();

    // Toggle categories on mobile
    const toggleBtn = document.getElementById("toggleCategories");
    const categoryList = document.querySelector(".category-list-container");

    if (toggleBtn && categoryList) {
        toggleBtn.addEventListener("click", function () {
            categoryList.classList.toggle("show");
            this.innerHTML = categoryList.classList.contains("show")
                ? '<i class="fas fa-times"></i> Đóng danh mục'
                : '<i class="fas fa-filter"></i> Lọc danh mục';
        });
    }
});

// Quick View functionality
function initQuickViewModal() {
    const modal = document.getElementById("quickViewModal");
    const closeBtn = modal?.querySelector(".close-modal");

    if (!modal) return;

    // Close modal when clicking the close button
    if (closeBtn) {
        closeBtn.addEventListener("click", () => {
            closeQuickViewModal();
        });
    }

    // Close modal when clicking outside the content
    window.addEventListener("click", (event) => {
        if (event.target === modal) {
            closeQuickViewModal();
        }
    });

    // Close modal with Escape key
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && modal.style.display === "flex") {
            closeQuickViewModal();
        }
    });

    // Initialize retry button
    const retryBtn = document.getElementById("btnRetryQuickView");
    if (retryBtn) {
        retryBtn.addEventListener("click", () => {
            const productId = modal.getAttribute("data-product-id");
            if (productId) {
                loadQuickViewContent(productId);
            }
        });
    }
}

// Open Quick View Modal
function quickView(productId) {
    if (!productId) return;

    const modal = document.getElementById("quickViewModal");
    if (!modal) return;

    // Set product ID
    modal.setAttribute("data-product-id", productId);

    // Show loading state
    document.getElementById("quickViewLoading").style.display = "flex";
    document.getElementById("quickViewError").style.display = "none";
    document.getElementById("quickViewContainer").style.display = "none";

    // Show modal
    modal.style.display = "flex";
    setTimeout(() => {
        modal.classList.add("show");
    }, 10);

    // Load content
    loadQuickViewContent(productId);
}

// Close Quick View Modal
function closeQuickViewModal() {
    const modal = document.getElementById("quickViewModal");
    if (!modal) return;

    modal.classList.remove("show");
    setTimeout(() => {
        modal.style.display = "none";
    }, 300);
}

// Load Quick View Content
function loadQuickViewContent(productId) {
    const container = document.getElementById("quickViewContainer");
    const loading = document.getElementById("quickViewLoading");
    const error = document.getElementById("quickViewError");

    if (!container || !loading || !error) return;

    // Show loading
    loading.style.display = "flex";
    error.style.display = "none";
    container.style.display = "none";

    // Fetch product data
    fetch(`/SanPhams/QuickView/${productId}`)
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.text();
        })
        .then((html) => {
            // Hide loading
            loading.style.display = "none";

            // Show content
            container.innerHTML = html;
            container.style.display = "block";

            // Initialize image gallery
            initQuickViewGallery();
        })
        .catch((error) => {
            console.error("Error loading quick view:", error);

            // Show error
            loading.style.display = "none";
            error.style.display = "flex";
        });
}

// Initialize Quick View Gallery
function initQuickViewGallery() {
    const mainImage = document.getElementById("quickViewMainImage");
    const thumbnails = document.querySelectorAll(".qv-thumbnail");
    const prevBtn = document.getElementById("prevImage");
    const nextBtn = document.getElementById("nextImage");

    if (!mainImage || thumbnails.length === 0) return;

    let currentIndex = 0;
    const maxIndex = thumbnails.length - 1;

    // Set first thumbnail as active
    thumbnails[0].classList.add("active");

    // Function to update the main image
    function updateMainImage(index) {
        if (index < 0) index = maxIndex;
        if (index > maxIndex) index = 0;

        currentIndex = index;

        // Update active thumbnail
        thumbnails.forEach((thumb) => thumb.classList.remove("active"));
        thumbnails[index].classList.add("active");

        // Update main image
        const imgSrc = thumbnails[index].querySelector("img").src;
        mainImage.src = imgSrc;

        // Scroll thumbnail into view if needed
        thumbnails[index].scrollIntoView({ behavior: "smooth", block: "nearest", inline: "center" });
    }

    // Event listeners for navigation buttons
    if (prevBtn) {
        prevBtn.addEventListener("click", () => {
            updateMainImage(currentIndex - 1);
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener("click", () => {
            updateMainImage(currentIndex + 1);
        });
    }

    // Add click event to thumbnails
    thumbnails.forEach((thumb, index) => {
        thumb.addEventListener("click", () => {
            updateMainImage(index);
        });
    });

    // Add keyboard navigation
    document.addEventListener("keydown", (e) => {
        if (document.getElementById("quickViewModal").classList.contains("show")) {
            if (e.key === "ArrowLeft") {
                updateMainImage(currentIndex - 1);
            } else if (e.key === "ArrowRight") {
                updateMainImage(currentIndex + 1);
            }
        }
    });
}

// Add to wishlist function
function addToWishlist(productId) {
    const button = event.currentTarget;
    const icon = button.querySelector("i");

    // Toggle heart icon
    if (icon.classList.contains("far")) {
        icon.classList.remove("far");
        icon.classList.add("fas");
        icon.style.color = "#e53e3e";

        // Show success message
        showQuickViewToast("Đã thêm vào danh sách yêu thích", "success");
    } else {
        icon.classList.remove("fas");
        icon.classList.add("far");
        icon.style.color = "";

        // Show remove message
        showQuickViewToast("Đã xóa khỏi danh sách yêu thích", "info");
    }

    // Send request to server
    fetch("/SanPhamYeuThiches/ToggleWishlist", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "X-CSRF-TOKEN": document.querySelector('input[name="__RequestVerificationToken"]')?.value || "",
        },
        body: JSON.stringify({ productId: productId }),
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then((data) => {
            console.log("Wishlist updated:", data);
        })
        .catch((error) => {
            console.error("Error updating wishlist:", error);
            showQuickViewToast("Có lỗi xảy ra. Vui lòng thử lại sau.", "error");
        });
}

// Toast notification function
function showQuickViewToast(message, type = "info") {
    // Check if toast container exists, if not create it
    let toastContainer = document.getElementById("qv-toast-container");
    if (!toastContainer) {
        toastContainer = document.createElement("div");
        toastContainer.id = "qv-toast-container";
        document.body.appendChild(toastContainer);
    }

    // Create toast element
    const toast = document.createElement("div");
    toast.className = `qv-toast qv-toast-${type}`;

    // Set toast content
    toast.innerHTML = `
                <div style="display: flex; align-items: center; gap: 10px;">
                    <i class="fas fa-${type === "success"
            ? "check-circle"
            : type === "error"
                ? "exclamation-circle"
                : type === "warning"
                    ? "exclamation-triangle"
                    : "info-circle"
        }"></i>
                    <span>${message}</span>
                </div>
                <button style="background: none; border: none; color: white; cursor: pointer; font-size: 16px;">
                    <i class="fas fa-times"></i>
                </button>
            `;

    // Add to container
    toastContainer.appendChild(toast);

    // Animate in
    setTimeout(() => {
        toast.style.transform = "translateY(0)";
        toast.style.opacity = "1";
    }, 10);

    // Add close button functionality
    const closeBtn = toast.querySelector("button");
    closeBtn.addEventListener("click", () => {
        closeQuickViewToast(toast);
    });

    // Auto close after 4 seconds
    setTimeout(() => {
        closeQuickViewToast(toast);
    }, 4000);
}

function closeQuickViewToast(toast) {
    toast.style.transform = "translateY(20px)";
    toast.style.opacity = "0";

    setTimeout(() => {
        toast.remove();
    }, 300);
}

// Sử dụng form liên hệ hiện có thay vì mở modal báo giá
function contactUs(idSanPham, tenSanPham) {
    // Chuyển hướng đến trang liên hệ với thông tin sản phẩm
    window.location.href = `/LienHe?idSanPham=${idSanPham}&tenSanPham=${tenSanPham}`;
}