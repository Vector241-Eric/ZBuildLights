using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        //State
        private readonly List<Project> _projects = new List<Project>();

        public Project[] Projects
        {
            get { return _projects.ToArray(); }
        }

        public DateTime LastUpdatedDate { get; set; }

        public Light[] UnassignedLights
        {
            get { return _unassignedLights.ToArray(); }
        }

        private readonly HashSet<Light> _unassignedLights = new HashSet<Light>();

        //Methods
        public bool ProjectExists(Guid id)
        {
            return Projects.Any(x => x.Id.Equals(id));
        }

        public void RemoveProject(Guid projectId)
        {
            _projects.RemoveAll(x => x.Id.Equals(projectId));
        }

        public Light[] AllLights
        {
            get { return AllGroups.SelectMany(x => x.Lights).Union(UnassignedLights).ToArray(); }
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

        public void AddUnassignedLights(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                AddUnassignedLight(light);
        }

        public void AddUnassignedLight(Light light)
        {
            _unassignedLights.Add(light);
        }

        public void AssignLightToGroup(uint homeId, byte deviceId, Guid groupId)
        {
            var light = FindLight(homeId, deviceId);
            if (light.IsInGroup)
                light.Unassign();
            else
                _unassignedLights.Remove(light);
            FindGroup(groupId).AddLight(light);
        }

        public Project CreateProject(Action<Project> initialize = null)
        {
            var init = initialize ?? (x => { });
            var project = new Project(this);
            init(project);
            if (project.Id == Guid.Empty)
                project.Id = Guid.NewGuid();

            _projects.Add(project);
            return project;
        }
    }
}