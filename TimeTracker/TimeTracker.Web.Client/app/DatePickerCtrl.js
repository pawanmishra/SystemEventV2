(function() {
    "use strict";
    angular.module("timeTracker")
        .constant("baseUrl", "http://localhost:50040/api/tracker/")
        .controller("DatePickerCtrl", ["$rootScope", "$filter", "timeTrackerResource", datePickerCtrl]);

    function datePickerCtrl($rootScope, $filter, timeTrackerResource) {
        var vm = this;

        vm.startDate = new Date();
        vm.endDate = new Date();

        vm.formats = ['dd-MM-yyyy'];
        vm.format = vm.formats[0];

        vm.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };

        vm.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            vm.opened = true;
        };

        vm.getResult = function () {
            var start = $filter('date')(vm.startDate, "dd-MM-yyyy");
            var end = $filter('date')(vm.endDate, "dd-MM-yyyy");
            timeTrackerResource.query({ startDate: start, endDate: end }, function (data) {
                $rootScope.$broadcast("dataRecieved", data);
            });
        };
    }
})();