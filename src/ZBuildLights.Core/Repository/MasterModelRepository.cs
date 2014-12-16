using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace ZBuildLights.Core.Repository
{
    public class MasterModelRepository : IMasterModelRepository
    {
        private readonly IFileSystemStorage _fileStorage;

        public MasterModelRepository(IFileSystemStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public MasterModel GetCurrent()
        {
            var fromDisk = _fileStorage.ReadMasterModel();
            return fromDisk ?? InitializeMasterModel();
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