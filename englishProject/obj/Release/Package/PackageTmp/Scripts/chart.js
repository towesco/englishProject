google.charts.load('current', { 'packages': ['corechart'] });
google.load('visualization', '1.0', { 'packages': ['corechart'] });

$(document).ready(function () {
    var drawChart = function (chart) {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Tarih');
        data.addColumn('number', 'Hedef puan');
        data.addColumn('number', 'Kazanmış olduğun puan');
        data.addRows(chart.length);
        $.each(chart, function (i, val) {
            data.setCell(i, 0, val.date);
            data.setCell(i, 1, val.targetScore);
            data.setCell(i, 2, val.currentScore);
        });

        var options = {
            backgroundColor: '#f5f5f5',
            hAxis: { title: 'Tarih', titleTextStyle: { color: '#333' } },
            vAxis: { minValue: 0 },
            width: 800,
            height: 300,
            legend: { position: 'none' },
            pointShape: 'circle',
            pointSize: 10,
        };

        var charts = new window.google.visualization.AreaChart(document.getElementById('chart_div'));
        charts.draw(data, options);
    };

    $.getJSON("/api/ajax/getChart", function (data) {
        drawChart(data);
    });

    ;
})