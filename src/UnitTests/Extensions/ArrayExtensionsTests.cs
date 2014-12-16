using NUnit.Framework;
using Should;
using ZBuildLights.Core.Extensions;

namespace UnitTests.Extensions
{
    public class ArrayExtensionsTests
    {
        [TestFixture]
        public class When_adding_to_end_of_array
        {
            [Test]
            public void Should_create_new_array_if_existing_is_null()
            {
                var result = ((string[]) null).AddToEnd("foo");
                result.ShouldNotBeNull();
                result.Length.ShouldEqual(1);
                result.ShouldContain("foo");
            }

            [Test]
            public void Should_add_to_end_of_existing_array()
            {
                var existing = new[] {"one", "two"};
                var result = existing.AddToEnd("three");

                result.Length.ShouldEqual(3);
                result[0].ShouldEqual("one");
                result[1].ShouldEqual("two");
                result[2].ShouldEqual("three");
            }
        } 
    }
}