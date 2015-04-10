using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using UnitTests._Bases;
using UnitTests._Stubs;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.CruiseControl;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace UnitTests.ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProviderTests
    {
        [TestFixture]
        public class When_getting_admin_view_model : TestBase
        {
            private AdminViewModel _result;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                masterModel.CreateProject(); //1
                masterModel.CreateProject(); //2
                masterModel.CreateProject(); //3
                masterModel.CreateProject(); //4
                masterModel.CreateProject(); //5

                masterModel.AddUnassignedLight(new Light(1, 22));

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Project[], AdminProjectViewModel[]>(masterModel.Projects))
                    .Return(new AdminProjectViewModel[3]);
                mapper.Stub(x => x.Map<Light[], AdminLightViewModel[]>(masterModel.UnassignedLights))
                    .IgnoreArguments()
                    .Return(new[] {new AdminLightViewModel {ZWaveHomeId = 1, ZWaveDeviceId = 22}});

                var provider = new AdminViewModelProvider(statusProvider, mapper, null);
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
                _result.Unassigned.Lights.Single().ZWaveHomeId.ShouldEqual((byte) 1);
                _result.Unassigned.Lights.Single().ZWaveDeviceId.ShouldEqual((byte) 22);
            }
        }

        [TestFixture]
        public class When_getting_edit_project_view_model_and_project_exists : TestBase
        {
            private EditProjectMasterViewModel _result;
            private EditProjectViewModel _projectViewModel;

            [SetUp]
            public void ContextSetup()
            {
                _projectViewModel = new EditProjectViewModel {Name = "I'm Mapped"};

                var masterModel = new MasterModel();
                var project1 = masterModel.CreateProject(); //1
                var projectWeAreLookingFor = masterModel.CreateProject(); //2
                var project3 = masterModel.CreateProject(); //3

                var server1 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/1";
                    x.Name = "1";
                });
                var server2 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/2";
                    x.Name = "2";
                });

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var cruiseProjectProvider = S<ICruiseProjectModelProvider>();
                var projectResponse1 = NetworkResponse.Success(new CcProjectCollectionViewModel
                {
                    Items = new[] {new CcProjectViewModel {Name = "1.1"}, new CcProjectViewModel {Name = "1.2"}}
                });
                var projectResponse2 = NetworkResponse.Success(new CcProjectCollectionViewModel
                {
                    Items = new[] {new CcProjectViewModel {Name = "2.1"}, new CcProjectViewModel {Name = "2.2"}}
                });

                cruiseProjectProvider.Stub(x => x.GetProjects(server1.Id)).Return(projectResponse1);
                cruiseProjectProvider.Stub(x => x.GetProjects(server2.Id)).Return(projectResponse2);


                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<Project, EditProjectViewModel>(projectWeAreLookingFor))
                    .Return(_projectViewModel);
                mapper.Stub(x => x.Map<CruiseServer[], EditProjectCruiseServerViewModel[]>(masterModel.CruiseServers))
                    .Return(new[]
                    {
                        new EditProjectCruiseServerViewModel {Url = "foo 1", Id = server1.Id},
                        new EditProjectCruiseServerViewModel {Url = "foo 2", Id = server2.Id}
                    });
                var provider = new AdminViewModelProvider(statusProvider, mapper, cruiseProjectProvider);
                _result = provider.GetEditProjectViewModel(projectWeAreLookingFor.Id);
            }

            [Test]
            public void Should_return_a_view_model_with_the_project()
            {
                _result.Project.ShouldBeSameAs(_projectViewModel);
            }

            [Test]
            public void Should_return_a_list_of_configured_cc_servers_and_projects()
            {
                _result.CruiseServers.Length.ShouldEqual(2);
                _result.CruiseServers.Any(x => x.Url.Contains("1")).ShouldBeTrue();
                _result.CruiseServers.Any(x => x.Url.Contains("2")).ShouldBeTrue();
            }

            [Test]
            public void Should_include_the_current_projects_from_the_cruise_server()
            {
                _result.CruiseServers.Single(x => x.Url.Equals("foo 1")).Projects.Length.ShouldEqual(2);
                _result.CruiseServers.Single(x => x.Url.Equals("foo 1")).Projects.ShouldContain("1.1");
                _result.CruiseServers.Single(x => x.Url.Equals("foo 1")).Projects.ShouldContain("1.2");
                _result.CruiseServers.Single(x => x.Url.Equals("foo 2")).Projects.Length.ShouldEqual(2);
                _result.CruiseServers.Single(x => x.Url.Equals("foo 2")).Projects.ShouldContain("2.1");
                _result.CruiseServers.Single(x => x.Url.Equals("foo 2")).Projects.ShouldContain("2.2");
            }

            [Test]
            public void Should_set_header_text_to_Edit_Project()
            {
                _result.HeaderText.ShouldEqual("Edit Project");
            }

            [Test]
            public void Should_enable_delete()
            {
                _result.ShowDelete.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_getting_edit_project_view_model_and_project_does_not_exist : TestBase
        {
            private Exception _thrown;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                IMapper doNotUseMapper = null;

                var provider = new AdminViewModelProvider(statusProvider, doNotUseMapper, null);
                _thrown = ExpectException<ArgumentException>(() => { provider.GetEditProjectViewModel(Guid.NewGuid()); });
            }

            [Test]
            public void Should_throw_an_exception_for_the_missing_project()
            {
                _thrown.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_edit_project_view_model_and_id_is_empty : TestBase
        {
            private EditProjectMasterViewModel _result;
            private Project _project1;
            private Project _project2;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                _project1 = masterModel.CreateProject(); //1
                _project2 = masterModel.CreateProject(); //2

                var server1 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/1";
                    x.Name = "1";
                });
                var server2 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/2";
                    x.Name = "2";
                });

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var cruiseProjectProvider = S<ICruiseProjectModelProvider>();
                var projectResponse1 = NetworkResponse.Success(new CcProjectCollectionViewModel
                {
                    Items = new[] {new CcProjectViewModel {Name = "1.1"}, new CcProjectViewModel {Name = "1.2"}}
                });
                var projectResponse2 = NetworkResponse.Success(new CcProjectCollectionViewModel
                {
                    Items = new[] {new CcProjectViewModel {Name = "2.1"}, new CcProjectViewModel {Name = "2.2"}}
                });

                cruiseProjectProvider.Stub(x => x.GetProjects(server1.Id)).Return(projectResponse1);
                cruiseProjectProvider.Stub(x => x.GetProjects(server2.Id)).Return(projectResponse2);

                var mapper = new StubMapper()
                    .StubResult(new[]
                    {
                        new EditProjectCruiseServerViewModel {Url = "Mapped 1", Id = server1.Id},
                        new EditProjectCruiseServerViewModel {Url = "Mapped 2", Id = server2.Id}
                    });
                var provider = new AdminViewModelProvider(statusProvider, mapper, cruiseProjectProvider);
                _result = provider.GetEditProjectViewModel(null);
            }

            [Test]
            public void Should_provide_a_new_empty_project()
            {
                _result.Project.Id.ShouldBeNull();
            }

            [Test]
            public void Should_return_a_list_of_configured_cc_servers_and_projects()
            {
                _result.CruiseServers.Length.ShouldEqual(2);
                _result.CruiseServers.Any(x => x.Url.Equals("Mapped 1")).ShouldBeTrue();
                _result.CruiseServers.Any(x => x.Url.Equals("Mapped 2")).ShouldBeTrue();
            }

            [Test]
            public void Should_include_the_current_projects_from_the_cruise_server()
            {
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 1")).Projects.Length.ShouldEqual(2);
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 1")).Projects.ShouldContain("1.1");
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 1")).Projects.ShouldContain("1.2");
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 2")).Projects.Length.ShouldEqual(2);
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 2")).Projects.ShouldContain("2.1");
                _result.CruiseServers.Single(x => x.Url.Equals("Mapped 2")).Projects.ShouldContain("2.2");
            }

            [Test]
            public void Should_set_header_text_to_Create_Project()
            {
                _result.HeaderText.ShouldEqual("Create Project");
            }

            [Test]
            public void Should_not_enable_delete()
            {
                _result.ShowDelete.ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_getting_cruise_servers_edit_view_model : TestBase
        {
            private EditCruiseServerViewModel[] _result;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/1";
                    x.Name = "1";
                });
                masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/2";
                    x.Name = "2";
                });

                var mappedServers = new[]
                {
                    new EditCruiseServerViewModel {Url = "url 1"},
                    new EditCruiseServerViewModel {Url = "url 2"}
                };

                var statusProvider = S<ISystemStatusProvider>();
                statusProvider.Stub(x => x.GetSystemStatus())
                    .Return(masterModel);

                var mapper = S<IMapper>();
                mapper.Stub(x => x.Map<CruiseServer[], EditCruiseServerViewModel[]>(masterModel.CruiseServers))
                    .Return(mappedServers);
                var provider = new AdminViewModelProvider(statusProvider, mapper, null);
                _result = provider.GetCruiseServerViewModels();
            }

            [Test]
            public void Should_provide_view_models()
            {
                _result.Length.ShouldEqual(2);
                _result.Any(x => x.Url.Equals("url 1")).ShouldBeTrue();
                _result.Any(x => x.Url.Equals("url 2")).ShouldBeTrue();
            }
        }
    }
}