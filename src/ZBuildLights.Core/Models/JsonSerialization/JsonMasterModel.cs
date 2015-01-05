using System;
using System.Linq;

namespace ZBuildLights.Core.Models.JsonSerialization
{
    public class JsonMasterModel
    {
        public JsonProject[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public JsonLight[] UnassignedLights { get; set; }

        public MasterModel ToDomainObject()
        {
            var masterModel = new MasterModel
            {
                LastUpdatedDate = LastUpdatedDate,
            };
            foreach (var jsonProject in Projects)
                masterModel.CreateProject(jsonProject.ToDomainObject());

            var unassignedLights = UnassignedLights ?? new JsonLight[0];

            masterModel.AddUnassignedLights(unassignedLights.Select(x => x.ToDomainObject()));
            return masterModel;
        }
    }

    public class JsonProject
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public JsonLightGroup[] Groups { get; set; }

        public Action<Project> ToDomainObject()
        {
            return p =>
            {
                p.Name = Name;
                p.Id = Id;
                p.AddGroups(Groups.Select(x => x.ToDomainObject()));
            };
        }
    }

    public class JsonLightGroup
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public JsonLight[] Lights { get; set; }

        public LightGroup ToDomainObject()
        {
            var group = new LightGroup
            {
                Id = Id,
                Name = Name,
            };
            group.AddLights(Lights.Select(x => x.ToDomainObject()));
            return group;
        }
    }

    public class JsonLight
    {
        public byte ZWaveDeviceId { get; set; }
        public uint ZWaveHomeId { get; set; }
        public int ColorValue { get; set; }

        public Light ToDomainObject()
        {
            var light = new Light(ZWaveHomeId, ZWaveDeviceId)
            {
                Color = LightColor.FromValue(ColorValue)
            };
            return light;
        }
    }
}