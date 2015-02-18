using System;
using System.Linq;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;

namespace UnitTests.ZBuildLights.Core.Models
{
    public class MasterModelTests
    {
        [TestFixture]
        public class When_model_has_multiple_projects_with_multiple_groups_with_multiple_lights
        {
            [Test]
            public void Should_find_a_light_by_homeId_and_deviceId()
            {
                var model = new MasterModel();

                var project1 = model.CreateProject(x => x.Name = "1");

                var group1_1 = project1.CreateGroup(x => x.Name = "1.1");
                var light1_1_1 = group1_1.AddLight(new Light(11, 1));
                var light1_1_2 = group1_1.AddLight(new Light(11, 2));
                var light1_1_3 = group1_1.AddLight(new Light(11, 3));

                var group1_2 = project1.CreateGroup(x => x.Name = "1.2");
                var light1_2_1 = group1_2.AddLight(new Light(12, 1));
                var light1_2_2 = group1_2.AddLight(new Light(12, 2));
                var light1_2_3 = group1_2.AddLight(new Light(12, 3));

                var group1_3 = project1.CreateGroup(x => x.Name = "1.3");
                var light1_3_1 = group1_3.AddLight(new Light(13, 1));
                var light1_3_2 = group1_3.AddLight(new Light(13, 2));
                var light1_3_3 = group1_3.AddLight(new Light(13, 3));

                var project2 = model.CreateProject(x => x.Name = "1");

                var group2_1 = project2.CreateGroup(x => x.Name = "2.1");
                var light2_1_1 = group2_1.AddLight(new Light(21, 1));
                var light2_1_2 = group2_1.AddLight(new Light(21, 2));
                var light2_1_3 = group2_1.AddLight(new Light(21, 3));

                var group2_2 = project2.CreateGroup(x => x.Name = "2.2");
                var light2_2_1 = group2_2.AddLight(new Light(22, 1));
                var light2_2_2 = group2_2.AddLight(new Light(22, 2));
                var light2_2_3 = group2_2.AddLight(new Light(22, 3));

                var group2_3 = project2.CreateGroup(x => x.Name = "2.3");
                var light2_3_1 = group2_3.AddLight(new Light(23, 1));
                var light2_3_2 = group2_3.AddLight(new Light(23, 2));
                var light2_3_3 = group2_3.AddLight(new Light(23, 3));

                uint homeId = 22;
                byte deviceId = 2;

                var found = model.FindLight(homeId, deviceId);
                found.ZWaveHomeId.ShouldEqual(homeId);
                found.ZWaveDeviceId.ShouldEqual(deviceId);
                found.ParentGroup.ShouldBeSameAs(group2_2);
                found.ParentGroup.ParentProject.ShouldBeSameAs(project2);
            }
        }

        [TestFixture]
        public class When_getting_all_lights
        {
            private Light[] _result;

            [SetUp]
            public void ContextSetup()
            {
                var model = new MasterModel();
                var group = model.CreateProject().CreateGroup();
                group.AddLight(new Light(1, 11)).AddLight(new Light(2, 22));
                model.AddUnassignedLights(new[] {new Light(3, 33), new Light(4, 44)});

                _result = model.AllLights;
            }

            [Test]
            public void Should_include_lights_in_groups()
            {
                _result.Any(x => x.ZWaveHomeId.Equals(1)).ShouldBeTrue();
                _result.Any(x => x.ZWaveHomeId.Equals(2)).ShouldBeTrue();
            }

            [Test]
            public void Should_include_lights_that_are_unassigned()
            {
                _result.Any(x => x.ZWaveHomeId.Equals(3)).ShouldBeTrue();
                _result.Any(x => x.ZWaveHomeId.Equals(4)).ShouldBeTrue();
            }

            [Test]
            public void Should_not_include_duplicates()
            {
                _result.Length.ShouldEqual(4);
            }
        }

        [TestFixture]
        public class When_model_has_multiple_projects_with_multiple_groups
        {
            [Test]
            public void Should_find_a_group_by_Id()
            {
                var model = new MasterModel();

                var project1 = model.CreateProject(x => x.Name = "1");
                var group1_1 = project1.CreateGroup(x => x.Name = "1.1");
                var group1_2 = project1.CreateGroup(x => x.Name = "1.2");
                var group1_3 = project1.CreateGroup(x => x.Name = "1.3");

                var project2 = model.CreateProject(x => x.Name = "1");
                var group2_1 = project2.CreateGroup(x => x.Name = "2.1");
                var group2_2 = project2.CreateGroup(x => x.Name = "2.2");
                var group2_3 = project2.CreateGroup(x => x.Name = "2.3");

                var found = model.FindGroup(group2_2.Id);
            }
        }

