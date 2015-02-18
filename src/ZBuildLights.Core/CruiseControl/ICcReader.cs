using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.CruiseControl
{
    public interface ICcReader
    {
        NetworkRequest<Projects> GetStatus(string url);
    }
}