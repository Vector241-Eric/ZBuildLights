using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.Services.CruiseControl
{
    public interface ICcReader
    {
        NetworkResponse<Projects> GetStatus(string url);
    }
}