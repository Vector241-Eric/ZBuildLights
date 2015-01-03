using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Storage;

namespace ZBuildLights.Core.Repository
{
    public class MasterModelRepository : IMasterModelRepository
    {
        private readonly IFileSystemStorage _fileStorage;
        private readonly IUnassignedLightService _unassignedLightService;

        public MasterModelRepository(IFileSystemStorage fileStorage, IUnassignedLightService unassignedLightService)
        {
            _fileStorage = fileStorage;
            _unassignedLightService = unassignedLightService;
        }

        public MasterModel GetCurrent()
        {
            var fromDisk = _fileStorage.ReadMasterModel();
            var masterModel = fromDisk ?? InitializeMasterModel();
            _unassignedLightService.SetUnassignedLights(masterModel);
            return masterModel;
        }

        private static MasterModel InitializeMasterModel()
        {
            return new MasterModel{LastUpdatedDate = SystemClock.Now()};
        }

        public void Save(MasterModel model)
        {
            model.LastUpdatedDate = SystemClock.Now();
            _fileStorage.Save(model);
        }
    }
}