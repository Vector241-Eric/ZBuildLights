using System;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace UnitTests._Stubs
{
    internal class StorageStub : IFileSystemStorage
    {
        public void Save(MasterModel model)
        {
            LastSaved = model;
        }

        public MasterModel LastSaved { get; private set; }

        public MasterModel ReadMasterModel()
        {
            throw new NotImplementedException();
        }
    }
}