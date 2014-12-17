using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Builders
{
    public interface ILightGroupManager
    {
        CreationResult<LightGroup> CreateLightGroup(Guid projectId, string name);
        EditResult<LightGroup> UpdateLightGroup(Guid groupId, string name);
        EditResult<LightGroup> DeleteLightGroup(Guid groupId);
    }
}