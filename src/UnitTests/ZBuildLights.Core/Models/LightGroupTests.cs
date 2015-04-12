using System;
using System.Linq;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;

namespace UnitTests.ZBuildLights.Core.Models
{
    public class LightGroupTests
    {
        [TestFixture]
        public class When_getting_the_full_name_of_a_group
        {
            [Test]
            public void Should_include_the_name_of_the_parent_project()
            {
                var project = new MasterModel().CreateProject(x => x.Name = "Foo");
                var group = project.CreateGroup(x => x.Name = "Bar");

                group.FullName.ShouldEqual("Foo.Bar");
            }
        }

        [TestFixture]
        public class When_adding_a_light_that_isnt_in_a_group
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _group = new MasterModel().CreateProject().CreateGroup();
                _light = new Light(1, 2, 123);
                _group.AddLight(_light);
            }

            [Test]
            public void Should_set_the_group_referene_to_the_containing_group()
            {
                _light.ParentGroup.ShouldBeSameAs(_group);
            }

            [Test]
            public void Should_add_the_light_to_the_light_collection_on_the_group()
            {
                _group.Lights.Length.ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_adding_a_light_that_is_already_in_a_group
        {
            private Exception _thrown;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var fooDaddy = masterModel.CreateProject(x => x.Name = "FooDaddy");
                var group = fooDaddy.CreateGroup(x => x.Name = "Foo");
                var light = new Light(1, 2, 123);
                group.AddLight(light);

                var barDaddy = masterModel.CreateProject(x => x.Name = "BarDaddy");
                var newGroup = barDaddy.CreateGroup(x => x.Name = "Bar");
                try
                {
                    newGroup.AddLight(light);
                }
                catch (Exception e)
                {
                    _thrown = e;
                }
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _thrown.ShouldNotBeNull();
                _thrown.GetType().ShouldEqual(typeof (InvalidOperationException));
                _thrown.Message.ShouldEqual(
                    "Cannot add light to group BarDaddy.Bar because it already belongs to group FooDaddy.Foo");
            }
        }

        [TestFixture]
        public class When_removing_a_light_from_a_group
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _group = new MasterModel().CreateProject().CreateGroup();
                _light = new Light(1, 2, 123);

                _group.AddLight(_light);
                _group.RemoveLight(_light);
            }

            [Test]
            public void Should_remove_the_light_from_the_groups_collection()
            {
                _group.Lights.Length.ShouldEqual(0);
            }

            [Test]
            public void Should_remove_the_parent_reference_to_the_group()
            {
                _light.ParentGroup.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_adding_a_light_that_is_already_in_the_same_group
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _group = new MasterModel().CreateProject().CreateGroup();
                _light = new Light(1, 2, 123);

                _group.AddLight(_light);
                _group.AddLight(_light);
            }

            [Test]
            public void Should_keep_the_light_in_the_group_without_creating_a_duplicate()
            {
                _group.Lights.Length.ShouldEqual(1);
                _group.Lights[0].ZWaveHomeId.ShouldEqual((uint) 1);
                _group.Lights[0].ZWaveDeviceId.ShouldEqual((byte) 2);
            }
        }

        [TestFixture]
        public class When_attempting_to_remove_a_light_that_is_in_a_different_group
        {
            private LightGroup _bar;
            private Light _light;
            private Exception _thrown;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var barDaddy = masterModel.CreateProject(x => { x.Name = "BarDaddy"; });
                _bar = barDaddy.CreateGroup(x => x.Name = "Bar");

                var fooDaddy = masterModel.CreateProject(x => { x.Name = "FooDaddy"; });
                var foo = fooDaddy.CreateGroup(x => x.Name = "Foo");
                _light = new Light(1, 2, 123);


                _bar.AddLight(_light);

                try
                {
                    foo.RemoveLight(_light);
                }
                catch (Exception e)
                {
                    _thrown = e;
                }
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _thrown.GetType().ShouldEqual(typeof (InvalidOperationException));
                _thrown.Message.ShouldEqual(
                    "Cannot remove light from group FooDaddy.Foo because it belongs to group BarDaddy.Bar");
            }

            [Test]
            public void Should_not_change_the_parent_reference_on_the_light()
            {
                _light.ParentGroup.ShouldBeSameAs(_bar);
            }
        }
        [TestFixture]
        public class When_unassigning_all_lights
        {
            private MasterModel _masterModel;
            private LightGroup _group;

            [SetUp]
            public void ContextSetup()
            {
                _masterModel = new MasterModel();
                _group = _masterModel.CreateProject().CreateGroup();
                _group.AddLight(new Light(11, 22, 123));
                _group.AddLight(new Light(11, 44, 123));

                _group.UnassignAllLights();
            }

            [Test]
            public void Should_move_lights_to_unassigned_collection_on_master_model()
            {
                _group.Lights.Length.ShouldEqual(0);
                _masterModel.UnassignedLights.Length.ShouldEqual(2);
                _masterModel.UnassignedLights.Any(x => x.ZWaveDeviceId.Equals(22)).ShouldBeTrue();
                _masterModel.UnassignedLights.Any(x => x.ZWaveDeviceId.Equals(44)).ShouldBeTrue();
            }
        }
    }
}