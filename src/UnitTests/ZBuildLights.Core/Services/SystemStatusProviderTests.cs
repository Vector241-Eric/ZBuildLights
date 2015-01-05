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
            private MasterModel _result;
            private StubLightStatusSetter _lightStatusSetter;

            [SetUp]
            public void ContextSetup()
            {
                var repo = S<IMasterModelRepository>();
                _model = new MasterModel();
                var group = _model.CreateProject().AddGroup(new LightGroup());
                group.AddLight(new Light(3, 11));
                group.AddLight(new Light(3, 22));
                group.AddLight(new Light(3, 33));
                group.AddLight(new Light(3, 44));
                repo.Stub(x => x.GetCurrent()).Return(_model);

                _lightStatusSetter = new StubLightStatusSetter().DefaultStatus(SwitchState.On).StubStatus(1,11,SwitchState.Off);

                var statusProvider = new SystemStatusProvider(repo, _lightStatusSetter);
                _result = statusProvider.GetSystemStatus();
            }

            [Test]
            public void Should_provide_the_persisted_master_model()
            {
                _result.ShouldEqual(_model);
            }

            [Test]
            public void Should_set_light_status_from_the_network()
            {
                _result.AllLights.All(x => x.SwitchState.Equals(SwitchState.On)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Length.ShouldEqual(4);
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(11)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(22)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(33)).ShouldBeTrue();
                _lightStatusSetter.LightsThatHadStatusSet.Any(x => x.ZWaveDeviceId.Equals(44)).ShouldBeTrue();
            }
        }
    }
}