using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class LightGroup
    {
        private readonly List<Light> _lights = new List<Light>();
        public string Name { get; set; }
        public Guid Id { get; internal set; }

        public Light[] Lights
        {
            get { return _lights.OrderBy(x => x.Color.DisplayOrder).ToArray(); }
        }

        public LightGroup AddLight(Light light)
        {
            _lights.Add(light);
            return this;
        }
    }
}