(function () {
    "use strict";
    angular.module("common.services")
           .factory("timeTrackerHistoryResource",
                    ["$resource", "appSettings", timeTrackerHistoryResource]);

    function timeTrackerHistoryResource($resource, appSettings) {
        return $resource(appSettings.serverPath + "api/tracker_history/:id");
    }
})();