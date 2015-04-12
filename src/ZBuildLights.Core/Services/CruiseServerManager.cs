using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public class CruiseServerManager : ICruiseServerManager
    {
        private readonly IMasterModelRepository _repository;

        public CruiseServerManager(IMasterModelRepository repository)
        {
            _repository = repository;
        }

        public CreationResult<CruiseServer> Create(string name, string url)
        {
            var masterModel = _repository.GetCurrent();
            if (string.IsNullOrWhiteSpace(name))
                return CreationResult.Fail<CruiseServer>("'Name' is a required property for cruise server");
            if (masterModel.CruiseServers.Any(x => name.Equals(x.Name)))
                return CreationResult.Fail<CruiseServer>("A cruise server with this name already exists");

            var cruiseServer = masterModel.CreateCruiseServer(x =>
            {
                x.Name = name;
                x.Url = url;
            });
            _repository.Save(masterModel);
            return CreationResult.Success(cruiseServer);
        }

        public EditResult<CruiseServer> Update(Guid id, string name, string url)
        {
            throw new NotImplementedException();
        }

        public EditResult<CruiseServer> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}