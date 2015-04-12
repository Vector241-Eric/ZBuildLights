using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public interface ILightGroupManager
    {
        CreationResult<LightGroup> Create(Guid projectId, string name);
        EditResult<LightGroup> Update(Guid groupId, string name);
        EditResult<LightGroup> Delete(Guid groupId);
    }
}