using System;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class LightGroup
    {
        public string Name { get; set; }
        public Guid Id { get; internal set; }

        public Light[] Lights { get; set; }

        public LightGroup AddLight(Light light)
        {
            Lights = Lights.Union(new[] {light}).ToArray();
            return this;
        }
    }
}