    // Global variables
    let currentShareUrl = '';
    let currentShareTitle = '';

    // Initialize when DOM is loaded
    document.addEventListener('DOMContentLoaded', function() {
        initializeModals();
    handleImageErrors();
    addLoadingStates();
    });

    // Initialize modals
    function initializeModals() {
        const shareModalEl = document.getElementById('shareModal');
    const deleteModalEl = document.getElementById('deleteModal');

    if (shareModalEl) {
        window.shareModal = new bootstrap.Modal(shareModalEl);
        }

    if (deleteModalEl) {
        window.deleteModal = new bootstrap.Modal(deleteModalEl);
        }
    }

    // Handle image errors
    function handleImageErrors() {
        document.querySelectorAll('.card-image').forEach(img => {
            img.addEventListener('error', function () {
                const container = this.parentNode;
                container.innerHTML = `
                    <div style="height: 100%; display: flex; flex-direction: column; align-items: center; justify-content: center; background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%); color: #adb5bd;">
                        <i class="bi bi-image-fill" style="font-size: 2.5rem; margin-bottom: 0.5rem; opacity: 0.6;"></i>
                        <span style="font-size: 0.9rem; font-weight: 500;">Không thể tải ảnh</span>
                    </div>
                `;
            });
        });
    }

    // Add loading states to buttons
    function addLoadingStates() {
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', function () {
                const submitBtn = this.querySelector('button[type="submit"]');
                if (submitBtn) {
                    const originalText = submitBtn.innerHTML;
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<i class="bi bi-hourglass-split me-1"></i>Đang xử lý...';

                    setTimeout(() => {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }, 3000);
                }
            });
        });
    }

    // Enhanced share functionality
    function openShareModal(title, id) {
        currentShareTitle = title;
    currentShareUrl = `${window.location.origin}${window.location.pathname.replace(/\/[^\/]*$/, '')}/Details/${id}`;

    document.getElementById('shareTitle').textContent = title;
    document.getElementById('shareUrl').value = currentShareUrl;

    // Reset copy success message
    document.getElementById('copySuccess').classList.remove('show');

    if (window.shareModal) {
        window.shareModal.show();
        }
    }

    function shareToSocial(platform) {
        const encodedUrl = encodeURIComponent(currentShareUrl);
    const encodedTitle = encodeURIComponent(currentShareTitle);

    const urls = {
        facebook: `https://www.facebook.com/sharer/sharer.php?u=${encodedUrl}`,
    twitter: `https://twitter.com/intent/tweet?text=${encodedTitle}&url=${encodedUrl}`,
    linkedin: `https://www.linkedin.com/sharing/share-offsite/?url=${encodedUrl}`,
    email: `mailto:?subject=${encodedTitle}&body=${encodeURIComponent('Xem tin tức này tại: ' + currentShareUrl)}`
        };

    if (platform === 'email') {
        window.location.href = urls[platform];
        } else {
            const popup = window.open(urls[platform], '_blank', 'width=600,height=400,scrollbars=yes,resizable=yes');

    // Focus on popup if it was successfully opened
    if (popup) {
        popup.focus();
            }
        }
    }

    function copyShareLink() {
        const shareUrl = document.getElementById('shareUrl');
    const copySuccess = document.getElementById('copySuccess');
    const copyBtn = document.querySelector('.copy-btn');

    // Temporarily change button text
    const originalBtnContent = copyBtn.innerHTML;
    copyBtn.innerHTML = '<i class="bi bi-check"></i>';
    copyBtn.disabled = true;

    shareUrl.select();
    shareUrl.setSelectionRange(0, 99999);

    try {
            if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(shareUrl.value).then(() => {
            showCopySuccess(copySuccess, copyBtn, originalBtnContent);
        }).catch(() => {
            fallbackCopy(shareUrl, copySuccess, copyBtn, originalBtnContent);
        });
            } else {
        fallbackCopy(shareUrl, copySuccess, copyBtn, originalBtnContent);
            }
        } catch (err) {
        console.error('Copy failed:', err);
    fallbackCopy(shareUrl, copySuccess, copyBtn, originalBtnContent);
        }
    }

    function fallbackCopy(shareUrl, copySuccess, copyBtn, originalBtnContent) {
        try {
        document.execCommand('copy');
    showCopySuccess(copySuccess, copyBtn, originalBtnContent);
        } catch (err) {
        console.error('Fallback copy failed:', err);
    resetCopyButton(copyBtn, originalBtnContent);
        }
    }

    function showCopySuccess(copySuccess, copyBtn, originalBtnContent) {
        copySuccess.classList.add('show');

        setTimeout(() => {
        copySuccess.classList.remove('show');
    resetCopyButton(copyBtn, originalBtnContent);
        }, 3000);
    }

    function resetCopyButton(copyBtn, originalBtnContent) {
        copyBtn.innerHTML = originalBtnContent;
    copyBtn.disabled = false;
    }

    // Delete functionality
    function openDeleteModal(id, title) {
        document.getElementById('deleteId').value = id;
    document.getElementById('deleteTitle').textContent = title;

    if (window.deleteModal) {
        window.deleteModal.show();
        }
    }

    // Enhanced keyboard navigation
    document.addEventListener('keydown', function(e) {
        // Close modals with Escape key
        if (e.key === 'Escape') {
            const openModals = document.querySelectorAll('.modal.show');
            openModals.forEach(modal => {
                const modalInstance = bootstrap.Modal.getInstance(modal);
    if (modalInstance) {
        modalInstance.hide();
                }
            });
        }
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
