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
            public void Lights_with_same_device_and_home_ids_should_be_equal()
            {
                var light1 = new Light(10, 20);
                var light2 = new Light(10, 20);

                light1.ShouldEqual(light2);
            }

            [Test]
            public void Lights_with_different_home_ids_should_not_be_equal()
            {
                var light1 = new Light(100, 20);
                var light2 = new Light(10, 20);

                light1.ShouldNotEqual(light2);
            }

            [Test]
            public void Lights_with_different_device_ids_should_not_be_equal()
            {
                var light1 = new Light(10, 20);
                var light2 = new Light(10, 200);

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
                _group = new LightGroup {Name = "Foo", ParentProject = new Project {Name = "FooDaddy"}};
                _light = new Light(1, 2);
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
                new Light(1, 2).Unassign();
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
                _light = new Light(43, 55);
                new LightGroup().AddLight(_light);
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
                _light = new Light(43, 55);
            }

            [Test]
            public void Should_indicate_it_is_in_a_group()
            {
                _light.IsInGroup.ShouldBeFalse();
            } 
        }
    }
}