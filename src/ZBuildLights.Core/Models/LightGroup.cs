using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Models
{
    public class LightGroup
    {
        private readonly List<Light> _lights = new List<Light>();

        internal LightGroup(Project parentProject)
        {
            Name = string.Empty;
            ParentProject = parentProject;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public Project ParentProject { get; private set; }

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

        public MasterModel MasterModel { get { return ParentProject.MasterModel; } }

        public LightGroup AddLight(Light light)
        {
            if (this.Equals(light.ParentGroup))
                return this;

            if (light.ParentGroup != null)
                throw new InvalidOperationException(
                    string.Format("Cannot add light to group {0} because it already belongs to group {1}",
                        FullName, light.ParentGroup.FullName));
            _lights.Add(light);
            light.ParentGroup = this;
            return this;
        }

        public LightGroup AddLights(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                AddLight(light);
            return this;
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

        protected bool Equals(LightGroup other)
        {
            if (other == null)
                return false;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LightGroup) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}