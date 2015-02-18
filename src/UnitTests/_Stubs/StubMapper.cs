using System;
using System.Collections.Generic;
using ZBuildLights.Core.Wrappers;

namespace UnitTests._Stubs
{
    public class StubMapper : IMapper
    {
        private readonly Dictionary<Type, object> _stubs = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _mappingInputs = new Dictionary<Type, object>(); 
 
        public TResult Map<TSource, TResult>(TSource source)
        {
            _mappingInputs[typeof(TSource)] = source;
            return (TResult) _stubs[typeof (TResult)];
        }

        public TSource LastMapInput<TSource>()
        {
            return (TSource) _mappingInputs[typeof(TSource)];
        }

        public StubMapper StubResult<TResult>(TResult result)
        {
            _stubs[typeof (TResult)] = result;
            return this;
        }
    }
}