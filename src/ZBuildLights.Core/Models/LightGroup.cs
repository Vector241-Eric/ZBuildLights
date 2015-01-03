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

        public string FullName
        {
            get
            {
                if (ParentProject == null)
                    return Name;
                return string.Format("{0}.{1}", ParentProject.Name, Name);
            }
        }

        public Light[] Lights
        {
            get { return _lights.OrderBy(x => x.Color.DisplayOrder).ToArray(); }
        }

        public LightGroup AddLight(Light light)
        {
            if (light.ParentGroup != null)
                throw new InvalidOperationException(
                    string.Format("Cannot add light to group {0} because it already belongs to group {1}",
                        FullName, light.ParentGroup.FullName));
            _lights.Add(light);
            light.ParentGroup = this;
            return this;
        }

        public void AddLights(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                AddLight(light);
        }

        public void RemoveLight(Light light)
        {
            if (light.ParentGroup != this)
                throw new InvalidOperationException(
                    string.Format("Cannot remove light from group {0} because it belongs to group {1}", FullName,
                        light.ParentGroup.FullName));
            _lights.Remove(light);
            light.ParentGroup = null;
        }
    }
}