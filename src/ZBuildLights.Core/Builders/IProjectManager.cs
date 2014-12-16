using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Builders
{
    public interface IProjectManager
    {
        CreationResult<Project> CreateProject(string name);
        EditResult<Project> DeleteProject(Guid id);
        EditResult<Project> UpdateProject(Guid id, string name);
    }
}