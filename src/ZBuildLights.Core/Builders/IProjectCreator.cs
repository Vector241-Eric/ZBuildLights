using ZBuildLights.Core.Models;
using ZBuildLights.Core.Validation;

namespace ZBuildLights.Core.Builders
{
    public interface IProjectCreator
    {
        CreationResult<Project> CreateProject(string name);
    }
}