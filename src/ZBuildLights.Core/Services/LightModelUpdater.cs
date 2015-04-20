using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class LightModelUpdater : ILightModelUpdater
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public LightModelUpdater(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
        }

        public void Update(ZWaveIdentity identity, Guid groupId, int colorId)
        {
            var masterModel = _masterModelRepository.GetCurrent();
            var light = FindLight(identity, masterModel);

            //Set color
            light.Color = LightColor.FromValue(colorId);
            
            //Set group
            if (groupId == Guid.Empty)
            {
                light.Unassign();
                masterModel.AddUnassignedLight(light);
            }
            else
            {
                masterModel.AssignLightToGroup(identity, groupId);
            }

            //Save
            _masterModelRepository.Save(masterModel);
        }

        private Light FindLight(ZWaveIdentity identity, MasterModel masterModel)
        {
            var light = masterModel.AllLights
                .SingleOrDefault(x => x.ZWaveIdentity.Equals(identity));
            if (light == null)
                throw new InvalidOperationException(
                    string.Format("Could not find light with identity: {0}", identity));
            return light;
        }
    }
}