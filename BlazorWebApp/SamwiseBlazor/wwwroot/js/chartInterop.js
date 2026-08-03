window.chartInterop = {
    charts: {},

    renderChart: function (canvasId, chartType, labels, dataValues, dataLabel, options = {}) {
        const ctx = document.getElementById(canvasId);

        if (!ctx) {
            return;
        }

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx, {
            type: chartType,
            data: {
                labels: labels,
                datasets: [{
                    label: dataLabel,
                    data: dataValues,
                    borderWidth: 2,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    backgroundColor: 'rgba(75, 192, 192, 0.2)'
                }]
            },
            options: {
                responsive: true,
                ...options
            }
        });
    },
    addData: function (chart, labels, dataValue) {
        chart.data.labels.push(labels);
        chart.data.datasets[0].data.push(dataValue);
        chart.update();
    },
    removeData: function (chart) {
        chart.data.labels.pop();
        chart.data.datasets[0].data.pop();
        chart.update();
    }
};
