var Admin = Admin || {};

Admin.alert = {
    show: function(message) {
        var template = $(document.getElementById('alert-template').innerHTML);
        template.find('[data-purpose="error-message"]').text(message);
        $('#alert-container').append(template);
    },

    close: function() {
        $('#alert-container').find('[data-purpose="error-alert"]').remove();
    }
}

Admin.collapse = {
    onShow: function() {
        var headingId = $(this).attr('data-panelheading');
        $('#' + headingId + ' .heading-icon').removeClass('fa-plus-circle').addClass('fa-minus-circle');
    },

    onCollapse: function() {
        var headingId = $(this).attr('data-panelheading');
        $('#' + headingId + ' .heading-icon').removeClass('fa-minus-circle').addClass('fa-plus-circle');
    },

    attachHandlers: function() {
        $('.collapse').on('show.bs.collapse', Admin.collapse.onShow);
        $('.collapse').on('hide.bs.collapse', Admin.collapse.onCollapse);
    }
}

Admin.networkState = {
    handleError: function (data) {
        if (data.status == 500) {
            console.log(data.statusText);
            Admin.alert.show('Failed to add project. Please try again later.');
            return;
        }
        Admin.alert.show(data.responseJSON.Message);
    }
}

Admin.project = {
    edit: function () {
        var projectId = $(this).attr('data-projectId');
        var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
        var projectName = projectPanel.attr('data-projectName');

        $('#edit-project-id').val(projectId);
        $('#edit-project-name-input').val(projectName);
        $('#edit-project-modal .delete-confirm').hide();
        $('#edit-project-modal .wait-spinner').hide();

        $('#edit-project-modal').modal('show');
    },

    saveNew: function() {
        Admin.alert.close();
        var spinner = $('#add-project-modal .wait-spinner');
        spinner.show();
        var input = $('#project-name-input').val();
        $.post(Admin.urls.addProject, { projectName: input })
            .always(function() {
                $('#add-project-modal').modal('hide');
                spinner.hide();
            })
            .success(function() {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    postDelete: function () {
        var spinner = $('#edit-project-modal .wait-spinner');
        spinner.show();
        var projectId = $('#edit-project-id').val();
        $.post(Admin.urls.deleteProject, { projectId: projectId })
            .always(function () {
                $('#edit-project-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    postEdits: function () {
        var projectId = $('#edit-project-id').val();
        var name = $('#edit-project-name-input').val();
        var spinner = $('#edit-project-modal .wait-spinner');
        spinner.show();
        $.post(Admin.urls.updateProject, { projectId: projectId, name: name })
            .always(function () {
                $('#edit-project-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    deleteConfirmation: {
        show: function () {
            $('.delete-project-confirm').show();
        },
        hide: function() {
            $('.delete-project-confirm').hide();
        }
    },

    attachHandlers: function() {
        $('#save-new-project').click(saveNew);
        $('.btn-edit-project').click(edit);
        $('.delete-project-link').click(deleteConfirmation.show);
        $('.delete-project-reject-button').click(deleteConfirmation.hide);
        $('#edit-project-modal .delete-confirm-link').click(postDelete);
        $('#save-edit-project').click(postEdits);
    }
};

Admin.group = {
    deleteConfirmation: {
        show: function () {
            $('.delete-group-confirm').show();
        },
        hide: function () {
            $('.delete-group-confirm').hide();
        }
    },

    add: function () {
        var projectId = $(this).attr('data-projectId');
        var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
        var projectName = projectPanel.attr('data-projectName');

        $('.project-name').text(projectName);
        $('#save-new-group').attr('data-projectid', projectId);

        $('#add-group-modal').modal('show');
    },

    saveNew: function () {
        var groupName = $('#group-name-input').val();
        var projectId = $(this).attr('data-projectId');
        var spinner = $('#add-group-modal .wait-spinner');
        spinner.show();
        $.post(Admin.urls.addGroup, { groupName: groupName, projectId: projectId })
            .always(function () {
                $('#add-group-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    edit: function () {
        var groupId = $(this).data('groupid');
        var groupName = $(this).data('groupname');

        $('#edit-group-id').val(groupId);
        $('#edit-group-name-input').val(groupName);
        $('#edit-group-modal .delete-confirm').hide();
        $('#edit-group-modal .wait-spinner').hide();

        $('#edit-group-modal').modal('show');
    },

    postEdit: function () {
        var groupId = $('#edit-group-id').val();
        var name = $('#edit-group-name-input').val();
        var spinner = $('#edit-group-modal .wait-spinner');
        spinner.show();
        $.post(Admin.urls.updateGroup, { groupId: groupId, name: name })
            .always(function () {
                $('#edit-group-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    postDelete: function () {
        var spinner = $('#edit-group-modal .wait-spinner');
        spinner.show();
        var groupId = $('#edit-group-id').val();
        $.post(Admin.urls.deleteGroup, { groupId: groupId })
            .always(function () {
                $('#edit-group-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(Admin.networkState.handleError);
    },

    attachHandlers: function() {
        $('.delete-group-reject-button').click(deleteConfirmation.hide);
        $('.delete-group-link').click(deleteConfirmation.show);
        $('.add-group-button').click(add);
        $('#save-new-group').click(saveNew);
        $('.btn-edit-group').click(edit);
        $('#save-edit-group').click(postEdit);
        $('#edit-group-modal .delete-confirm-link').click(postDelete);
    }
};

$(function() {
    Admin.collapse.attachHandlers();
    Admin.project.attachHandlers();
    Admin.group.attachHandlers();
});