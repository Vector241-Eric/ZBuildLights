using System.Collections.Generic;

namespace ZBuildLights.Core.Models
{
    public class Project
    {
        public readonly List<LightGroup> _groups = new List<LightGroup>();

        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }

        public LightGroup[] Groups
        {
            get { return _groups.ToArray(); }
        }

        public LightGroup AddGroup(LightGroup group)
        {
            _groups.Add(group);
            return group;
        }
    }
}