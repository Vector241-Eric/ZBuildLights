using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class UnassignedLightServiceTests
    {
        [TestFixture]
        public class When_some_lights_in_the_master_model_are_missing_from_the_network_and_some_lights_are_new_in_the_network :
                TestBase
        {
            private MasterModel _masterModel;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                var group = _masterModel.AddProject(new Project()).AddGroup(new LightGroup());
                group.AddLight(new Light(1, 11));
                group.AddLight(new Light(1, 22));
                group.AddLight(new Light(1, 33));

                _masterModel.AddUnassignedLight(new Light(1, 44){Color = LightColor.Red});

                var allSwitches = new[]
                {
                    new ZWaveSwitch {HomeId = 1, DeviceId = 11}, //In a group
                    new ZWaveSwitch {HomeId = 1, DeviceId = 15}, //New
                    new ZWaveSwitch {HomeId = 1, DeviceId = 16}, //New
                    new ZWaveSwitch {HomeId = 1, DeviceId = 22}, //In a group
                    new ZWaveSwitch {HomeId = 1, DeviceId = 33}, //In a group
                    new ZWaveSwitch {HomeId = 2, DeviceId = 11}, //New (different home ID)
                    new ZWaveSwitch {HomeId = 2, DeviceId = 22} //New (different home ID)
                };

                var network = S<IZWaveNetwork>();
                network.Stub(x => x.GetAllSwitches()).Return(allSwitches);

                var repository = S<IMasterModelRepository>();
                repository.Stub(x => x.GetCurrent()).Return(_masterModel);

                var service = new UnassignedLightService(network);
                service.SetUnassignedLights(_masterModel);
            }

            [Test]
            public void Should_find_the_new_lights_in_the_network()
            {
                _masterModel.UnassignedLights.Length.ShouldEqual(5);
                _masterModel.UnassignedLights.Any(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(15)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(16)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(44)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveHomeId.Equals(2) && x.ZWaveDeviceId.Equals(11)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveHomeId.Equals(2) && x.ZWaveDeviceId.Equals(22)).ShouldBeTrue();
            }

            [Test]
            public void Should_add_the_new_lights_without_overwriting_the_state_on_the_existing_lights()
            {
                var light = _masterModel.UnassignedLights.Single(x => x.ZWaveHomeId.Equals(1) && x.ZWaveDeviceId.Equals(44));
                light.Color.ShouldEqual(LightColor.Red);
            }
        }
    }
}