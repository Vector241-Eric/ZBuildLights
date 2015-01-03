using Newtonsoft.Json;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.JsonSerialization;
using ZBuildLights.Core.Wrappers;

namespace ZBuildLights.Core.Services.Storage
{
    public class JsonSerializerService : IJsonSerializerService
    {
        private readonly IMapper _mapper;

        public JsonSerializerService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public string SerializeMasterModel(MasterModel masterModel)
        {
            var jsonModel = _mapper.Map<MasterModel, JsonMasterModel>(masterModel);
            return JsonConvert.SerializeObject(jsonModel, Formatting.Indented);
        }

        public MasterModel DeserializeMasterModel(string json)
        {
            var jsonMasterModel = JsonConvert.DeserializeObject<JsonMasterModel>(json);
            return jsonMasterModel.ToDomainObject();
        }
    }
}