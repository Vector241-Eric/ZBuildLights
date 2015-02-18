using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Services
{
    public interface IProjectManager
    {
        CreationResult<Project> Create(string name);
        EditResult<Project> Delete(Guid id);
        EditResult<Project> Update(Guid id, string name);
    }
}