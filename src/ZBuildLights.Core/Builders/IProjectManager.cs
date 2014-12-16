using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Builders
{
    public interface IProjectManager
    {
        CreationResult<Project> CreateProject(string name);
        void DeleteProject(Guid id);
    }
}