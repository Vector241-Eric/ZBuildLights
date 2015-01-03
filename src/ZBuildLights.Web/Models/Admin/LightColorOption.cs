using System.Linq;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Web.Models.Admin
{
    public class LightColorOption
    {
        public const string CssClassPrefix = "light-option-";

        public string Text { get; set; }
        public int Id { get; set; }
        public string CssClass { get; set; }

        public static LightColorOption[] GetAll()
        {
            return LightColor.GetAll()
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new LightColorOption
                {
                    Text = x.DisplayName,
                    Id = x.Value,
                    CssClass = string.Format("{0}{1}", CssClassPrefix, MakeDisplayClass(x))
                })
                .ToArray();
        }

        private static string MakeDisplayClass(LightColor x)
        {
            return x.DisplayName.ToLowerInvariant().Replace("<", string.Empty).Replace(">", string.Empty);
        }
    }
}