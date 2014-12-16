using AutoMapper;
using ZBuildLights.Core.Models.JsonSerialization;
using ZBuildLights.Web.Mapping;

namespace ZBuildLights.Web
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ViewModelProfile>();
                cfg.AddProfile<JsonSerializationProfile>();
            });
        }
    }
}