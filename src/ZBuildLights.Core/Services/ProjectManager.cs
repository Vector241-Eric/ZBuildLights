using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public class ProjectManager : IProjectManager
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public ProjectManager(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
        }

        public CreationResult<Project> Create(EditProject editModel)
        {
            if (string.IsNullOrEmpty(editModel.Name))
                return CreationResult.Fail<Project>("Project name is required.");
            var currentModel = _masterModelRepository.GetCurrent();
            if (IsProjectNameAlreadyUsed(editModel.Name, currentModel))
                return CreationResult.Fail<Project>(NameCollisionMessage(editModel.Name));

            var project = currentModel.CreateProject(x =>
            {
                x.Name = editModel.Name;
                x.CruiseProjectAssociations =
                    editModel.SafeProjects.Select(
                        cp => new CruiseProjectAssociation {ServerId = cp.Server, Name = cp.Project})
                        .ToArray();
            });
            _masterModelRepository.Save(currentModel);

            return CreationResult.Success(project);
        }

        private static string NameCollisionMessage(string name)
        {
            return string.Format("There is already a project named '{0}'", name);
        }

        public EditResult<Project> Delete(Guid id)
        {
            var currentModel = _masterModelRepository.GetCurrent();
            if (!currentModel.ProjectExists(id))
                return CouldNotLocateProject(id);

            currentModel.RemoveProject(id);
            _masterModelRepository.Save(currentModel);
            return EditResult.Success<Project>(null);
        }

        private static EditResult<Project> CouldNotLocateProject(Guid id)
        {
            return EditResult.Fail<Project>(string.Format("Could not locate a project with Id '{0}'", id));
        }

        public EditResult<Project> Update(EditProject editModel)
        {
            var currentModel = _masterModelRepository.GetCurrent();
            var project = currentModel.Projects.SingleOrDefault(x => x.Id.Equals(editModel.ProjectId.Value));

            if (project == null)
                return CouldNotLocateProject(editModel.ProjectId.Value);

            var existingProjectWithThisName = currentModel.Projects.SingleOrDefault(x => x.Name.Equals(editModel.Name));
            if (existingProjectWithThisName != null && existingProjectWithThisName.Id != editModel.ProjectId)
                return EditResult.Fail<Project>(NameCollisionMessage(editModel.Name));

            project.Name = editModel.Name;
            if (editModel.CruiseProjects == null)
                project.CruiseProjectAssociations = new CruiseProjectAssociation[0];
            else
            {
                project.CruiseProjectAssociations = editModel.CruiseProjects
                    .Select(
                        cp => new CruiseProjectAssociation {Name = cp.Project, ServerId = cp.Server})
                    .ToArray();
            }
            _masterModelRepository.Save(currentModel);

            return EditResult.Success(project);
        }

        private bool IsProjectNameAlreadyUsed(string name, MasterModel currentModel)
        {
            var existingProjects = currentModel.Projects;
            return existingProjects.Any(x => x.Name.Equals(name));
        }
    }
}