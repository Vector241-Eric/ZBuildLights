/// <reference path="~/Scripts/jquery-1.10.2.js" />
/// <reference path="~/Scripts/jquery-1.10.2.intellisense.js" />
/// <reference path="~/Scripts/bootstrap.js" />
/// <reference path="~/Scripts/App/namespace.js" />

(function() {
    ZBuildLights.createNamespace('Admin');
    var Admin = ZBuildLights.Admin;

    Admin.Alert = (function() {
        var show = function(message) {
            var template = $(document.getElementById('alert-template').innerHTML);
            template.find('[data-purpose="error-message"]').text(message);
            $('#alert-container').append(template);
        };

        var close = function() {
            $('#alert-container').find('[data-purpose="error-alert"]').remove();
        };

        return {
            show: show,
            close: close
        }
    })();

    Admin.Collapse = (function() {
        var onShow = function() {
            var headingId = $(this).attr('data-panelheading');
            $('#' + headingId + ' .heading-icon').removeClass('fa-plus-circle').addClass('fa-minus-circle');
        };


        var onCollapse = function() {
            var headingId = $(this).attr('data-panelheading');
            $('#' + headingId + ' .heading-icon').removeClass('fa-minus-circle').addClass('fa-plus-circle');
        };

        var attachHandlers = function() {
            $('.collapse').on('show.bs.collapse', onShow);
            $('.collapse').on('hide.bs.collapse', onCollapse);
        };

        return {
            attachHandlers: attachHandlers
        }
    })();

    Admin.Error = (function() {
        var handle = function(data, message) {
            if (data.status == 500) {
                console.log(data.statusText);
                Admin.Alert.show('500 error: ' + message);
                return;
            }
            Admin.Alert.show(data.responseJSON.Message);
        };
        
        var handleWrapper = function(failureMessage) {
            var handleCore = function(data) {
                handle(data, failureMessage);
            }

            return handleCore;
        }

        return {
            handle: handleWrapper
        }
    })();

    Admin.Project = (function() {
        var edit = function() {
            var projectId = $(this).attr('data-projectId');
            var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
            var projectName = projectPanel.attr('data-projectName');

            $('#edit-project-id').val(projectId);
            $('#edit-project-name-input').val(projectName);
            $('#edit-project-modal .delete-confirm').hide();
            $('#edit-project-modal .wait-spinner').hide();

            $('#edit-project-modal').modal('show');
        };

        var saveNew = function() {
            Admin.Alert.close();
            var spinner = $('#add-project-modal .wait-spinner');
            spinner.show();
            var input = $('#project-name-input').val();
            $.post(Admin.Urls.addProject, { projectName: input })
                .always(function() {
                    $('#add-project-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to save new project.'));
        };

        var postDelete = function() {
            var spinner = $('#edit-project-modal .wait-spinner');
            spinner.show();
            var projectId = $('#edit-project-id').val();
            $.post(Admin.Urls.deleteProject, { projectId: projectId })
                .always(function() {
                    $('#edit-project-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to delete project.'));
        };

        var postEdits = function() {
            var projectId = $('#edit-project-id').val();
            var name = $('#edit-project-name-input').val();
            var spinner = $('#edit-project-modal .wait-spinner');
            spinner.show();
            $.post(Admin.Urls.updateProject, { projectId: projectId, name: name })
                .always(function() {
                    $('#edit-project-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to edit project.'));
        };

        var deleteConfirmation = {
            show: function() {
                $('.delete-project-confirm').show();
            },
            hide: function() {
                $('.delete-project-confirm').hide();
            }
        };

        var refreshCcProjects = function() {
            var url = $('#project-ccurl-input').val();
            $.getJSON(ZBuildLights.Admin.Urls.ccJson, {url: url})
                .success(function(data) {
                console.log(data);
            });
        }

        var attachHandlers = function() {
            $('#save-new-project').click(saveNew);
            $('.btn-edit-project').click(edit);
            $('.delete-project-link').click(deleteConfirmation.show);
            $('.delete-project-reject-button').click(deleteConfirmation.hide);
            $('#edit-project-modal .delete-confirm-link').click(postDelete);
            $('#save-edit-project').click(postEdits);
            $('#button-refresh-cc-projects').click(refreshCcProjects);
        };

        return {
            attachHandlers: attachHandlers,
        }
    })();

    Admin.Group = (function() {
        var deleteConfirmation = {
            show: function() {
                $('.delete-group-confirm').show();
            },
            hide: function() {
                $('.delete-group-confirm').hide();
            }
        };

        var add = function() {
            var projectId = $(this).attr('data-projectId');
            var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
            var projectName = projectPanel.attr('data-projectName');

            $('.project-name').text(projectName);
            $('#save-new-group').attr('data-projectid', projectId);

            $('#add-group-modal').modal('show');
        };

        var saveNew = function() {
            var groupName = $('#group-name-input').val();
            var projectId = $(this).attr('data-projectId');
            var spinner = $('#add-group-modal .wait-spinner');
            spinner.show();
            $.post(Admin.Urls.addGroup, { groupName: groupName, projectId: projectId })
                .always(function() {
                    $('#add-group-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to create new light group.'));
        };

        var edit = function() {
            var groupId = $(this).data('groupid');
            var groupName = $(this).data('groupname');

            $('#edit-group-id').val(groupId);
            $('#edit-group-name-input').val(groupName);
            $('#edit-group-modal .delete-confirm').hide();
            $('#edit-group-modal .wait-spinner').hide();

            $('#edit-group-modal').modal('show');
        };

        var postEdit = function() {
            var groupId = $('#edit-group-id').val();
            var name = $('#edit-group-name-input').val();
            var spinner = $('#edit-group-modal .wait-spinner');
            spinner.show();
            $.post(Admin.Urls.updateGroup, { groupId: groupId, name: name })
                .always(function() {
                    $('#edit-group-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to edit light group.'));
        };

        var postDelete = function() {
            var spinner = $('#edit-group-modal .wait-spinner');
            spinner.show();
            var groupId = $('#edit-group-id').val();
            $.post(Admin.Urls.deleteGroup, { groupId: groupId })
                .always(function() {
                    $('#edit-group-modal').modal('hide');
                    spinner.hide();
                })
                .success(function() {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to delete light group.'));
        };

        var attachHandlers = function() {
            $('.delete-group-reject-button').click(deleteConfirmation.hide);
            $('.delete-group-link').click(deleteConfirmation.show);
            $('.add-group-button').click(add);
            $('#save-new-group').click(saveNew);
            $('.btn-edit-group').click(edit);
            $('#save-edit-group').click(postEdit);
            $('#edit-group-modal .delete-confirm-link').click(postDelete);
        };

        return {
            attachHandlers: attachHandlers
        }
    })();

    Admin.Light = (function () {

        var postEdit = function () {
            var spinner = $('#edit-light-modal .wait-spinner');
            spinner.show();

            var homeId = $('#edit-light-homeid').val();
            var deviceId = $('#edit-light-deviceid').val();
            var groupId = $('#select-light-group').val();
            var colorId = $('#select-light-color').val();

            $.post(Admin.Urls.editLight, { homeId: homeId, deviceId: deviceId, groupId: groupId, colorId: colorId })
                .always(function () {
                    $('#edit-light-modal').modal('hide');
                    spinner.hide();
                })
                .success(function () {
                    location.reload();
                })
                .fail(Admin.Error.handle('Failed to edit light.'));
        };

        var editLight = function() {
            var container = $(this).parents('.zwaveLight');

            var homeId = container.data('homeid');
            var deviceId = container.data('deviceid');
            var group = container.data('group');
            var color = container.data('color');

            $('#edit-light-homeid').val(homeId);
            $('#edit-light-deviceid').val(deviceId);
            selectColor(color);
            $('#select-light-group').val(group);
            $('#edit-light-modal').modal('show');
        };

        function selectColor(colorId) {
            $('#select-light-color').val(colorId);
            adjustColorSelectStylesBasedOnValue();
        }

        var adjustColorSelectStylesBasedOnValue = function () {
            var prefix = ZBuildLights.Admin.Constants.lightOptionPrefix;

            var el = $('#select-light-color');
            var selectElement = el[0];
            var selectedOption = selectElement[selectElement.selectedIndex];
            var cssToAdd = selectedOption.getAttribute('data-optioncss');
            el.removeClass(function (index, cssClasses) {
                var splits = cssClasses.split(' ');
                var classesToRemove = [];
                for (var i = 0; i < splits.length; i++) {
                    var css = splits[i];
                    if (css.match('^' + prefix)) {
                        classesToRemove.push(css);
                    }
                }
                var removeClassesString = classesToRemove.join(' ');
                return removeClassesString;
            });
            el.addClass(cssToAdd);
        }

        var attachHandlers = function() {
            $('#save-light').click(postEdit);
            $('.edit-light-link').click(editLight);
            $('#select-light-color').change(adjustColorSelectStylesBasedOnValue);
        };

        return {
            attachHandlers: attachHandlers
        }
    })();
})();

$(function() {
    ZBuildLights.Admin.Collapse.attachHandlers();
    ZBuildLights.Admin.Project.attachHandlers();
    ZBuildLights.Admin.Group.attachHandlers();
    ZBuildLights.Admin.Light.attachHandlers();
});