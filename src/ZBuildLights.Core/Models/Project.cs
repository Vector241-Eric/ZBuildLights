using System;
using System.Linq;
using ZBuildLights.Core.Enumerations;

namespace ZBuildLights.Core.Models
{
    public class Project
    {
        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }
        public Guid Id { get; internal set; }

        public LightGroup[] Groups { get; set; }

        public LightGroup AddGroup(LightGroup group)
        {
            Groups = Groups.Union(new[] {group}).ToArray();
            return group;
        }
    }
}