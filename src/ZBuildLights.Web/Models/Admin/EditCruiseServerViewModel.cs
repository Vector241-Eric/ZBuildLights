using System;
using ZBuildLights.Web.Extensions;

namespace ZBuildLights.Web.Models.Admin
{
    public class EditCruiseServerViewModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public string HeaderId
        {
            get { return string.Format("panel-header-{0}", Name.ToSafeId()); }
        }

        public string PanelId
        {
            get { return string.Format("panel-body-{0}", Name.ToSafeId()); }
        }
    }
}