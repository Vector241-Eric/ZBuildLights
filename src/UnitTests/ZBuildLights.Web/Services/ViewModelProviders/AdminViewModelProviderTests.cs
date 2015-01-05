using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace UnitTests.ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProviderTests
    {
        [TestFixture]
        public class Always : TestBase
        {
            private AdminViewModel _result;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                masterModel.CreateProject();    //1
                masterModel.CreateProject();    //2
                masterModel.CreateProject();    //3
                masterModel.CreateProject();    //4
                masterModel.CreateProject();    //5

                masterModel.AddUnassignedLight(new Light(1, 22));

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Project[], AdminProjectViewModel[]>(masterModel.Projects))
                    .Return(new AdminProjectViewModel[3]);
                mapper.Stub(x => x.Map<Light[], AdminLightViewModel[]>(masterModel.UnassignedLights))
                    .IgnoreArguments()
                    .Return(new AdminLightViewModel[]{new AdminLightViewModel{ZWaveHomeId = 1, ZWaveDeviceId = 22}, });

                var provider = new AdminViewModelProvider(statusProvider, mapper);
                _result = provider.GetIndexViewModel();
            }

            [Test]
            public void Should_set_projects_based_on_all_projects()
            {
                _result.Projects.Length.ShouldEqual(3);
            }

            [Test]
            public void Should_set_unassigned_group_based_on_currently_unassigned_lights()
            {
                _result.Unassigned.Name.ShouldEqual("Unassigned");
                _result.Unassigned.Lights.Length.ShouldEqual(1);
                _result.Unassigned.Lights.Single().ZWaveHomeId.ShouldEqual((byte)1);
                _result.Unassigned.Lights.Single().ZWaveDeviceId.ShouldEqual((byte)22);
            }
        }
    }
}