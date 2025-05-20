// Contact List JavaScript functionality

document.addEventListener('DOMContentLoaded', function () {
    // Initialize components
    initializeSearch();
    initializeTableSorting();
    initializeDeleteConfirmation();
    initializeContentPreview();

    // Check if there's a newly added item to highlight
    highlightNewItem();
});

// Search functionality
function initializeSearch() {
    const searchInput = document.getElementById('searchContacts');
    if (!searchInput) return;

    searchInput.addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase();
        const tableRows = document.querySelectorAll('.contact-table tbody tr');

        tableRows.forEach(row => {
            const text = row.textContent.toLowerCase();
            if (text.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });

        // Show message if no results
        const noResults = document.getElementById('noResults');
        const hasVisibleRows = Array.from(tableRows).some(row => row.style.display !== 'none');

        if (noResults) {
            noResults.style.display = hasVisibleRows ? 'none' : 'block';
        }
    });
}

// Table sorting functionality
function initializeTableSorting() {
    const tableHeaders = document.querySelectorAll('.contact-table th[data-sortable]');

    tableHeaders.forEach(header => {
        header.addEventListener('click', function () {
            const column = this.dataset.column;
            const isAscending = this.classList.contains('asc');

            // Remove sort classes from all headers
            tableHeaders.forEach(h => {
                h.classList.remove('asc', 'desc');
                h.querySelector('.sort-icon')?.remove();
            });

            // Add sort class and icon to current header
            this.classList.add(isAscending ? 'desc' : 'asc');
            const iconClass = isAscending ? 'bi-sort-down' : 'bi-sort-up';
            this.innerHTML += `<i class="bi ${iconClass} sort-icon ms-1"></i>`;

            // Sort the table
            sortTable(column, !isAscending);
        });
    });
}

function sortTable(column, asc) {
    const tbody = document.querySelector('.contact-table tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    // Sort rows
    const sortedRows = rows.sort((a, b) => {
        const aValue = a.querySelector(`td[data-column="${column}"]`).textContent.trim();
        const bValue = b.querySelector(`td[data-column="${column}"]`).textContent.trim();

        // Check if values are dates
        if (column === 'thoiGian') {
            return asc
                ? new Date(aValue) - new Date(bValue)
                : new Date(bValue) - new Date(aValue);
        }

        // Regular string comparison
        return asc
            ? aValue.localeCompare(bValue)
            : bValue.localeCompare(aValue);
    });

    // Remove existing rows
    rows.forEach(row => row.remove());

    // Append sorted rows
    sortedRows.forEach(row => tbody.appendChild(row));
}

// Delete confirmation
function initializeDeleteConfirmation() {
    const deleteButtons = document.querySelectorAll('.btn-delete');

    deleteButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            if (!confirm('Are you sure you want to delete this contact?')) {
                e.preventDefault();
            }
        });
    });
}

// Content preview on hover
function initializeContentPreview() {
    const contentCells = document.querySelectorAll('.content-cell');

    contentCells.forEach(cell => {
        const content = cell.textContent;

        if (content.length > 50) {
            // Create tooltip for long content
            cell.setAttribute('data-bs-toggle', 'tooltip');
            cell.setAttribute('data-bs-placement', 'top');
            cell.setAttribute('title', content);

            // Initialize Bootstrap tooltips
            const tooltip = new bootstrap.Tooltip(cell);
        }
    });
}

// Highlight newly added items
function highlightNewItem() {
    const urlParams = new URLSearchParams(window.location.search);
    const newItemId = urlParams.get('newItem');

    if (newItemId) {
        const newRow = document.querySelector(`tr[data-id="${newItemId}"]`);
        if (newRow) {
            newRow.classList.add('highlight-new');
            newRow.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }
}

// Export to CSV functionality
function exportToCSV() {
    const table = document.querySelector('.contact-table');
    if (!table) return;

    const rows = table.querySelectorAll('tbody tr:not([style*="display: none"])');
    const headers = Array.from(table.querySelectorAll('thead th'))
        .map(header => header.textContent.trim());

    let csvContent = headers.join(',') + '\n';

    rows.forEach(row => {
        const rowData = Array.from(row.querySelectorAll('td'))
            .map(cell => {
                // Escape quotes and wrap content in quotes
                let content = cell.textContent.trim();
                content = content.replace(/"/g, '""');
                return `"${content}"`;
            });

        csvContent += rowData.join(',') + '\n';
    });

    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');

    link.setAttribute('href', url);
    link.setAttribute('download', 'contacts.csv');
    link.style.display = 'none';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Print functionality
function printTable() {
    const tableContainer = document.querySelector('.contact-card');
    const printWindow = window.open('', '_blank');

    printWindow.document.write(`
        <html>
        <head>
            <title>Contacts List</title>
            <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css">
            <link rel="stylesheet" href="/css/lienhe.css">
            <style>
                @media print {
                    .no-print { display: none; }
                    body { padding: 20px; }
                }
            </style>
        </head>
        <body>
            <h1 class="mb-4">Contacts List</h1>
            ${tableContainer.outerHTML}
            <script>
                window.onload = function() {
                    document.querySelectorAll('.no-print').forEach(el => el.remove());
                    window.print();
                    window.close();
                }
            </script>
        </body>
        </html>
    `);
}