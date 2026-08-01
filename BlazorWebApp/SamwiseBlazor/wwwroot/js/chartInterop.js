window.chartInterop = {
    createChart: function (canvasId, chartType, labels, dataValues, dataLabel, options = {}) {
        const ctx = document.getElementById(canvasId);

        return new Chart(ctx, {
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
    }
};
