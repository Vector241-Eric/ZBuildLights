using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class LightDisplayUpdater : ILightDisplayUpdater
    {
        private readonly IMasterModelRepository _repository;
        private readonly IZWaveNetwork _zWaveNetwork;
        private readonly IProjectStatusUpdater _projectStatusUpdater;

        public LightDisplayUpdater(IMasterModelRepository repository, IZWaveNetwork zWaveNetwork,
            IProjectStatusUpdater projectStatusUpdater)
        {
            _repository = repository;
            _zWaveNetwork = zWaveNetwork;
            _projectStatusUpdater = projectStatusUpdater;
        }

        public void Update()
        {
            var masterModel = _repository.GetCurrent();
            _projectStatusUpdater.UpdateAllProjectStatuses(masterModel);
            _repository.Save(masterModel);

            foreach (var project in masterModel.Projects)
            {
                var projectStatus = project.StatusMode.StatusLightConfiguration;
                foreach (var lightGroup in project.Groups)
                {
                    foreach (var light in lightGroup.Lights)
                    {
                        _zWaveNetwork.SetSwitchState(light.ZWaveIdentity, projectStatus[light.Color]);
                    }
                }
            }
        }
    }
}