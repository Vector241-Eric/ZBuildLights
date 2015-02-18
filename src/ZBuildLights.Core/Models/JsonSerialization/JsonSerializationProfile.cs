using AutoMapper;

namespace ZBuildLights.Core.Models.JsonSerialization
{
    public class JsonSerializationProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<MasterModel, JsonMasterModel>();
            CreateMap<Project, JsonProject>();
            CreateMap<CruiseServer, JsonCruiseServer>();
            CreateMap<LightGroup, JsonLightGroup>();
            CreateMap<Light, JsonLight>();
        }
    }
}