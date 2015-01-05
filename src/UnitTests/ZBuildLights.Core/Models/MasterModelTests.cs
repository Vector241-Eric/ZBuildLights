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
                
                var project1 = model.AddProject(new Project {Name = "1"});
                
                var group1_1 = project1.AddGroup(new LightGroup {Name = "1.1"});
                var light1_1_1 = group1_1.AddLight(new Light(11, 1));
                var light1_1_2 = group1_1.AddLight(new Light(11, 2));
                var light1_1_3 = group1_1.AddLight(new Light(11, 3));
                
                var group1_2 = project1.AddGroup(new LightGroup {Name = "1.2"});
                var light1_2_1 = group1_2.AddLight(new Light(12, 1));
                var light1_2_2 = group1_2.AddLight(new Light(12, 2));
                var light1_2_3 = group1_2.AddLight(new Light(12, 3));
                
                var group1_3 = project1.AddGroup(new LightGroup {Name = "1.3"});
                var light1_3_1 = group1_3.AddLight(new Light(13, 1));
                var light1_3_2 = group1_3.AddLight(new Light(13, 2));
                var light1_3_3 = group1_3.AddLight(new Light(13, 3));

                var project2 = model.AddProject(new Project {Name = "1"});
                
                var group2_1 = project2.AddGroup(new LightGroup {Name = "2.1"});
                var light2_1_1 = group2_1.AddLight(new Light(21, 1));
                var light2_1_2 = group2_1.AddLight(new Light(21, 2));
                var light2_1_3 = group2_1.AddLight(new Light(21, 3));
                
                var group2_2 = project2.AddGroup(new LightGroup {Name = "2.2"});
                var light2_2_1 = group2_2.AddLight(new Light(22, 1));
                var light2_2_2 = group2_2.AddLight(new Light(22, 2));
                var light2_2_3 = group2_2.AddLight(new Light(22, 3));
                
                var group2_3 = project2.AddGroup(new LightGroup {Name = "2.3"});
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
                var group = model.AddProject(new Project()).AddGroup(new LightGroup());
                group.AddLight(new Light(1, 11)).AddLight(new Light(2, 22));
                model.AddUnassignedLights(new[] {new Light(3, 33), new Light(4, 44),});

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
        public class When_model_has_some_unassigned_lights_and_we_get_the_unassigned_group
        {
            private LightGroup _results;

            [SetUp]
            public void ContextSetup()
            {
                var model = new MasterModel();
                model.AddUnassignedLights(new[] {new Light(1, 1), new Light(2, 2),});
                _results = model.GetUnassignedGroup();
            }

            [Test]
            public void Should_set_the_group_name()
            {
                _results.Name.ShouldEqual("Unassigned");
            }

            [Test]
            public void Should_contain_the_unassigned_lights()
            {
                _results.Lights.Length.ShouldEqual(2);
                _results.Lights.Any(x => x.ZWaveDeviceId.Equals(1) && x.ZWaveHomeId.Equals(1)).ShouldBeTrue();
                _results.Lights.Any(x => x.ZWaveDeviceId.Equals(2) && x.ZWaveHomeId.Equals(2)).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_model_has_multiple_projects_with_multiple_groups
        {
            [Test]
            public void Should_find_a_group_by_Id()
            {
                var model = new MasterModel();
                
                var project1 = model.AddProject(new Project {Name = "1"});
                var group1_1 = project1.AddGroup(new LightGroup {Name = "1.1", Id = Guid.NewGuid()});
                var group1_2 = project1.AddGroup(new LightGroup {Name = "1.2", Id = Guid.NewGuid()});
                var group1_3 = project1.AddGroup(new LightGroup {Name = "1.3", Id = Guid.NewGuid()});

                var project2 = model.AddProject(new Project {Name = "1"});
                var group2_1 = project2.AddGroup(new LightGroup {Name = "2.1", Id = Guid.NewGuid()});
                var group2_2 = project2.AddGroup(new LightGroup {Name = "2.2", Id = Guid.NewGuid()});
                var group2_3 = project2.AddGroup(new LightGroup {Name = "2.3", Id = Guid.NewGuid()});

                var found = model.FindGroup(group2_2.Id);
            }
        }

        [TestFixture]
        public class When_model_is_empty
        {

            [Test]
            public void Should_throw_an_exception_when_searching_for_a_group()
            {
                var model = new MasterModel();

                Exception thrown = null;

                var id = Guid.NewGuid();
                try
                {
                    model.FindGroup(id);
                }
                catch (Exception e)
                {
                    thrown = e;
                }

                thrown.GetType().ShouldEqual(typeof(InvalidOperationException));
                thrown.Message.ShouldEqual(string.Format("Could not find group with id: {0}", id));
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
                var groupId = Guid.NewGuid();
                _light = new Light(11, 23);
                _model = new MasterModel();
                _destinationGroup = new LightGroup{Id = groupId};

                _model.AddProject(new Project()).AddGroup(_destinationGroup);
                _model.AddUnassignedLight(_light);

                _model.AssignLightToGroup(11, 23, groupId);
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
                _model.GetUnassignedGroup().Lights.Length.ShouldEqual(0);
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
                _fooGroup = model.AddProject(new Project()).AddGroup(new LightGroup {Id = Guid.NewGuid()});
                _barGroup = model.AddProject(new Project()).AddGroup(new LightGroup {Id = Guid.NewGuid()});

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
                _barGroup.Lights[0].ZWaveHomeId.ShouldEqual((uint)1);
                _light.ParentGroup.ShouldBeSameAs(_barGroup);
            }
        }

    }
}