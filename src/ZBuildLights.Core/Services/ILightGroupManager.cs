using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Services
{
    public interface ILightGroupManager
    {
        CreationResult<LightGroup> Create(Guid projectId, string name);
        EditResult<LightGroup> Update(Guid groupId, string name);
        EditResult<LightGroup> Delete(Guid groupId);
    }
}