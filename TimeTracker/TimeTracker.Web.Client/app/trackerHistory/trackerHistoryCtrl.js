(function () {
    "use strict";
    angular.module("timeTracker").controller("TimeTrackerHistoryCtrl", ["$rootScope", "$filter", timeTrackerHistoryCtrl]);

    function timeTrackerHistoryCtrl($rootScope, $filter) {
        var vm = this;

        $rootScope.$on("historyDataRecieved", function (event, message) {
            bindChart(vm, $rootScope, $filter, message);
        });
    }

    function bindChart(scope, $rootScope, $filter, trackerHistoryData)
    {
        var activeMinutes = $.map(trackerHistoryData, function (e) { return e.active_minutes; })
        var updateTime = $.map(trackerHistoryData, function (e) { return e.last_update; });

        //This is not a highcharts object. It just looks a little like one!
        scope.chartConfig = {
            options: {
                //This is the Main Highcharts chart config. Any Highchart options are valid here.
                //will be overriden by values specified below.
                chart: {
                    type: 'spline'
                },
                tooltip: {
                    style: {
                        padding: 10,
                        fontWeight: 'bold'
                    },
                    headerFormat: '<b>{series.name}</b><br>',
                    pointFormat: '{point.y} minutes'
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Total Minutes'
                    },
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    }
                },
                plotOptions: {
                    spline: {
                        dataLabels: {
                            enabled: true
                        }
                    },
                    series: {
                        cursor: 'pointer'
                    }
                }
            },
            //The below properties are watched separately for changes.

            //Series object (optional) - a list of series using normal highcharts series options.
            series: [{
                name: 'active_minutes',
                data: activeMinutes
            }],
            //Title configuration (optional)
            title: {
                text: 'Active Minutes'
            },
            //Boolean to control showng loading status on chart (optional)
            //Could be a string if you want to show specific loading text.
            loading: false,
            //Configuration for the xAxis (optional). Currently only one x axis can be dynamically controlled.
            //properties currentMin and currentMax provied 2-way binding to the chart's maximimum and minimum
            xAxis: {
                title: { text: 'Date' },
                categories: updateTime
            },
            //Whether to use HighStocks instead of HighCharts (optional). Defaults to false.
            useHighStocks: false,
            //size (optional) if left out the chart will default to size of the div or something sensible.
            //function (optional)
            func: function (chart) {
                //setup some logic for the chart
            }
        };
    }
})();