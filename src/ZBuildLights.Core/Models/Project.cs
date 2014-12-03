using System.Collections.Generic;
using System.Linq;
using BuildLightControl;

namespace ZBuildLights.Core.Models
{
    public class Project
    {
        private readonly List<SwitchableLight> _lights = new List<SwitchableLight>();
        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }

        public SwitchableLight[] SwitchableLights
        {
            get { return _lights.OrderBy(x => x.Color.DisplayOrder).ToArray(); }
        }

        public Project AddLight(SwitchableLight switchableLight)
        {
            _lights.Add(switchableLight);
            return this;
        }
    }

    public class Light
    {
        public SwitchableLight SwitchableLight { get; set; }
        public string Description { get; set; }
    }
}