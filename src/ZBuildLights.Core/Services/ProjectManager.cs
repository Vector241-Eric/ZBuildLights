using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Services
{
    public class ProjectManager : IProjectManager
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public ProjectManager(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
        }

        public CreationResult<Project> CreateProject(string name)
        {
            var currentModel = _masterModelRepository.GetCurrent();
            if (IsProjectNameAlreadyUsed(name, currentModel))
                return CreationResult.Fail<Project>(NameCollisionMessage(name));

            var project = currentModel.CreateProject(x => { x.Name = name; });
            _masterModelRepository.Save(currentModel);

            return CreationResult.Success(project);
        }

        private static string NameCollisionMessage(string name)
        {
            return string.Format("There is already a project named '{0}'", name);
        }

        public EditResult<Project> DeleteProject(Guid id)
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

        public EditResult<Project> UpdateProject(Guid id, string name)
        {
            var currentModel = _masterModelRepository.GetCurrent();
            var project = currentModel.Projects.SingleOrDefault(x => x.Id.Equals(id));
            
            if (project == null)
                return CouldNotLocateProject(id);

            if (project.Name.Equals(name))
                return EditResult.Success(project);

            if (currentModel.Projects.Any(x => x.Name.Equals(name)))
                return EditResult.Fail<Project>(NameCollisionMessage(name));

            project.Name = name;
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