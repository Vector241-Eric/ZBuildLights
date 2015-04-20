using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services.CruiseControl;

namespace ZBuildLights.Core.Services
{
    public class ProjectStatusUpdater : IProjectStatusUpdater
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ICcReader _cruiseReader;

        public ProjectStatusUpdater(ICcReader cruiseReader)
        {
            _cruiseReader = cruiseReader;
        }

        public void UpdateAllProjectStatuses(MasterModel masterModel)
        {
            var projects = masterModel.Projects;
            var serverIds = projects.SelectMany(x => x.CruiseProjectAssociations)
                .Select(x => x.ServerId)
                .Distinct()
                .ToArray();

            var servers = masterModel.CruiseServers.Where(x => serverIds.Contains(x.Id));
            var serverStatuses = servers.ToDictionary(server => server.Id, server => _cruiseReader.GetStatus(server.Url));

            foreach (var project in projects)
            {
                var cruiseProjectActivities =
                    GetProjectValues(project, serverStatuses, x => x.activity, CcBuildActivity.Sleeping);
                var cruiseProjectStatuses =
                    GetProjectValues(project, serverStatuses, x => x.lastBuildStatus, CcBuildStatus.Unknown);

                var somethingIsBuilding = cruiseProjectActivities.Any(x => x == CcBuildActivity.Building);
                var somethingIsBroken = cruiseProjectStatuses.Any(x => x == CcBuildStatus.Failure);
                var somethingIsNotConnected = cruiseProjectStatuses.Any(x => x == CcBuildStatus.Unknown);
                var allAreSuccessful = cruiseProjectStatuses.All(x => x == CcBuildStatus.Success);

                if (somethingIsNotConnected)
                    project.StatusMode = StatusMode.NotConnected;
                else if (somethingIsBuilding && allAreSuccessful)
                    project.StatusMode = StatusMode.SuccessAndBuilding;
                else if (!somethingIsBuilding && allAreSuccessful)
                    project.StatusMode = StatusMode.Success;
                else if (somethingIsBuilding && somethingIsBroken)
                    project.StatusMode = StatusMode.BrokenAndBuilding;
                else
                    project.StatusMode = StatusMode.Broken;
            }
        }

        private TEnum[] GetProjectValues<TEnum>(Project project,
            IDictionary<Guid, NetworkResponse<Projects>> serverStatuses,
            Func<ProjectsProject, string> getValueFromProject, TEnum defaultValue) where TEnum : struct
        {
            var result = project.CruiseProjectAssociations
                .Select(cpa =>
                {
                    var networkResponse = serverStatuses[cpa.ServerId];
                    if (networkResponse.IsSuccessful)
                    {
                        var cruiseProjects = networkResponse.Data.Items;
                        return GetValueFromCruiseProject(cruiseProjects, cpa, getValueFromProject, defaultValue);
                    }
                    return defaultValue;
                }).ToArray();
            return result;
        }

        private static TEnum GetValueFromCruiseProject<TEnum>(ProjectsProject[] cruiseProjects,
            CruiseProjectAssociation cpa, Func<ProjectsProject, string> getValueFromProject, TEnum defaultValue)
            where TEnum : struct
        {
            var cruiseProject = cruiseProjects.SingleOrDefault(x => x.name.Equals(cpa.Name));
            if (cruiseProject == null)
            {
                Log.Error("Could not find project with name '{0}'", cpa.Name);
                return defaultValue;
            }
            var valueString = getValueFromProject(cruiseProject);
            TEnum value;
            var couldParse = Enum.TryParse(valueString, true, out value);
            if (couldParse)
                return value;
            Log.Error("Could not parse enumeration of type '{0}' from value '{1}'", typeof (TEnum).Name, valueString);
            return defaultValue;
        }
    }
}