        [TestFixture]
        public class When_model_is_empty
        {
            private MasterModel _model;

            [SetUp]
            public void ContextSetup()
            {
                _model = new MasterModel();
            }

            [Test]
            public void Should_throw_an_exception_when_searching_for_a_group()
            {
                Exception thrown = null;

                var id = Guid.NewGuid();
                try
                {
                    _model.FindGroup(id);
                }
                catch (Exception e)
                {
                    thrown = e;
                }

                thrown.GetType().ShouldEqual(typeof (InvalidOperationException));
                thrown.Message.ShouldEqual(string.Format("Could not find group with id: {0}", id));
            }

            [Test]
            public void Should_not_have_any_cruise_servers()
            {
                _model.CruiseServers.Length.ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_assigning_an_unassigned_light
        {
            private MasterModel _model;
            private Light _light;
            private LightGroup _destinationGroup;

            [SetUp]
            public void ContextSetup()
            {
                _light = new Light(11, 23);
                _model = new MasterModel();
                _destinationGroup = _model.CreateProject().CreateGroup();

                _model.AddUnassignedLight(_light);

                _model.AssignLightToGroup(11, 23, _destinationGroup.Id);
            }

            [Test]
            public void Should_set_the_parent_group()
            {
                _light.ParentGroup.ShouldBeSameAs(_destinationGroup);
            }

            [Test]
            public void Should_add_the_light_to_the_parent_group()
            {
                _destinationGroup.Lights.ShouldContain(_light);
            }

            [Test]
            public void Should_remove_the_light_from_the_unassigned_collection()
            {
                _model.UnassignedLights.Length.ShouldEqual(0);
                _model.UnassignedLights.Length.ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_moving_a_light_from_one_group_to_another
        {
            private LightGroup _fooGroup;
            private LightGroup _barGroup;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                var model = new MasterModel();
                _fooGroup = model.CreateProject().CreateGroup();
                _barGroup = model.CreateProject().CreateGroup();

                _light = new Light(1, 2);
                _fooGroup.AddLight(_light);

                model.AssignLightToGroup(1, 2, _barGroup.Id);
            }

            [Test]
            public void Should_remove_the_light_from_the_original_group()
            {
                _fooGroup.Lights.Length.ShouldEqual(0);
            }

            [Test]
            public void Should_add_the_light_to_the_new_group()
            {
                _barGroup.Lights.Length.ShouldEqual(1);
                _barGroup.Lights[0].ZWaveHomeId.ShouldEqual((uint) 1);
                _light.ParentGroup.ShouldBeSameAs(_barGroup);
            }
        }

        [TestFixture]
        public class When_creating_a_project_with_an_initializer_without_an_id
        {
            private MasterModel _masterModel;
            private Project _project;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject(x => x.Name = "Howdy");
            }

            [Test]
            public void Should_set_the_master_model_reference()
            {
                _project.MasterModel.ShouldBeSameAs(_masterModel);
            }

            [Test]
            public void Should_assign_an_id()
            {
                _project.Id.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_initialize_the_project_based_on_the_provided_initializer()
            {
                _project.Name.ShouldEqual("Howdy");
            }
        }

        [TestFixture]
        public class When_creating_a_project_without_an_initializer
        {
            private MasterModel _masterModel;
            private Project _project;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject();
            }

            [Test]
            public void Should_set_the_master_model_reference()
            {
                _project.MasterModel.ShouldBeSameAs(_masterModel);
            }

            [Test]
            public void Should_assign_an_id()
            {
                _project.Id.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_add_the_project_to_the_project_collection()
            {
                _masterModel.Projects.Length.ShouldEqual(1);
                _masterModel.Projects[0].ShouldBeSameAs(_project);
            }
        }

        [TestFixture]
        public class When_creating_a_project_with_an_id
        {
            private MasterModel _masterModel;
            private Project _project;
            private Guid _expectedId = Guid.NewGuid();

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _project = _masterModel.CreateProject(x => x.Id = _expectedId);
            }

            [Test]
            public void Should_keep_the_existing_id()
            {
                _project.Id.ShouldEqual(_expectedId);
            }
        }

        [TestFixture]
        public class When_removing_project_from_master_model
        {
            private MasterModel _masterModel;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                var notDeleted = _masterModel.CreateProject(x => x.Name = "Not Deleted");
                notDeleted.CreateGroup().AddLight(new Light(1, 11)).AddLight(new Light(1, 12));
                notDeleted.CreateGroup().AddLight(new Light(1, 13)).AddLight(new Light(1, 14));
                var toBeDeleted = _masterModel.CreateProject(x => x.Name = "To Be Deleted");
                toBeDeleted.CreateGroup().AddLight(new Light(1, 21)).AddLight(new Light(1, 22));
                toBeDeleted.CreateGroup().AddLight(new Light(1, 23)).AddLight(new Light(1, 24));

                _masterModel.RemoveProject(toBeDeleted.Id);
            }

            [Test]
            public void Should_reassign_all_lights_under_that_project_to_the_unassigned_group()
            {
                var unassignedLights = _masterModel.UnassignedLights;
                unassignedLights.Length.ShouldEqual(4);
                unassignedLights.Any(x => x.ZWaveDeviceId == (byte) 21).ShouldBeTrue();
                unassignedLights.Any(x => x.ZWaveDeviceId == (byte) 22).ShouldBeTrue();
                unassignedLights.Any(x => x.ZWaveDeviceId == (byte) 23).ShouldBeTrue();
                unassignedLights.Any(x => x.ZWaveDeviceId == (byte) 24).ShouldBeTrue();
            }

            [Test]
            public void Should_not_remove_other_projects()
            {
                _masterModel.Projects.Any(x => x.Name == "Not Deleted").ShouldBeTrue();
                var undeletedProject = _masterModel.Projects.Single(x => x.Name == "Not Deleted");
                var assignedLights = undeletedProject.Groups.SelectMany(x => x.Lights).ToArray();
                assignedLights.Length.ShouldEqual(4);
                assignedLights.Any(x => x.ZWaveDeviceId == (byte) 11).ShouldBeTrue();
                assignedLights.Any(x => x.ZWaveDeviceId == (byte) 12).ShouldBeTrue();
                assignedLights.Any(x => x.ZWaveDeviceId == (byte) 13).ShouldBeTrue();
                assignedLights.Any(x => x.ZWaveDeviceId == (byte) 14).ShouldBeTrue();
            }

            [Test]
            public void Should_remove_the_requested_project()
            {
                _masterModel.Projects.Length.ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_adding_cruise_server_to_master_model
        {
            private MasterModel _model;

            [SetUp]
            public void ContextSetup()
            {
                _model = new MasterModel();
                _model.CreateCruiseServer(x => { x.Url = "http://www.example.com/1"; x.Name = "1"; });
                _model.CreateCruiseServer(x => { x.Url = "http://www.example.com/2"; x.Name = "2"; });
            }

            [Test]
            public void Should_add_server()
            {

                _model.CruiseServers.Length.ShouldEqual(2);
                _model.CruiseServers.Count(x => x.Url.Equals("http://www.example.com/1")).ShouldEqual(1);
                _model.CruiseServers.Count(x => x.Url.Equals("http://www.example.com/2")).ShouldEqual(1);
            }

            [Test]
            public void Should_set_ID_on_cruise_server()
            {
                _model.CruiseServers.Any(x => x.Id == Guid.Empty).ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_removing_cruise_server_from_master_model
        {
            [Test]
            public void Should_remove_cruise_server()
            {
                var model = new MasterModel();
                var server1 = model.CreateCruiseServer(x => { x.Url = "http://www.example.com/1"; x.Name = "1"; });
                var server2 = model.CreateCruiseServer(x => { x.Url = "http://www.example.com/2"; x.Name = "2"; });
                var server3 = model.CreateCruiseServer(x => { x.Url = "http://www.example.com/3"; x.Name = "3"; });

                model.RemoveCruiseServer(server2.Id);

                model.CruiseServers.Length.ShouldEqual(2);
                model.CruiseServers.Count(x => x.Id.Equals(server1.Id)).ShouldEqual(1);
                model.CruiseServers.Count(x => x.Id.Equals(server3.Id)).ShouldEqual(1);
            }
        }
    }
}