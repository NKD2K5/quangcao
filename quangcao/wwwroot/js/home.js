function renderServices(categoryId) {
    const category = categories[categoryId];

    if (!category) return;

    // Update category title and description
    categoryTitle.textContent = category.name;
    categoryDescription.textContent = category.description;

    // Clear services grid
    servicesGrid.innerHTML = '';

    // Render services
    category.services.forEach(service => {
        const serviceCard = document.createElement('div');
        serviceCard.className = 'service-card';
        serviceCard.dataset.id = service.id;

        serviceCard.innerHTML = `
                <div class="service-image">
                    <img src="${service.image}" alt="${service.name}">
                </div>
                <div class="service-content">
                    <h3>${service.name}</h3>
                    <p>${service.description}</p>
                    <button class="detail-btn" data-id="${service.id}">
                        <span>Chi tiết</span>
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </div>
                <div class="service-details" id="details-${service.id}">
                    <h4>Đặc điểm nổi bật:</h4>
                    <ul class="features-list">
                        ${service.features.map(feature => `<li>${feature}</li>`).join('')}
                    </ul>
                    <div class="order-btn-container">
                        <a href="/san-pham/${service.id}" class="order-btn">Đặt hàng ngay</a>
                    </div>
                </div>
            `;

        servicesGrid.appendChild(serviceCard);
    });

    // Add event listeners to detail buttons
    const detailButtons = document.querySelectorAll('.detail-btn');
    detailButtons.forEach(button => {
        button.addEventListener('click', function () {
            const serviceId = this.dataset.id;
            const detailsElement = document.getElementById(`details-${serviceId}`);
            const serviceCard = this.closest('.service-card');

            // Toggle details visibility
            if (detailsElement.classList.contains('active')) {
                detailsElement.classList.remove('active');
                serviceCard.classList.remove('active');
            } else {
                // Close any open details
                document.querySelectorAll('.service-details.active').forEach(el => {
                    el.classList.remove('active');
                    el.closest('.service-card').classList.remove('active');
                });

                // Open this detail
                detailsElement.classList.add('active');
                serviceCard.classList.add('active');
            }
        });
    });
}

// Initialize with first category
renderServices('in-an');

// Category button click handlers
categoryButtons.forEach(button => {
    button.addEventListener('click', function () {
        const categoryId = this.dataset.category;

        // Update active button
        categoryButtons.forEach(btn => btn.classList.remove('active'));
        this.classList.add('active');

        // Update select for mobile
        categorySelect.value = categoryId;

        // Render services for selected category
        renderServices(categoryId);
    });
});

// Mobile category select change handler
if (categorySelect) {
    categorySelect.addEventListener('change', function () {
        const categoryId = this.value;

        // Update active button
        categoryButtons.forEach(btn => {
            if (btn.dataset.category === categoryId) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });

        // Render services for selected category
        renderServices(categoryId);
    });
}
});