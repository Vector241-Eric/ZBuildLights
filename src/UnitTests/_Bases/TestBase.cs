using Rhino.Mocks;

namespace UnitTests._Bases
{
    public class TestBase
    {
        protected T S<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }
    }
}