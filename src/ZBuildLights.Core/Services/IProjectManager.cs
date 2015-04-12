using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public interface IProjectManager
    {
        CreationResult<Project> Create(EditProject editModel);
        EditResult<Project> Delete(Guid id);
        EditResult<Project> Update(EditProject editModel);
    }
}