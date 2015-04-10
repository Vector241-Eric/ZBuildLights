using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.CruiseControl
{
    public interface ICcReader
    {
        NetworkResponse<Projects> GetStatus(string url);
    }
}