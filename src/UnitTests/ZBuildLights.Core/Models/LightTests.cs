using NUnit.Framework;
using Should;
using ZBuildLights.Core.Models;

namespace UnitTests.ZBuildLights.Core.Models
{
    public class LightTests
    {
        [TestFixture]
        public class When_checking_equality
        {
            [Test]
            public void Lights_with_same_node_and_home_and_value_ids_should_be_equal()
            {
                var light1 = new Light(new ZWaveValueIdentity(10, 20, 123));
                var light2 = new Light(new ZWaveValueIdentity(10, 20, 123));

                light1.ShouldEqual(light2);
            }

            [Test]
            public void Lights_with_different_home_ids_should_not_be_equal()
            {
                var light1 = new Light(new ZWaveValueIdentity(100, 20, 123));
                var light2 = new Light(new ZWaveValueIdentity(10, 20, 123));

                light1.ShouldNotEqual(light2);
            }

            [Test]
            public void Lights_with_different_node_ids_should_not_be_equal()
            {
                var light1 = new Light(new ZWaveValueIdentity(10, 20, 123));
                var light2 = new Light(new ZWaveValueIdentity(10, 200, 123));

                light1.ShouldNotEqual(light2);
            }

            [Test]
            public void Lights_with_different_value_ids_should_not_be_equal()
            {
                var light1 = new Light(new ZWaveValueIdentity(10, 20, 111));
                var light2 = new Light(new ZWaveValueIdentity(10, 20, 222));

                light1.ShouldNotEqual(light2);
            }
        }

        [TestFixture]
        public class When_unassigning_a_light
        {
            private LightGroup _group;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                var masterModel = new MasterModel();
                var fooDaddy = masterModel.CreateProject(x => x.Name = "FooDaddy");
                _group = fooDaddy.CreateGroup(x => x.Name = "Foo");
                _light = new Light(new ZWaveValueIdentity(1, 2, 123));
                _group.AddLight(_light);

                _light.Unassign();
            }

            [Test]
            public void Should_remove_the_light_from_the_original_group()
            {
                _group.Lights.Length.ShouldEqual(0);
            }

            [Test]
            public void Should_clear_the_parent_group_reference()
            {
                _light.ParentGroup.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_unassigning_a_light_that_is_not_already_assigned_to_a_group
        {
            [SetUp]
            public void ContextSetup()
            {
                new Light(new ZWaveValueIdentity(1, 2, 123)).Unassign();
            }

            [Test]
            public void Should_not_throw_an_exception()
            {
                Assert.Pass("If we made it this far, we passed.");
            }
        }

        [TestFixture]
        public class When_light_has_been_assigned_to_a_group
        {
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _light = new Light(new ZWaveValueIdentity(43, 55, 123));
                new MasterModel().CreateProject().CreateGroup().AddLight(_light);
            }

            [Test]
            public void Should_indicate_it_is_in_a_group()
            {
                _light.IsInGroup.ShouldBeTrue();
            } 
        }

        [TestFixture]
        public class When_light_has_not_been_assigned_to_a_group
        {
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _light = new Light(new ZWaveValueIdentity(43, 55, 123));
            }

            [Test]
            public void Should_indicate_it_is_in_a_group()
            {
                _light.IsInGroup.ShouldBeFalse();
            } 
        }
    }
}