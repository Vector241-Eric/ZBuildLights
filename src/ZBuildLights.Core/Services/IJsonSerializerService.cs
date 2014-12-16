using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IJsonSerializerService
    {
        string SerializeMasterModel(MasterModel masterModel);
        MasterModel DeserializeMasterModel(string json);
    }
}