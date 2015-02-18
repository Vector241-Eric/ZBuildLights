using System.Linq;

namespace ZBuildLights.Core.Models.CruiseControl
{
    public class CcProjectCollectionViewModel
    {
        public CcProjectViewModel[] Items { private get; set; }

        public CcProjectViewModel[] Projects
        {
            get { return Items.OrderBy(x => x.Name).ToArray(); }
        }
    }
}