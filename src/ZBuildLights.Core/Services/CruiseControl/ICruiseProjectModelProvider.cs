using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.Services.CruiseControl
{
    public interface ICruiseProjectModelProvider
    {
        NetworkRequest<CcProjectCollectionViewModel> GetProjects(Guid serverId);
    }
}