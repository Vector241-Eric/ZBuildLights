namespace ZBuildLights.Web.Extensions
{
    public static class ViewExtensions
    {
        public static string ToJqueryId(this string s)
        {
            return string.Format("#{0}", s);
        }

        public static string ToSafeId(this string s)
        {
            return s.ToLowerInvariant().Replace(" ", "_");
        }
    }
}