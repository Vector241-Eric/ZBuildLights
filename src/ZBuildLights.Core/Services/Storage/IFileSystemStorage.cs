using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services.Storage
{
    public interface IFileSystemStorage
    {
        void Save(MasterModel model);
        MasterModel ReadMasterModel();
    }
}