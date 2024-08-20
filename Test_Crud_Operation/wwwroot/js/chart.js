$(function () {
    var chartName = "chart";
    var ctx = document.getElementById(chartName).getContext('2d');

    var data = {
        labels: Html.Raw(XLabels),
        datasets: Html.Raw(datasets)
    };

    var options = {
        maintainAspectRatio: false,
        scales: {
            yAxes: [{
                ticks: {
                    min: 0,
                    beginAtZero: true
                },
                gridLines: {
                    display: true,
                    color: "rgba(255,99,164,0.2)"
                }
            }],
            xAxes: [{
                ticks: {
                    min: 0,
                    beginAtZero: true
                },
                gridLines: {
                    display: false
                }
            }]
        }
    };

    var myChart = new Chart(ctx, {
        options: options,
        data: data,
        type: 'bar'
    });
})