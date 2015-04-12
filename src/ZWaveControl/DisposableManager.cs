using System;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public class DisposableManager : IDisposable
    {
        private readonly ZWManager _manager;

        public DisposableManager(ZWManager instance)
        {
            _manager = instance;
        }

        public void Dispose()
        {
            _manager.Destroy();
        }

        public static implicit operator ZWManager(DisposableManager helper)
        {
            return helper._manager;
        }
    }
}