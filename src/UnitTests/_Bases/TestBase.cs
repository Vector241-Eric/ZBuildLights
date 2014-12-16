using System;
using Rhino.Mocks;
using ZBuildLights.Core.Services;

namespace UnitTests._Bases
{
    public class TestBase
    {
        protected T S<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        protected void SetSystemClock(DateTime now)
        {
            SystemClock.Now = () => now;
        }
    }
}