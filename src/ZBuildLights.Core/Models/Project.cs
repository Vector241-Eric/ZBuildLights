using System;
using System.Collections.Generic;
using ZBuildLights.Core.Enumerations;

namespace ZBuildLights.Core.Models
{
    public class Project
    {
        public readonly List<LightGroup> _groups = new List<LightGroup>();

        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }
        public Guid Id { get; internal set; }
        public MasterModel MasterModel { get; private set; }

        public Project(MasterModel masterModel)
        {
            MasterModel = masterModel;
            StatusMode = StatusMode.NotConnected;
        }

        public LightGroup[] Groups
        {
            get { return _groups.ToArray(); }
        }

        public LightGroup AddGroup(LightGroup group)
        {
            _groups.Add(group);
            group.ParentProject = this;
            return group;
        }

        public void AddGroups(IEnumerable<LightGroup> groups)
        {
            foreach (var group in groups)
                this.AddGroup(group);
        }

        public void RemoveGroup(LightGroup lightGroup)
        {
            _groups.Remove(lightGroup);
        }
    }
}