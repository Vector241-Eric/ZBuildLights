namespace ZBuildLights.Web.Services
{
    public interface IMapper
    {
        TResult Map<TSource, TResult>(TSource source);
    }
}