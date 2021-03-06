﻿using System.IO;
using ZBuildLights.Core.Configuration;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Wrappers;

namespace ZBuildLights.Core.Services.Storage
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
            lock (this)
            {
                var json = _jsonSerializer.SerializeMasterModel(model);
                var storageFilePath = _configuration.StorageFilePath;
                var storageDirectory = Path.GetDirectoryName(storageFilePath);
                if (!_fileSystem.DirectoryExists(storageDirectory))
                    _fileSystem.CreateDirectory(storageDirectory);
                _fileSystem.WriteAllText(storageFilePath, json);
            }
        }

        public MasterModel ReadMasterModel()
        {
            lock (this)
            {
                var path = _configuration.StorageFilePath;
                if (!_fileSystem.FileExists(path))
                    return null;
                var json = _fileSystem.ReadAllText(path);
                return _jsonSerializer.DeserializeMasterModel(json);
            }
        }
    }
}