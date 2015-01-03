using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class SystemStatusProviderTests
    {
        [TestFixture]
        public class Always : TestBase
        {
            private MasterModel _model;
            private SystemStatusModel _result;
            private StubLightStatusSetter _lightStatusSetter;

            [SetUp]
            public void ContextSetup()
            {
                var repo = S<IMasterModelRepository>();
                _model = new MasterModel();
                var group = _model.AddProject(new Project()).AddGroup(new LightGroup());
                group.AddLight(new Light(3, 11));
                group.AddLight(new Light(3, 22));
                group.AddLight(new Light(3, 33));
                group.AddLight(new Light(3, 44));
                repo.Stub(x => x.GetCurrent()).Return(_model);

                _lightStatusSetter = new StubLightStatusSetter().StubStatus(SwitchState.On);

                var lightAssignmentService = S<IUnassignedLightService>();
                lightAssignmentService.Stub(x => x.GetUnassignedLights())
                    .Return(new LightGroup {Name = "Look Ma! New lights."});

                var statusProvider = new SystemStatusProvider(repo, _lightStatusSetter, lightAssignmentService);
                _result = statusProvider.GetSystemStatus();
            }

            [Test]
            public void Should_provide_the_persisted_master_model()
            {
                _result.MasterModel.ShouldEqual(_model);
            }

            [Test]
            public void Should_set_light_status_from_the_network()
            {
                _result.MasterModel.AllLights.All(x => x.SwitchState.Equals(SwitchState.On)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Length.ShouldEqual(4);
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(11)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(22)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(33)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(44)).ShouldBeTrue();
            }

            [Test]
            public void Should_provide_unassigned_lights()
            {
                _result.UnassignedLights.Name.ShouldEqual("Look Ma! New lights.");
            }
        }
    }

    public class LightStatusSetterTests
    {
        [TestFixture]
        public class When_updating_light_status_and_all_lights_are_in_the_network : TestBase
        {
            private Light[] _lights;
            private IZWaveNetwork _network;

            [SetUp]
            public void ContextSetup()
            {
                var switches = new[]
                {
                    new ZWaveSwitch {HomeId = 1, DeviceId = 1, SwitchState = SwitchState.On},
                    new ZWaveSwitch {HomeId = 1, DeviceId = 2, SwitchState = SwitchState.Off},
                    new ZWaveSwitch {HomeId = 1, DeviceId = 3, SwitchState = SwitchState.On},
                    new ZWaveSwitch {HomeId = 1, DeviceId = 4, SwitchState = SwitchState.Off}
                };

                //Reordered
                _lights = new[]
                {
                    new Light(switches[3].HomeId, switches[3].DeviceId),
                    new Light(switches[0].HomeId, switches[0].DeviceId),
                    new Light(switches[1].HomeId, switches[1].DeviceId),
                    new Light(switches[2].HomeId, switches[2].DeviceId)
                };

                _network = S<IZWaveNetwork>();
                _network.Stub(x => x.GetAllSwitches()).Return(switches);

                var statusProvider = new LightStatusSetter(_network);
                statusProvider.SetLightStatus(_lights);
            }

            [Test]
            public void Should_set_light_status()
            {
                _lights.Single(x => x.ZWaveDeviceId.Equals(1)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveDeviceId.Equals(2)).SwitchState.ShouldEqual(SwitchState.Off);
                _lights.Single(x => x.ZWaveDeviceId.Equals(3)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveDeviceId.Equals(4)).SwitchState.ShouldEqual(SwitchState.Off);
            }

            [Test]
            public void Should_only_query_the_network_once()
            {
                _network.AssertWasCalled(x => x.GetAllSwitches(), opt => opt.Repeat.Times(1));
            }
        }

        [TestFixture]
        public class When_some_lights_are_not_in_the_network_and_some_are : TestBase
        {
            private Light[] _lights;

            [SetUp]
            public void ContextSetup()
            {
                var switches = new[]
                {
                    new ZWaveSwitch {HomeId = 1, DeviceId = 1, SwitchState = SwitchState.On},
                    new ZWaveSwitch {HomeId = 1, DeviceId = 4, SwitchState = SwitchState.Off}
                };

                //Reordered
                _lights = new[]
                {
                    new Light(1, 2),
                    new Light(switches[0].HomeId, switches[0].DeviceId),
                    new Light(switches[1].HomeId, switches[1].DeviceId),
                    new Light(1, 3)
                };

                var network = S<IZWaveNetwork>();
                network.Stub(x => x.GetAllSwitches()).Return(switches);

                var statusProvider = new LightStatusSetter(network);
                statusProvider.SetLightStatus(_lights);
            }

            [Test]
            public void Should_set_light_status_for_the_lights_in_the_network()
            {
                _lights.Single(x => x.ZWaveDeviceId.Equals(1)).SwitchState.ShouldEqual(SwitchState.On);
                _lights.Single(x => x.ZWaveDeviceId.Equals(4)).SwitchState.ShouldEqual(SwitchState.Off);
            }

            [Test]
            public void Should_set_the_status_to_unknown_for_lights_not_in_the_network()
            {
                _lights.Single(x => x.ZWaveDeviceId.Equals(2)).SwitchState.ShouldEqual(SwitchState.Unknown);
                _lights.Single(x => x.ZWaveDeviceId.Equals(3)).SwitchState.ShouldEqual(SwitchState.Unknown);
            }
        }
    }
}