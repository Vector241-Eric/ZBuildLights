using System;
using System.Linq;
using ZBuildLights.Core.Extensions;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        public MasterModel()
        {
            Projects = new Project[0];
        }

        public Project[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public Project AddProject(Project project)
        {
            Projects = Projects.AddToEnd(project);
            return project;
        }

        public bool ProjectExists(Guid id)
        {
            return Projects.Any(x => x.Id.Equals(id));
        }

        public void RemoveProject(Guid projectId)
        {
            Projects = Projects.Except(Projects.Where(x => x.Id.Equals(projectId))).ToArray();
        }

        public Light[] AllLights
        {
            get { return AllGroups.SelectMany(x => x.Lights).ToArray(); }
        }

        public LightGroup[] AllGroups
        {
            get { return Projects.SelectMany(x => x.Groups).ToArray(); }
        }

        public Light FindLight(uint homeId, byte deviceId)
        {
            var light = AllLights
                .SingleOrDefault(x => x.ZWaveHomeId.Equals(homeId) && x.ZWaveDeviceId.Equals(deviceId));
            if (light == null)
                throw new InvalidOperationException(
                    string.Format("Could not find light with homeId: {0} and deviceId: {1}", homeId, deviceId));
            return light;
        }

        public LightGroup FindGroup(Guid id)
        {
            var group = AllGroups.SingleOrDefault(x => x.Id.Equals(id));
            if (group == null)
                throw new InvalidOperationException(string.Format("Could not find group with id: {0}", id));
            return group;
        }
    }
}