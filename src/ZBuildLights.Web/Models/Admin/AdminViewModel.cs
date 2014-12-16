using System;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Web.Models.Admin
{
    public class AdminViewModel
    {
        public AdminProjectViewModel[] Projects { get; set; }
        public AdminLightGroupViewModel Unassigned { get; set; }
    }

    public class AdminProjectViewModel
    {
        public string Name { get; set; }
        public StatusMode StatusMode { get; set; }
        public Guid Id { get; set; }

        public AdminLightGroupViewModel[] Groups { get; set; }

        public string HeaderId  
        {
            get { return string.Format("panel-header-{0}", Name); }
        }

        public string PanelId
        {
            get { return string.Format("panel-body-{0}", Name); }
        }
    }

    public class AdminLightGroupViewModel
    {
        public string Name { get; set; }
        public AdminLightViewModel[] Lights { get; set; }
    }

    public class AdminLightViewModel
    {
        public LightColor Color { get; set; }

        public SwitchState SwitchState { get; set; }
    }
}