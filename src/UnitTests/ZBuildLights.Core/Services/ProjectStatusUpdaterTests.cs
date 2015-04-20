using System.Linq;
using NUnit.Framework;
using Should;
using UnitTests._Stubs;
using UnitTests._TestData;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services;

namespace UnitTests.ZBuildLights.Core.Services
{
    public class ProjectStatusUpdaterTests
    {
        [TestFixture]
        public class When_all_projects_are_passing_and_none_are_building
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject1;
            private Project _zBuildLightsProject2;
            private StubCcReader _cruiseReader;
            private CruiseServer _cruiseServer1;
            private CruiseServer _cruiseServer2;
            private CruiseServer _cruiseServerNotReferenced;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                _cruiseServer1 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });
                _cruiseServer2 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server2";
                    x.Name = "Server 2";
                });
                _cruiseServerNotReferenced = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server3";
                    x.Name = "Server 3";
                });

                _zBuildLightsProject1 = masterModel.CreateProject();
                _zBuildLightsProject1.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = _cruiseServer1.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = _cruiseServer1.Id},
                    new CruiseProjectAssociation {Name = "Project 2.1", ServerId = _cruiseServer2.Id}
                };

                _zBuildLightsProject2 = masterModel.CreateProject();
                _zBuildLightsProject2.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project A", ServerId = _cruiseServer2.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Failure)
                    }
                };

                var ccReaderDataServer2 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 2.1")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project A")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Failure)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                _cruiseReader = new StubCcReader()
                    .WithResponse(_cruiseServer1.Url, NetworkResponse.Success(ccReaderDataServer1))
                    .WithResponse(_cruiseServer2.Url, NetworkResponse.Success(ccReaderDataServer2));

                var updater = new ProjectStatusUpdater(repository, _cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_success()
            {
                _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject1.Id))
                    .StatusMode.ShouldEqual(StatusMode.Success);
                _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject2.Id))
                    .StatusMode.ShouldEqual(StatusMode.Success);
            }

            [Test]
            public void Should_only_request_status_for_each_cruise_server_once()
            {
                _cruiseReader.GetLookupCountForUrl(_cruiseServer1.Url).ShouldEqual(1);
                _cruiseReader.GetLookupCountForUrl(_cruiseServer2.Url).ShouldEqual(1);
            }

            [Test]
            public void Should_not_request_status_for_unreferenced_servers()
            {
                _cruiseReader.GetLookupCountForUrl(_cruiseServerNotReferenced.Url).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_all_projects_are_passing_and_some_are_checking_modifications
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });
                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id}
                };

                var ccServerData = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.CheckingModifications)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Failure)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccServerData));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_success()
            {
                var status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                status.ShouldEqual(StatusMode.Success);
            }
        }

        [TestFixture]
        public class When_all_projects_are_passing_and_some_are_building
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer1 = masterModel.CreateCruiseServer(x => x.Url = "https://example.com/server1");

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer1.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer1.Id}
                };

                var cruiseProjects = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Failure)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer1.Url, NetworkResponse.Success(cruiseProjects));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_success_and_building()
            {
                _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode.ShouldEqual(StatusMode.SuccessAndBuilding);
            }
        }

        [TestFixture]
        public class When_all_projects_are_failing_and_none_are_building
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Failure),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Failure),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Success)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccReaderDataServer1));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_failure()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.Broken);
            }
        }

        [TestFixture]
        public class When_all_projects_are_failing_and_some_are_building
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Failure),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Failure),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Success)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccReaderDataServer1));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_failure_and_building()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.BrokenAndBuilding);
            }
        }

        [TestFixture]
        public class When_all_but_one_project_are_passing_and_one_is_disconnected
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.3", ServerId = cruiseServer.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Unknown),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project 1.3")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccReaderDataServer1));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_disconnected()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.NotConnected);
            }
        }

        [TestFixture]
        public class When_server_is_unreachable
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id}
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url,
                        NetworkResponse.Fail<Projects>("Could not reach server or something bad happened."));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_set_status_for_disconnected()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.NotConnected);
            }
        }

        [TestFixture]
        public class When_cruise_project_has_an_unparseable_status
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Building)
                            .StatusString("I'm a bad value"),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccReaderDataServer1));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_default_to_disconnected()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.NotConnected);
            }
        }

        [TestFixture]
        public class When_cruise_project_does_not_exist_on_the_server
        {
            private MasterModel _lastSavedModel;
            private Project _zBuildLightsProject;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var cruiseServer = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "https://example.com/server1";
                    x.Name = "Server 1";
                });

                _zBuildLightsProject = masterModel.CreateProject();
                _zBuildLightsProject.CruiseProjectAssociations = new[]
                {
                    new CruiseProjectAssociation {Name = "Project 1.1", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project 1.2", ServerId = cruiseServer.Id},
                    new CruiseProjectAssociation {Name = "Project Not On Server", ServerId = cruiseServer.Id}
                };

                var ccReaderDataServer1 = new Projects
                {
                    Items = new ProjectsProject[]
                    {
                        New.ProjectsProject.Name("Project 1.1")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Project 1.2")
                            .Activity(CcBuildActivity.Sleeping)
                            .Status(CcBuildStatus.Success),
                        New.ProjectsProject.Name("Detractor")
                            .Activity(CcBuildActivity.Building)
                            .Status(CcBuildStatus.Success)
                    }
                };

                var repository = new StubMasterModelRepository();
                repository.UseCurrentModel(masterModel);

                var cruiseReader = new StubCcReader()
                    .WithResponse(cruiseServer.Url, NetworkResponse.Success(ccReaderDataServer1));

                var updater = new ProjectStatusUpdater(repository, cruiseReader);
                updater.UpdateAllProjectStatuses();

                _lastSavedModel = repository.LastSaved;
            }

            [Test]
            public void Should_default_to_disconnected()
            {
                var project1Status = _lastSavedModel.Projects
                    .Single(x => x.Id.Equals(_zBuildLightsProject.Id))
                    .StatusMode;
                project1Status.ShouldEqual(StatusMode.NotConnected);
            }
        }
    }
}