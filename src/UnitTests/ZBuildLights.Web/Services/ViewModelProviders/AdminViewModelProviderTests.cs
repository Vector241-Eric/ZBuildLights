using System;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;
using ZBuildLights.Web.Services;
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
                var unassignedGroup = new LightGroup {Name = "foo"};
                var projects = new Project[5];

                var lightAssignmentService = S<ILightAssignmentService>();
                lightAssignmentService.Stub(x => x.GetUnassignedLights()).Return(unassignedGroup);

                var repo = S<IMasterModelRepository>();
                repo.Stub(x => x.GetCurrent()).Return(new MasterModel(){Projects = projects});

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Project[], AdminProjectViewModel[]>(projects))
                    .Return(new AdminProjectViewModel[3]);
                mapper.Stub(x => x.Map<LightGroup, AdminLightGroupViewModel>(unassignedGroup))
                    .Return(new AdminLightGroupViewModel {Name = "bar"});

                var provider = new AdminViewModelProvider(repo, lightAssignmentService, mapper);
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