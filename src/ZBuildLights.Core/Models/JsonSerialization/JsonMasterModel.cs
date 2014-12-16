using System;
using System.Linq;
using ZBuildLights.Core.Enumerations;

namespace ZBuildLights.Core.Models.JsonSerialization
{
    public class JsonMasterModel
    {
        public JsonProject[] Projects { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public MasterModel ToDomainObject()
        {
            return new MasterModel
            {
                LastUpdatedDate = LastUpdatedDate,
                Projects = Projects.Select(x => x.ToDomainObject()).ToArray()
            };
        }
    }

    public class JsonProject
    {
        public string Name { get; set; }
        public string StatusModeValue { get; set; }
        public Guid Id { get; set; }
        public JsonLightGroup[] Groups { get; set; }

        public Project ToDomainObject()
        {
            var project = new Project
            {
                Name = Name,
                StatusMode = StatusMode.Parse(StatusModeValue),
                Id = Id,
            };
            project.AddGroups(Groups.Select(x => x.ToDomainObject()));
            return project;
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
        public Guid Id { get; set; }
        public int ColorValue { get; set; }

        public Light ToDomainObject()
        {
            var light = new Light(ZWaveHomeId, ZWaveDeviceId)
            {
                Id = Id,
                Color = LightColor.FromValue(ColorValue)
            };
            return light;
        }
    }
}