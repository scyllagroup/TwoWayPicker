angular.module('umbraco.resources').factory('twoWayPickerResource', function ($q, $http) {
    //the factory object returned
    return {
        getAllContentTypeAliasesWithProperties: function (urls, domains) {
            return $http({
                url: "backoffice/TwoWayPicker/TwoWayPickerApi/GetListOfContentTypeAliasesWithProperties",
                method: "GET"
            });
        }
    }
});