namespace ZBuildLights.Core.Wrappers
{
    public interface IMapper
    {
        TResult Map<TSource, TResult>(TSource source);
    }

    public class Mapper : IMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return AutoMapper.Mapper.Map<TDestination>(source);
        }
    }
}