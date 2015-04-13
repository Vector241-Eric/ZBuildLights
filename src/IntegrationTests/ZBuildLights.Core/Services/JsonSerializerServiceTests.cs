using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Enumerations;
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

                var masterModel = new MasterModel {LastUpdatedDate = DateTime.Now};

                var server1 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/1";
                    x.Name = "One";
                });
                var server2 = masterModel.CreateCruiseServer(x =>
                {
                    x.Url = "http://www.example.com/2";
                    x.Name = "Two";
                });

                var core = masterModel.CreateProject(x =>
                {
                    x.Name = "Core";
                    x.StatusMode = StatusMode.Success;
                    x.CcXmlUrl = "http://someserver:8888/cc.xml";
                    x.CruiseProjectAssociations = new[]
                    {
                        new CruiseProjectAssociation {Name = "I like toast", ServerId = server1.Id},
                        new CruiseProjectAssociation {Name = "I like jam", ServerId = server2.Id}
                    };
                });
                core.CreateGroup(x => x.Name = "SnP Square")
                    .AddLight(new Light(new ZWaveIdentity(1, 1, 123)) {Color = LightColor.Green, SwitchState = SwitchState.On})
                    .AddLight(new Light(new ZWaveIdentity(1, 2, 123)) { Color = LightColor.Yellow, SwitchState = SwitchState.Off })
                    .AddLight(new Light(new ZWaveIdentity(1, 3, 123)) { Color = LightColor.Red, SwitchState = SwitchState.Off })
                    ;
                core.CreateGroup(x => x.Name = "SnP Near Matt")
                    .AddLight(new Light(new ZWaveIdentity(1, 4, 123)) { Color = LightColor.Green, SwitchState = SwitchState.On })
                    .AddLight(new Light(new ZWaveIdentity(1, 5, 123)) { Color = LightColor.Yellow, SwitchState = SwitchState.Off })
                    .AddLight(new Light(new ZWaveIdentity(1, 6, 123)) { Color = LightColor.Red, SwitchState = SwitchState.Off })
                    ;

                var apps = masterModel.CreateProject(x =>
                {
                    x.StatusMode = StatusMode.BrokenAndBuilding;
                    x.Name = "Apps";
                });
                apps.CreateGroup(x => x.Name = "Near Window")
                    .AddLight(new Light(new ZWaveIdentity(1, 7, 123)) { Color = LightColor.Green, SwitchState = SwitchState.Off })
                    .AddLight(new Light(new ZWaveIdentity(1, 8, 123)) { Color = LightColor.Yellow, SwitchState = SwitchState.On })
                    .AddLight(new Light(new ZWaveIdentity(1, 9, 123)) { Color = LightColor.Red, SwitchState = SwitchState.On })
                    ;

                masterModel.AddUnassignedLight(new Light(new ZWaveIdentity(3333, 3, 123)));

                //Act
                var serializer = container.GetInstance<IJsonSerializerService>();
                var json = serializer.SerializeMasterModel(masterModel);

                Console.WriteLine(json);

                var deserialized = serializer.DeserializeMasterModel(json);

                var comparer =
                    new CompareLogic(new ComparisonConfig
                    {
                        MembersToIgnore = new List<string> {"SwitchState", "StatusMode"}
                    });
                var result = comparer.Compare(masterModel, deserialized);

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