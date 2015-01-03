using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class UnassignedLightService : IUnassignedLightService
    {
        private readonly IMasterModelRepository _repository;
        private readonly IZWaveNetwork _network;

        public UnassignedLightService(IMasterModelRepository repository, IZWaveNetwork network)
        {
            _repository = repository;
            _network = network;
        }

        public LightGroup GetUnassignedLights()
        {
            var allLights = _repository.GetCurrent().AllLights;
            var allSwitches = _network.GetAllSwitches();

            var switchesAlreadyInAProject = allSwitches
                .Where(sw =>
                    allLights.Any(l =>
                        l.ZWaveDeviceId.Equals(sw.DeviceId) &&
                        l.ZWaveHomeId.Equals(sw.HomeId)
                        )
                ).ToArray();

            var newSwitches = allSwitches.Except(switchesAlreadyInAProject);
            var newLights = newSwitches.Select(x => new Light(x.HomeId, x.DeviceId)).ToArray();

            var lightGroup = new LightGroup {Name = "Unassigned"};
            lightGroup.AddLights(newLights);
            return lightGroup;
        }
    }
}