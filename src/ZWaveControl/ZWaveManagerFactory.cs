using System;
using System.Diagnostics;
using System.Threading;
using NLog;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public static class ZWaveManagerFactory
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static ZWManager _instance;
        private static bool _destroyed;

        private static readonly ZWaveSettings ZWaveSettings = new ZWaveSettings();

        private static readonly ZWOptions _options = new ZWOptions();
        private static readonly object _lock = new object();

        private static bool _allNodesQueried;
        private static readonly TimeSpan MaxQuietWait = TimeSpan.FromMinutes(2);

        public static ZWManager GetInstance()
        {
            lock (_lock)
            {
                if (_destroyed)
                    throw new InvalidOperationException("Factory cannot be used after it has been destroyed.");
                if (_instance == null)
                    _instance = Create();
            }
            return _instance;
        }

        public static void Destroy()
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    _instance.Destroy();
                    _instance = null;
                }
                _destroyed = true;
            }
        }

        private static ZWManager Create()
        {
            SetOptions();

            var manager = new ZWManager();
            // create the OpenZWave Manager
            manager.Create();
            manager.OnNotification += notification =>
            {
                ZWaveNotificationHandler.HandleNotification(notification, manager);
                var type = notification.GetType();
                if (type.Equals(ZWNotification.Type.AllNodesQueriedSomeDead) ||
                    type.Equals(ZWNotification.Type.AllNodesQueried))
                    _allNodesQueried = true;
            };
            manager.OnControllerStateChanged += state =>    
            {
                Log.Debug(state);
            };
            // once the driver is added it takes some time for the device to get ready
            manager.AddDriver(ZWaveSettings.ControllerComPort);
            WaitForZWaveToInitialize();
            return manager;
        }

        private static void WaitForZWaveToInitialize()
        {
            Log.Info("Waiting for zWave network to initialize and stabilize (Max wait: {0}).", MaxQuietWait);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(TimeSpan.FromSeconds(5));
            while (!_allNodesQueried)
            {
                if (stopwatch.Elapsed > MaxQuietWait)
                    throw new Exception("Timed out waiting to ZWave network to get quiet");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Log.Info("zWave initialization complete.");
        }

        private static void SetOptions()
        {
            // the directory the config files are copied to in the post build
            _options.Create(ZWaveSettings.ConfigurationPath, @"", @"");

            // logging options
            _options.AddOptionInt("SaveLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("QueueLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("DumpTriggerLevel", (int) ZWLogLevel.Error);

            // lock the options
            _options.Lock();
        }
    }
}