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

        protected Exception ExpectException(Action action)
        {
            return ExpectException<Exception>(action);
        }

        protected TException ExpectException<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException e)
            {
                return e;
            }

            return null;
        }
    }
}