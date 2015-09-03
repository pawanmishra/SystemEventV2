/// <reference path="../../Scripts/jquery-1.9.1.min.js" />
/// <reference path="../../Scripts/angular.min.js" />

(function () {
    "use strict";
    angular.module("timeTracker").controller("TimeTrackerCtrl", ["$rootScope", "$filter", "timeTrackerHistoryResource", timeTrackerCtrl]);

    function timeTrackerCtrl($rootScope, $filter, timeTrackerHistoryResource) {
        var vm = this;

        $rootScope.$on("dataRecieved", function (event, message) {
            bindChart(vm, $rootScope, $filter, timeTrackerHistoryResource, message);
        });
    }

    function bindChart(scope, $rootScope, $filter, timeTrackerHistoryResource, trackerData) {

        var meetingMinutes = $.map(trackerData, function (e) { return e.meeting_minutes; })
        var activeMinutes = $.map(trackerData, function (e) { return e.active_minutes; })
        var dates = $.map(trackerData, function (e) { return e.date; });

        //This is not a highcharts object. It just looks a little like one!
        scope.chartConfig = {
            options: {
                //This is the Main Highcharts chart config. Any Highchart options are valid here.
                //will be overriden by values specified below.
                chart: {
                    type: 'line'
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
                    //line: {
                    //    dataLabels: {
                    //        enabled: true
                    //    }
                    //},
                    series: {
                        cursor: 'pointer',
                        point: {
                            events: {
                                click: function (event) {
                                    var tempCategory = this.category;
                                    //console.log('Category: ' + this.category + ', value: ' + this.y);
                                    var items = $.grep(trackerData, function (item, i) {
                                        if (item.date === tempCategory) {
                                            return item;
                                        }
                                    });
                                    var result = items[0].id;
                                    timeTrackerHistoryResource.query({ id: result }, function (data) {
                                        $rootScope.$broadcast("historyDataRecieved", data);
                                    });
                                }
                            }
                        }
                    }
                }
            },
            //The below properties are watched separately for changes.

            //Series object (optional) - a list of series using normal highcharts series options.
            series: [{
                name: 'active_minutes',
                data: activeMinutes
            }, {
                name: 'meeting_minutes',
                data: meetingMinutes
            }],
            //Title configuration (optional)
            title: {
                text: 'Active vs Meeting minutes'
            },
            //Boolean to control showng loading status on chart (optional)
            //Could be a string if you want to show specific loading text.
            loading: false,
            //Configuration for the xAxis (optional). Currently only one x axis can be dynamically controlled.
            //properties currentMin and currentMax provied 2-way binding to the chart's maximimum and minimum
            xAxis: {
                title: { text: 'Date' },
                categories: dates
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