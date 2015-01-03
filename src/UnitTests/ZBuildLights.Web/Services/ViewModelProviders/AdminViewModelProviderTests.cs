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
                var projects = new Project[5];

                var masterModel = new MasterModel {Projects = projects};

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Project[], AdminProjectViewModel[]>(projects))
                    .Return(new AdminProjectViewModel[3]);
                mapper.Stub(x => x.Map<LightGroup, AdminLightGroupViewModel>(null))
                    .IgnoreArguments()
                    .Return(new AdminLightGroupViewModel {Name = "bar"});

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
                _result.Unassigned.Name.ShouldEqual("bar");
            }
        }
    }
}