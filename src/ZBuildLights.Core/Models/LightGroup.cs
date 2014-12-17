using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class LightGroup
    {
        private readonly List<Light> _lights = new List<Light>();

        public LightGroup()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
        public Guid Id { get; internal set; }
        public Project ParentProject { get; set; }

        public Light[] Lights
        {
            get { return _lights.OrderBy(x => x.Color.DisplayOrder).ToArray(); }
        }

        public LightGroup AddLight(Light light)
        {
            _lights.Add(light);
            return this;
        }

        public void AddLights(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                this.AddLight(light);
        }
    }
}