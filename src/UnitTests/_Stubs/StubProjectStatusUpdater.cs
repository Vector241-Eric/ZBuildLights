using System.Collections.Generic;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests._Stubs
{
    public class StubProjectStatusUpdater : IProjectStatusUpdater
    {
        private readonly Dictionary<Project, StatusMode> _stubStatus = new Dictionary<Project, StatusMode>();

        public StubProjectStatusUpdater WithStubStatus(Project project, StatusMode statusMode)
        {
            _stubStatus[project] = statusMode;
            return this;
        }

        public void UpdateAllProjectStatuses(MasterModel model)
        {
            foreach (var project in model.Projects)
            {
                project.StatusMode = _stubStatus[project];
            }
        }
    }
}