using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace UnitTests._Stubs
{
    public class StubMasterModelRepository : IMasterModelRepository
    {
        private MasterModel _currentModel;

        public void UseCurrentModel(MasterModel model)
        {
            _currentModel = model;
        }

        public MasterModel GetCurrent()
        {
            return _currentModel;
        }

        public void Save(MasterModel model)
        {
            LastSaved = model;
        }

        public MasterModel LastSaved { get; private set; }
    }
}