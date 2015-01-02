using System;
using System.Linq;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Web.Extensions;

namespace ZBuildLights.Web.Models.Admin
{
    public class AdminViewModel
    {
        public AdminViewModel()
        {
            Projects = new AdminProjectViewModel[0];
        }

        public AdminProjectViewModel[] Projects { get; set; }
        public AdminLightGroupViewModel Unassigned { get; set; }

        public bool NoProjects
        {
            get { return Projects.Length == 0; }
        }

        public AdminLightGroupViewModel[] AllGroups
        {
            get { return Projects.SelectMany(x => x.Groups).OrderBy(x => x.FullName).ToArray(); }
        }
    }

    public class AdminProjectViewModel
    {
        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }
        public Guid Id { get; set; }

        public AdminLightGroupViewModel[] Groups { get; set; }

        public string HeaderId
        {
            get { return string.Format("panel-header-{0}", Name.ToSafeId()); }
        }

        public string PanelId
        {
            get { return string.Format("panel-body-{0}", Name.ToSafeId()); }
        }
    }

    public class AdminLightGroupViewModel
    {
        public string Name { get; set; }
        public AdminLightViewModel[] Lights { get; set; }
        public Guid Id { get; set; }
        public string ParentProjectName { get; set; }

        public string FullName
        {
            get { return string.Format("{0}.{1}", ParentProjectName, Name); }
        }
    }

    public class AdminLightViewModel
    {
        public LightColor Color { get; set; }

        public SwitchState SwitchState { get; set; }
        public byte ZWaveDeviceId { get; private set; }
        public uint ZWaveHomeId { get; private set; }
    }
}