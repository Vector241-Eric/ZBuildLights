using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Builders;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Storage;
using ZBuildLights.Web;
using ZBuildLights.Web.DependencyResolution;

namespace IntegrationTests.ZBuildLights.Core.Services
{
    public class JsonSerializerServiceTests
    {
        [TestFixture]
        public class When_working_with_the_master_model_object
        {
            [Test]
            public void Should_serialize_and_deserialize()
            {
                AutoMapperConfig.Initialize();
                var container = IoC.Initialize();

                var projects = new StubStatusProvider().GetCurrentProjects();
                var testData = new MasterModel {LastUpdatedDate = DateTime.Now, Projects = projects};
                testData.AddUnassignedLight(new Light(3333, 3));

                var serializer = container.GetInstance<IJsonSerializerService>();
                var json = serializer.SerializeMasterModel(testData);

                Console.WriteLine(json);

                var deserialized = serializer.DeserializeMasterModel(json);

                var comparer =
                    new CompareLogic(new ComparisonConfig
                    {
                        MembersToIgnore = new List<string> {"SwitchState", "StatusMode"}
                    });
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