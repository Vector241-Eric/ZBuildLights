using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Builders
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
                return CreationResult.Fail<Project>(string.Format("There is already a project named '{0}'", name));

            var project = InitializeProject(name);
            currentModel.AddProject(project);

            _masterModelRepository.Save(currentModel);

            return CreationResult.Success(project);
        }

        public void DeleteProject(Guid id)
        {
            var currentModel = _masterModelRepository.GetCurrent();
            currentModel.RemoveProject(id);
            _masterModelRepository.Save(currentModel);
        }

        private bool IsProjectNameAlreadyUsed(string name, MasterModel currentModel)
        {
            var existingProjects = currentModel.Projects;
            return existingProjects.Any(x => x.Name.Equals(name));
        }

        private static Project InitializeProject(string name)
        {
            var project = new Project
            {
                Name = name,
                Id = Guid.NewGuid(),
            };
            return project;
        }
    }
}