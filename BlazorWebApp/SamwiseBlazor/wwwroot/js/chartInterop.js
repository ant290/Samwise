window.chartInterop = {
    charts: {},

    renderChart: function (canvasId, chartType, labels, datasetMap, options = {}) {
        const ctx = document.getElementById(canvasId);

        if (!ctx) {
            return;
        }

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        const getColor = (index) => {
            const hue = (index * 67) % 360;
            return {
                border: `hsla(${hue}, 70%, 40%, 1)`,
                background: `hsla(${hue}, 70%, 70%, 0.25)`
            };
        };

        const datasets = (Array.isArray(datasetMap) ? datasetMap : Object.entries(datasetMap || {}).map(([label, values], index) => {
            const color = getColor(index);
            return {
                label,
                data: Array.isArray(values) ? values.map(v => v === undefined ? null : v) : [],
                borderWidth: 2,
                borderColor: color.border,
                backgroundColor: color.background,
                spanGaps: true,
                fill: false
            };
        }));

        this.charts[canvasId] = new Chart(ctx, {
            type: chartType,
            data: {
                labels: labels,
                datasets: datasets
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
