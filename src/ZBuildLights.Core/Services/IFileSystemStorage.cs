using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IFileSystemStorage
    {
        void Save(MasterModel model);
        MasterModel ReadMasterModel();
    }
}