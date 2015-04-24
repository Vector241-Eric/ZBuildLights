using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class MasterModel
    {
        //State
        private readonly List<Project> _projects = new List<Project>();
        private readonly List<CruiseServer> _cruiseServers = new List<CruiseServer>();

        public Project[] Projects
        {
            get { return _projects.ToArray(); }
        }

        public CruiseServer[] CruiseServers
        {
            get { return _cruiseServers.ToArray(); }
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
            var toRemove = _projects.SingleOrDefault(x => x.Id.Equals(projectId));
            if (toRemove == null)
                return;

            foreach (var group in toRemove.Groups)
                group.UnassignAllLights();
            _projects.Remove(toRemove);
        }

        public Light[] AllLights
        {
            get { return AllGroups.SelectMany(x => x.Lights).Union(UnassignedLights).ToArray(); }
        }

        public LightGroup[] AllGroups
        {
            get { return Projects.SelectMany(x => x.Groups).ToArray(); }
        }

        public Light FindLight(ZWaveValueIdentity identity)
        {
            var light = AllLights.SingleOrDefault(x => x.ZWaveIdentity.Equals(identity));
            if (light == null)
                throw new InvalidOperationException(string.Format("Could not find light with identity: {0}", identity));
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

        public void AssignLightToGroup(ZWaveValueIdentity identity, Guid groupId)
        {
            var light = FindLight(identity);
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

        public CruiseServer CreateCruiseServer(Action<CruiseServer> initialize = null)
        {
            var init = initialize ?? (x => { });
            var cruiseServer = new CruiseServer();
            init(cruiseServer);
            if (cruiseServer.Id == Guid.Empty)
                cruiseServer.Id = Guid.NewGuid();

            _cruiseServers.Add(cruiseServer);
            return cruiseServer;
        }

        public void RemoveCruiseServer(Guid cruiseServerId)
        {
            _cruiseServers.RemoveAll(x => x.Id.Equals(cruiseServerId));
        }
    }
}