// Initialize everything when the document is ready
document.addEventListener('DOMContentLoaded', function () {
    initializeForm();
    setupImageUpload();
    setupValidation();
    setupPriceFormatting();
    setupCharacterCounting();
});

function initializeForm() {
    // Initialize Select2
    if (typeof $ !== 'undefined' && typeof $.fn.select2 !== 'undefined') {
        $('.select2').select2({
            theme: 'bootstrap-5',
            width: '100%'
        });
    }

    // Initialize TinyMCE
    if (typeof tinymce !== 'undefined') {
        tinymce.init({
            selector: '.editor-textarea',
            height: 400,
            menubar: false,
            plugins: [
                'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
                'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
                'insertdatetime', 'media', 'table', 'help', 'wordcount'
            ],
            toolbar: 'undo redo | formatselect | ' +
                'bold italic backcolor | alignleft aligncenter ' +
                'alignright alignjustify | bullist numlist outdent indent | ' +
                'removeformat | help',
            content_style: 'body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 16px; line-height: 1.6; }'
        });
    }

    // Initialize existing media preview
    const currentMedia = document.querySelectorAll('.media-item');
    currentMedia.forEach(item => {
        const previewBtn = item.querySelector('.preview');
        const deleteBtn = item.querySelector('.delete');

        if (previewBtn) {
            previewBtn.addEventListener('click', function () {
                const mediaPath = item.dataset.path;
                previewMedia('/' + mediaPath);
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', function () {
                const mediaPath = item.dataset.path;
                deleteMedia(this, mediaPath);
            });
        }
    });
}

function previewMedia(mediaPath) {
    const modal = document.getElementById('mediaPreviewModal');
    const previewContent = document.getElementById('previewContent');

    if (!modal || !previewContent) return;

    const isVideo = mediaPath.toLowerCase().endsWith('.mp4') ||
        mediaPath.toLowerCase().endsWith('.webm');

    // Ensure mediaPath starts with '/' for server paths
    const fullPath = mediaPath.startsWith('data:') ? mediaPath :
        (mediaPath.startsWith('/') ? mediaPath : '/' + mediaPath);

    if (isVideo) {
        previewContent.innerHTML = `
            <video controls style="max-width: 100%; max-height: 70vh; background: #000;">
                <source src="${fullPath}" type="video/mp4">
                Your browser does not support the video tag.
            </video>`;

        const video = previewContent.querySelector('video');
        video.addEventListener('loadedmetadata', () => {
            video.play().catch(e => console.log('Auto-play prevented:', e));
        });
    } else {
        previewContent.innerHTML = `
            <img src="${fullPath}" style="max-width: 100%; max-height: 70vh;" alt="Preview">`;
    }

    modal.style.display = 'block';
    setTimeout(() => modal.classList.add('show'), 10);
}

function deleteMedia(button, mediaPath) {
    const mediaItem = button.closest('.media-item');
    if (!mediaItem) return;

    // Remove the hidden input for this media file
    const hiddenInput = mediaItem.querySelector('input[name="AnhCuGiuLai"]');
    if (hiddenInput) {
        hiddenInput.remove();
    }

    mediaItem.classList.add('deleted');
    const statusBadge = mediaItem.querySelector('.status-badge');
    if (statusBadge) {
        statusBadge.innerHTML = '<i class="fas fa-trash"></i> Đã xóa';
        statusBadge.style.color = '#dc2626';
    }

    // Add restore button
    const mediaActions = mediaItem.querySelector('.media-actions');
    if (mediaActions) {
        const restoreBtn = document.createElement('button');
        restoreBtn.className = 'btn-action restore';
        restoreBtn.innerHTML = '<i class="fas fa-undo"></i>';
        restoreBtn.onclick = () => restoreMedia(mediaItem, mediaPath);
        mediaActions.appendChild(restoreBtn);
    }

    // Update hidden input for deleted media
    const deletedMediaInput = document.getElementById('deletedMedia');
    if (deletedMediaInput) {
        const currentDeleted = deletedMediaInput.value.split(';').filter(Boolean);
        currentDeleted.push(mediaPath);
        deletedMediaInput.value = currentDeleted.join(';');
    }
}

function restoreMedia(mediaItem, mediaPath) {
    mediaItem.classList.remove('deleted');
    const statusBadge = mediaItem.querySelector('.status-badge');
    if (statusBadge) {
        statusBadge.innerHTML = '<i class="fas fa-check-circle"></i> Đã lưu';
        statusBadge.style.color = '#059669';
    }

    // Add back the hidden input
    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.name = 'AnhCuGiuLai';
    hiddenInput.value = mediaPath;
    mediaItem.appendChild(hiddenInput);

    // Remove restore button
    const restoreBtn = mediaItem.querySelector('.restore');
    if (restoreBtn) {
        restoreBtn.remove();
    }

    // Update hidden input for deleted media
    const deletedMediaInput = document.getElementById('deletedMedia');
    if (deletedMediaInput) {
        const currentDeleted = deletedMediaInput.value.split(';').filter(path => path !== mediaPath);
        deletedMediaInput.value = currentDeleted.join(';');
    }
}

