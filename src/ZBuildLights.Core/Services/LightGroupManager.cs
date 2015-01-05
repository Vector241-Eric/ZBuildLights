using System;
using System.Linq;
using ZBuildLights.Core.Extensions;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Services
{
    public class LightGroupManager : ILightGroupManager
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public LightGroupManager(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
        }

        public CreationResult<LightGroup> CreateLightGroup(Guid projectId, string name)
        {
            var masterModel = _masterModelRepository.GetCurrent();
            if (masterModel.Projects.None(x => x.Id.Equals(projectId)))
                return
                    CreationResult.Fail<LightGroup>(
                        string.Format("Cannot create group for project '{0}' that doesn't exist", projectId));

            var project = masterModel.Projects.Single(x => x.Id.Equals(projectId));
            if (project.Groups.Any(x => x.Name.Equals(name)))
                return CreationResult.Fail<LightGroup>("A group with this name already exists");

            var group = project.CreateGroup(x => x.Name = name);
            _masterModelRepository.Save(masterModel);

            return CreationResult.Success(group);
        }

        public EditResult<LightGroup> UpdateLightGroup(Guid groupId, string name)
        {
            var masterModel = _masterModelRepository.GetCurrent();
            var allGroups = masterModel.Projects.SelectMany(x => x.Groups).ToArray();
            if (allGroups.None(x => x.Id.Equals(groupId)))
                return EditResult.Fail<LightGroup>(BadId(groupId));

            var group = allGroups.Single(x => x.Id.Equals(groupId));
            if (group.Name.Equals(name))
                return EditResult.Success(group);

            var parentProject = group.ParentProject;
            if (parentProject.Groups.Any(x => x.Name.Equals(name)))
                return
                    EditResult.Fail<LightGroup>(string.Format("There is already a group named '{0}' in project '{1}'",
                        name, parentProject.Name));

            group.Name = name;
            _masterModelRepository.Save(masterModel);

            return EditResult.Success(group);
        }

        private static string BadId(Guid groupId)
        {
            return string.Format("Could not locate a light group with ID '{0}'", groupId);
        }

        public EditResult<LightGroup> DeleteLightGroup(Guid groupId)
        {
            var masterModel = _masterModelRepository.GetCurrent();
            var group = masterModel.Projects
                .SelectMany(x => x.Groups)
                .SingleOrDefault(x => x.Id.Equals(groupId));

            if (group == null)
                return EditResult.Fail<LightGroup>(BadId(groupId));
            
            group.UnassignAllLights();

            var parent = group.ParentProject;
            parent.RemoveGroup(group);

            _masterModelRepository.Save(masterModel);
            return EditResult.Success<LightGroup>(null);
        }
    }
}