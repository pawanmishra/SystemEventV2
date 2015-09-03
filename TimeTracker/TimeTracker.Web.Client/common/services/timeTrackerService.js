(function () {
    "use strict";
    angular.module("common.services")
           .factory("timeTrackerResource",
                    ["$resource", "appSettings", timeTrackerResource]);

    function timeTrackerResource($resource, appSettings)
    {
        return $resource(appSettings.serverPath + "api/tracker/");
    }
})();