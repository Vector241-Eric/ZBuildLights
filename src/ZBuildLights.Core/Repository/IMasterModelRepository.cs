using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Repository
{
    public interface IMasterModelRepository
    {
        MasterModel GetCurrent();
        void Save(MasterModel model);
    }
}