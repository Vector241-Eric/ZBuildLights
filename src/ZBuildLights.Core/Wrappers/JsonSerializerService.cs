using Newtonsoft.Json;

namespace ZBuildLights.Core.Wrappers
{
    public class JsonSerializerService
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}