function setupImageUpload() {
    const mediaInput = document.getElementById('mediaInput');
    const dropzone = document.getElementById('dropzone');
    const newMediaGrid = document.getElementById('newMediaGrid');
    const newMediaSection = document.getElementById('newMediaSection');

    if (mediaInput) {
        mediaInput.addEventListener('change', function (e) {
            handleFiles(e.target.files);
        });
    }

    if (dropzone) {
        // Add click handler
        dropzone.addEventListener('click', () => {
            mediaInput.click();
        });

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropzone.addEventListener(eventName, preventDefaults);
            document.body.addEventListener(eventName, preventDefaults);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            dropzone.addEventListener(eventName, () => {
                dropzone.classList.add('drag-over');
            });
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropzone.addEventListener(eventName, () => {
                dropzone.classList.remove('drag-over');
            });
        });

        dropzone.addEventListener('drop', function (e) {
            const dt = e.dataTransfer;
            if (dt.files.length) {
                mediaInput.files = dt.files;
                handleFiles(dt.files);
            }
        });
    }
}

function handleFiles(files) {
    if (!files.length) return;

    const newMediaGrid = document.getElementById('newMediaGrid');
    const newMediaSection = document.getElementById('newMediaSection');

    if (!newMediaGrid || !newMediaSection) return;

    // Show new media section without clearing existing content
    newMediaSection.style.display = 'block';

    Array.from(files).forEach((file, index) => {
        const reader = new FileReader();
        reader.onload = function (e) {
            const mediaItem = createMediaPreview(e.target.result, file, index);
            newMediaGrid.appendChild(mediaItem);
        };
        reader.readAsDataURL(file);
    });
}

function createMediaPreview(previewUrl, file, index) {
    const isVideo = file.type.startsWith('video/');
    const div = document.createElement('div');
    div.className = 'media-item new';

    if (isVideo) {
        div.innerHTML = `
            <div class="media-wrapper">
                <video class="media-preview" controls style="object-fit: contain;">
                    <source src="${previewUrl}" type="${file.type}">
                </video>
                <div class="media-type-badge video">
                    <i class="fas fa-video"></i> Video
                </div>
                <div class="media-overlay">
                    <div class="media-actions">
                        <button type="button" class="btn-action preview" onclick="previewMedia('${previewUrl}')">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn-action delete" onclick="removeNewMedia(${index})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
            <div class="media-status">
                <span class="status-badge">
                    <i class="fas fa-plus-circle"></i> Mới
                </span>
            </div>`;
    } else {
        div.innerHTML = `
            <div class="media-wrapper">
                <img src="${previewUrl}" class="media-preview" alt="Preview">
                <div class="media-type-badge image">
                    <i class="fas fa-image"></i> Ảnh
                </div>
                <div class="media-overlay">
                    <div class="media-actions">
                        <button type="button" class="btn-action preview" onclick="previewMedia('${previewUrl}')">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button type="button" class="btn-action delete" onclick="removeNewMedia(${index})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
            <div class="media-status">
                <span class="status-badge">
                    <i class="fas fa-plus-circle"></i> Mới
                </span>
            </div>`;
    }

    return div;
}

function removeNewMedia(index) {
    const mediaInput = document.getElementById('mediaInput');
    const newMediaGrid = document.getElementById('newMediaGrid');
    const newMediaSection = document.getElementById('newMediaSection');

    if (!mediaInput || !newMediaGrid || !newMediaSection) return;

    const dt = new DataTransfer();
    Array.from(mediaInput.files).forEach((file, i) => {
        if (i !== index) dt.items.add(file);
    });

    mediaInput.files = dt.files;

    if (dt.files.length > 0) {
        handleFiles(dt.files);
    } else {
        newMediaGrid.innerHTML = '';
        newMediaSection.style.display = 'none';
    }
}

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

// Close preview modal
function closePreviewModal() {
    const modal = document.getElementById('mediaPreviewModal');
    if (modal) {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.style.display = 'none';
            document.getElementById('previewContent').innerHTML = '';
        }, 300);
    }
}

// Setup price formatting
function setupPriceFormatting() {
    const priceInput = document.querySelector('input[name="Gia"]');
    if (!priceInput) return;

    function formatPrice(price) {
        // Convert to thousands (N3)
        price = price * 1000;
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(price);
    }

    function updateDisplay() {
        const display = document.getElementById('priceDisplay');
        if (display && priceInput.value) {
            display.textContent = formatPrice(priceInput.value);
        }
    }

    // Format initial value
    if (priceInput.value) {
        const initialValue = parseFloat(priceInput.value) / 1000;
        priceInput.value = initialValue;
    }

    priceInput.addEventListener('input', updateDisplay);
    updateDisplay();
}

// Setup character counting
function setupCharacterCounting() {
    const textareas = document.querySelectorAll('textarea[maxlength]');
    textareas.forEach(textarea => {
        const counter = document.createElement('div');
        counter.className = 'character-count';
        textarea.parentNode.appendChild(counter);

        function updateCount() {
            const remaining = textarea.maxLength - textarea.value.length;
            counter.textContent = `${textarea.value.length}/${textarea.maxLength}`;
            counter.classList.toggle('warning', textarea.value.length > textarea.maxLength * 0.9);
        }

        textarea.addEventListener('input', updateCount);
        updateCount();
    });
}

// Form validation
function setupValidation() {
    const form = document.getElementById('editForm');
    if (!form) return;

    form.addEventListener('submit', function (e) {
        if (!form.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }
        form.classList.add('was-validated');
    });
}

// Close modal on click outside
document.addEventListener('click', function (e) {
    const modal = document.getElementById('mediaPreviewModal');
    if (modal && e.target === modal) {
        closePreviewModal();
    }
});

// Close modal on Escape key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closePreviewModal();
    }
});