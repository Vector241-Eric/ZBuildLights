using System;
using System.Linq;

namespace ZBuildLights.Core.Models.JsonSerialization
{
    public class JsonMasterModel
    {
        public JsonMasterModel()
        {
            Projects = new JsonProject[0];
            CruiseServers = new JsonCruiseServer[0];
            UnassignedLights = new JsonLight[0];
        }

        public JsonProject[] Projects { get; set; }
        public JsonCruiseServer[] CruiseServers { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public JsonLight[] UnassignedLights { get; set; }

        public MasterModel ToDomainObject()
        {
            var masterModel = new MasterModel
            {
                LastUpdatedDate = LastUpdatedDate,
            };
            foreach (var jsonProject in Projects)
                masterModel.CreateProject(jsonProject.InitializeDomainObject());
            foreach (var jsonCruiseServer in CruiseServers)
                masterModel.CreateCruiseServer(jsonCruiseServer.InitializeDomainObject());

            var unassignedLights = UnassignedLights ?? new JsonLight[0];

            masterModel.AddUnassignedLights(unassignedLights.Select(x => x.ToDomainObject()));
            return masterModel;
        }
    }

    public class JsonCruiseServer
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public Action<CruiseServer> InitializeDomainObject()
        {
            return srv =>
            {
                srv.Url = Url;
                srv.Id = Id;
                srv.Name = Name;
            };
        }
    }

    public class JsonProject
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public JsonLightGroup[] Groups { get; set; }
        public string CcXmlUrl { get; set; }
        public JsonCruiseProject[] CruiseProjects { get; set; }

        public Action<Project> InitializeDomainObject()
        {
            return p =>
            {
                p.Name = Name;
                p.Id = Id;
                p.CcXmlUrl = CcXmlUrl;
                var mappedCruiseProjects = (CruiseProjects ?? new JsonCruiseProject[0]).Select(cp => cp.BuildDomainObject()).ToArray();
                p.CruiseProjects = mappedCruiseProjects;
                foreach (var jsonGroup in Groups)
                    p.CreateGroup(jsonGroup.InitializeDomainObject());
            };
        }
    }

    public class JsonCruiseProject
    {
        public Guid ServerId { get; set; }
        public string ProjectName { get; set; }

        public CruiseProject BuildDomainObject()
        {
            return new CruiseProject
            {
                ProjectName = ProjectName,
                ServerId = ServerId,
            };
        }
    }

    public class JsonLightGroup
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public JsonLight[] Lights { get; set; }

        public Action<LightGroup> InitializeDomainObject()
        {
            return g =>
            {
                g.Id = Id;
                g.Name = Name;
                g.AddLights(Lights.Select(x => x.ToDomainObject()));
            };
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