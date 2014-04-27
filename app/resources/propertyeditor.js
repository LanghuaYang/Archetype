angular.module('umbraco.resources').factory('archetypePropertyEditorResource', function($q, $http, umbRequestHelper){
    return { 
        getAllDataTypes: function() {
            // Hack - grab DataTypes from Tree API, as `dataTypeService.getAll()` isn't implemented yet
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetAll"), 'Failed to retrieve datatypes from tree service'
            );
        },
        getDataType: function(guid) {
        	return umbRequestHelper.resourcePromise(
        		$http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetByGuid?guid=" + guid), 'Failed to retrieve datatype'
    		);
        },
        getPropertyEditorMapping: function(alias) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetAllPropertyEditors"), 'Failed to retrieve datatype mappings'
            ).then(function (data) {
                var result = _.find(data, function(d) {
                    return d.alias === alias;
                });

                if (result != null) 
                    return result;

                return "";
            });
        },
        getConfiguration: function (guid) {
            // GetConfiguration expects a guid - make sure we're giving it one, even if it's empty :)
            guid = guid || "00000000-0000-0000-0000-000000000000";
            return umbRequestHelper.resourcePromise(
        		$http.get("/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/GetConfiguration?guid=" + guid), 'Failed to get configuration'
    		);
        },
        saveConfiguration: function (guid, configuration) {
            // must perform synchronous save operation to keep the data type editor from reloading 
            // before the configuration has been saved to the API controller
            // - thus we can't use umbRequestHelper.resourcePromise(...) as it's purely asynchronous :(
            var result = false;
            $.ajax({
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                url: "/umbraco/backoffice/ArchetypeApi/ArchetypeDataType/PutConfiguration",
                data: JSON.stringify({ id: guid, configuration: configuration }),
                async: false,
                success: function (data, textStatus, xhr) {
                    //console.log("Configuration saved");
                    result = true;
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log("Could not save configuration: " + textStatus);
                    result = false;
                }
            });
            return result;
        }
    }
}); 