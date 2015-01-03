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
        public class When_moving_a_light_from_one_group_to_another
        {
            private LightGroup _fooGroup;
            private LightGroup _barGroup;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _fooGroup = new LightGroup {Name = "Foo", ParentProject = new Project {Name = "FooDaddy"}};
                _light = new Light(1, 2);
                _fooGroup.AddLight(_light);

                _barGroup = new LightGroup {Name = "Bar", ParentProject = new Project {Name = "BarDaddy"}};
                _light.MoveTo(_barGroup);
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
        public class When_moving_a_light_that_is_not_already_in_a_group
        {
            private LightGroup _barGroup;
            private Light _light;

            [SetUp]
            public void ContextSetup()
            {
                _light = new Light(1, 2);

                _barGroup = new LightGroup {Name = "Bar", ParentProject = new Project {Name = "BarDaddy"}};
                _light.MoveTo(_barGroup);
            }

            [Test]
            public void Should_gracefully_add_the_light_to_the_new_group()
            {
                _light.ParentGroup.ShouldBeSameAs(_barGroup);
            }
        }
    }
}