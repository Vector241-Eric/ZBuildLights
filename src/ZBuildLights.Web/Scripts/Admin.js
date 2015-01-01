var Admin = Admin || {};

Admin.urls = {
    addProject: '@Url.Action("AddProject")',
    deleteProject: '@Url.Action("DeleteProject")',
    updateProject: '@Url.Action("UpdateProject")',
    addGroup: '@Url.Action("AddGroup")',
    updateGroup: '@Url.Action("UpdateGroup")',
    deleteGroup: '@Url.Action("DeleteGroup")',
};

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

$(function () {
    Admin.collapse.attachHandlers();

    function handleError(data) {
        if (data.status == 500) {
            console.log(data.statusText);
            Admin.alert.show('Failed to add project. Please try again later.');
            return;
        }
        Admin.alert.show(data.responseJSON.Message);
    }

    //New Project Modal
    $('#save-new-project').click(function () {
        Admin.alert.close();
        var spinner = $('#add-project-modal .wait-spinner');
        spinner.show();
        var input = $('#project-name-input').val();
        $.post(Admin.urls.addProject, { projectName: input })
            .always(function () {
                $('#add-project-modal').modal('hide');
                spinner.hide();
            })
            .success(function () {
                location.reload();
            })
            .fail(handleError);
    });

    //Edit Project
    $('.btn-edit-project').click(function () {
        var projectId = $(this).attr('data-projectId');
        var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
        var projectName = projectPanel.attr('data-projectName');

        $('#edit-project-id').val(projectId);
        $('#edit-project-name-input').val(projectName);
        $('#edit-project-modal .delete-confirm').hide();
        $('#edit-project-modal .wait-spinner').hide();

        $('#edit-project-modal').modal('show');
    });

    $('.delete-item-link').click(function () {
        $('.delete-confirm').show();
    });

    $('.delete-reject-button').click(function () {
        $('.delete-confirm').hide();
    });

    $('#edit-project-modal .delete-confirm-link').click(function () {
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
            .fail(handleError);
    });

    $('#save-edit-project').click(function () {
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
            .fail(handleError);
    });

    //Add group
    $('.add-group-button').click(function () {
        var projectId = $(this).attr('data-projectId');
        var projectPanel = $('.admin-project-panel[data-projectId="' + projectId + '"]');
        var projectName = projectPanel.attr('data-projectName');

        $('.project-name').text(projectName);
        $('#save-new-group').attr('data-projectid', projectId);

        $('#add-group-modal').modal('show');
    });

    $('#save-new-group').click(function () {
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
            .fail(handleError);
    });

    //Edit group
    $('.btn-edit-group').click(function () {
        var groupId = $(this).data('groupid');
        var groupName = $(this).data('groupname');

        $('#edit-group-id').val(groupId);
        $('#edit-group-name-input').val(groupName);
        $('#edit-group-modal .delete-confirm').hide();
        $('#edit-group-modal .wait-spinner').hide();

        $('#edit-group-modal').modal('show');
    });

    $('#save-edit-group').click(function () {
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
            .fail(handleError);
    });

    $('#edit-group-modal .delete-confirm-link').click(function () {
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
            .fail(handleError);
    });
});