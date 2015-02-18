using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Services
{
    public interface ICruiseServerManager
    {
        CreationResult<CruiseServer> Create(string name, string url);
        EditResult<CruiseServer> Update(Guid id, string name, string url);
        EditResult<CruiseServer> Delete(Guid id);
    }
}