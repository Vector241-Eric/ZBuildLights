using System;
using System.Security.Cryptography;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Should;
using TestDataGenerator;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Wrappers;

namespace IntegrationTests.ZBuildLights.Core.Wrappers
{
    public class JsonSerializerServiceTests
    {
        [TestFixture]
        public class When_working_with_the_master_model_object
        {
            public class GuidBuilder : IBuildInstances
            {
                public bool CanCreate(Type type)
                {
                    return type == typeof (Guid);
                }

                public object CreateInstance(Type type, string name)
                {
                    return Guid.NewGuid();
                }
            }

            public class TestModel
            {
                public string Name { get; set; }
            }

            [Test]
            public void Should_serialize_and_deserialize()
            {
                var catalog = new Catalog();
                catalog.RegisterCustomBuilder(new GuidBuilder());

                var testData = catalog.CreateInstance<TestModel>();

                var serializer = new JsonSerializerService();
                var json = serializer.Serialize(testData);

                Console.WriteLine(json);

                var deserialized = serializer.Deserialize<MasterModel>(json);

                var comparer = new CompareLogic();
                var result = comparer.Compare(testData, deserialized);

                if (!result.AreEqual)
                {
                    Console.WriteLine("Comparison failed!:");
                    Console.WriteLine("\t{0}", result.DifferencesString);
                    result.AreEqual.ShouldBeTrue();
                }
            } 
        } 
    }
}