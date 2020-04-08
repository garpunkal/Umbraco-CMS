/**
 * @ngdoc controller
 * @name Umbraco.Editors.BlockList.BlockConfigurationOverlayController
 * @function
 *
 * @description
 * The controller for the content type editor property settings dialog
 */

(function () {
    "use strict";

    function BlockConfigurationOverlayController($scope, overlayService, localizationService, editorService, elementTypeResource) {

        var vm = this;
        vm.block = $scope.model.block;

        loadElementTypes();

        function loadElementTypes() {
            return elementTypeResource.getAll().then(function (elementTypes) {
                vm.elementTypes = elementTypes;
            });
        }

        vm.getElementTypeByAlias = function(alias) {
            return _.find(vm.elementTypes, function (type) {
                return type.alias === alias;
            });
        };

        vm.openElementType = function(elementTypeAlias) {
            var elementTypeId = vm.getElementTypeByAlias(elementTypeAlias).id;
            const editor = {
                id: elementTypeId,
                submit: function (model) {
                    loadElementTypes();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };
            editorService.documentTypeEditor(editor);
        }

        vm.addSettingsForBlock = function ($event, block) {

            var elemTypeSelectorOverlay = {
                view: "itempicker",
                title: "Pick settings (missing translation)",
                availableItems: vm.elementTypes,
                position: "target",
                event: $event,
                size: vm.elementTypes.length < 7 ? "small" : "medium",
                createNewItem: {
                    action: function() {
                        overlayService.close();
                        vm.createElementTypeAndAdd((alias) => {
                            vm.applySettingsToBlock(block, alias);
                        });
                    },
                    icon: "icon-add",
                    name: "Create new"
                },
                submit: function (overlay) {
                    vm.applySettingsToBlock(block, overlay.selectedItem.alias);
                    overlayService.close();
                },
                close: function () {
                    overlayService.close();
                }
            };

            overlayService.open(elemTypeSelectorOverlay);
        };
        vm.applySettingsToBlock = function(block, alias) {
            block.settingsElementTypeAlias = alias;
        };

        vm.requestRemoveSettingsForBlock = function(block) {
            localizationService.localizeMany(["general_remove", "defaultdialogs_confirmremoveusageof"]).then(function (data) {

                var settingsElementType = vm.getElementTypeByAlias(entry.settingsElementTypeAlias);

                overlayService.confirmRemove({
                    title: data[0],
                    content: localizationService.tokenReplace(data[1], [settingsElementType.name]),
                    close: function () {
                        overlayService.close();
                    },
                    submit: function () {
                        vm.removeSettingsForEntry(entry);
                        overlayService.close();
                    }
                });
            });
        };
        vm.removeSettingsForEntry = function(entry) {
            entry.settingsElementTypeAlias = null;
        };


        vm.addViewForBlock = function(block) {
            const filePicker = {
                title: "Select view (TODO need translation)",
                section: "settings",
                treeAlias: "files",
                entityType: "file",
                isDialog: true,
                filter: function (i) {
                    return i.name.indexOf(".html" !== -1);
                },
                select: function (file) {
                    console.log(file);
                    block.view = file.name;
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };
            editorService.treePicker(filePicker);
        }
        vm.requestRemoveViewForBlock = function(block) {
            localizationService.localizeMany(["general_remove", "defaultdialogs_confirmremoveusageof"]).then(function (data) {
                overlayService.confirmRemove({
                    title: data[0],
                    content: localizationService.tokenReplace(data[1], [block.view]),
                    close: function () {
                        overlayService.close();
                    },
                    submit: function () {
                        vm.removeViewForBlock(block);
                        overlayService.close();
                    }
                });
            });
        };
        vm.removeViewForBlock = function(block) {
            block.view = null;
        };



        vm.addThumbnailForBlock = function(block) {
            const thumbnailPicker = {
                title: "Select thumbnail (TODO need translation)",
                section: "settings",
                treeAlias: "files",
                entityType: "file",
                isDialog: true,
                filter: function (i) {
                    return (i.name.indexOf(".jpg") !== -1 || i.name.indexOf(".jpeg") !== -1 || i.name.indexOf(".png") !== -1 || i.name.indexOf(".svg") !== -1 || i.name.indexOf(".webp") !== -1 || i.name.indexOf(".gif") !== -1);
                },
                select: function (file) {
                    block.thumbnail = file.name;
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };
            editorService.treePicker(thumbnailPicker);
        }
        vm.removeThumbnailForBlock = function(entry) {
            entry.thumbnail = null;
        };




        vm.submit = function () {
            if ($scope.model && $scope.model.submit) {
                $scope.model.submit($scope.model);
            }
        }

        vm.close = function() {
            if ($scope.model && $scope.model.close) {
                // TODO: If content has changed, we should notify user.
                $scope.model.close($scope.model);
            }
        }

    }

    angular.module("umbraco").controller("Umbraco.PropertyEditors.BlockList.BlockConfigurationOverlayController", BlockConfigurationOverlayController);

})();
