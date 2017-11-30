angular.module('umbraco').controller('TwoWayPicker.DoctypePropertyPrevalueController',
  function ($scope, dialogService, entityResource, editorState, $log, iconHelper, $routeParams, twoWayPickerResource, fileManager, contentEditingHelper, angularHelper, navigationService, $location) {

      $scope.availableDocTypeAliases = null;
      $scope.model.value = $scope.model.value || [];
      $scope.selectedDocType = {};
      $scope.currentPropertyAliases = null;
      $scope.loading = true;
      twoWayPickerResource.getAllContentTypeAliasesWithProperties().success(function (aliases) {
          $scope.availableDocTypeAliases = aliases;
          $scope.loading = false;
      });

      $scope.add = function (item) {
          if ($scope.model.value.find(x => x.alias == item.alias) == undefined) {
              //The doc type was not even in the list so we can add it
              addSelectionToList(item);
          } else {
              var selectedDocType = $scope.model.value.find(x => x.alias == item.alias);

              //Check to make sure that they aren't trying to add the same property
              if (selectedDocType.selectedProperty == item.selectedProperty){
                  alert("You have already selected this property.");
              }
              else{
                  addSelectionToList(item);
              }
          }

          return false;
      }

      var addSelectionToList = function (item) {
          $scope.model.value.push(item);
              $scope.selectedDocType = {};    
      };

      $scope.remove = function (item) {
          var index = $scope.model.value.indexOf(item);
          if (index > -1) {
              $scope.model.value.splice(index, 1);
          }
      }

      $scope.docTypeAliasChanged = function () {
          var selectedDocType = $scope.availableDocTypeAliases.find(x => x.alias == $scope.selectedDocType.alias);
          $scope.currentPropertyAliases = selectedDocType.propertyAliases;
      }
  });