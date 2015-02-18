using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.Requests;

namespace ZBuildLights.Core.Services.CruiseControl
{
    public interface ICruiseProjectModelProvider
    {
        NetworkRequest<CcProjectCollection> GetProjects(Guid serverId);
    }
}