using System;
using ZBuildLights.Core.Configuration;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Wrappers;

namespace ZBuildLights.Core.Services
{
    public class FileSystemStorage : IFileSystemStorage
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializerService _jsonSerializer;
        private readonly IApplicationConfiguration _configuration;

        public FileSystemStorage(IFileSystem fileSystem, IJsonSerializerService jsonSerializer, IApplicationConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
            _configuration = configuration;
        }

        public void Save(MasterModel model)
        {
            var json = _jsonSerializer.SerializeMasterModel(model);
            _fileSystem.WriteAllText(_configuration.StorageFilePath, json);
        }

        public MasterModel ReadMasterModel()
        {
            var path = _configuration.StorageFilePath;
            if (!_fileSystem.FileExists(path))
                return null;
            var json = _fileSystem.ReadAllText(path);
            return _jsonSerializer.DeserializeMasterModel(json);
        }
    }
}