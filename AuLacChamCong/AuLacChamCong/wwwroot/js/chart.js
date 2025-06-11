
window.renderCharts = {
    bar: (canvasId, chartData) => {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.charts && window.charts[canvasId]) {
            window.charts[canvasId].destroy();
        }
        window.charts = window.charts || {};
        window.charts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: chartData,
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true }
                },
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: 'Tổng Ngày Công/Nghỉ/Lễ' }
                }
            }
        });
    },
    pie: (canvasId, chartData) => {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.charts && window.charts[canvasId]) {
            window.charts[canvasId].destroy();
        }
        window.charts = window.charts || {};
        window.charts[canvasId] = new Chart(ctx, {
            type: 'pie',
            data: chartData,
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'top' },
                    title: { display: true, text: 'Tỷ Lệ Ngày Công/Nghỉ/Lễ' }
                }
            }
        });
    },
    line: (canvasId, chartData) => {
        const ctx = document.getElementById(canvasId).getContext('2d');
        if (window.charts && window.charts[canvasId]) {
            window.charts[canvasId].destroy();
        }
        window.charts = window.charts || {};
        window.charts[canvasId] = new Chart(ctx, {
            type: 'line',
            data: chartData,
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true }
                },
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: 'Xu Hướng Ngày Công Theo Ngày' }
                }
            }
        });
    }
};