using System;
using System.Diagnostics;
using System.Threading;
using NLog;
using OpenZWaveDotNet;

namespace ZWaveControl
{
    public interface IZWaveManagerFactory
    {
        ZWManager GetManager();
    }

    public class ZWaveManagerFactory : IZWaveManagerFactory
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ZWOptions _options = new ZWOptions();
        private bool _allNodesQueried;

        private static readonly object _lock = new object();
        private static ZWManager _instance;
        private static bool _destroyed;

        private static readonly TimeSpan MaxQuietWait = TimeSpan.FromMinutes(2);

        private readonly IZWaveSettings _settings;
        private readonly IZWaveNodeList _nodeList;

        public ZWaveManagerFactory(IZWaveSettings settings, IZWaveNodeList nodeList)
        {
            _settings = settings;
            _nodeList = nodeList;
        }

        public ZWManager GetManager()
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

        private ZWManager Create()
        {
            SetOptions();

            var manager = new ZWManager();
            // create the OpenZWave Manager
            manager.Create();
            manager.OnNotification += new ZWaveNotificationHandler(_nodeList, manager).HandleNotification;
            manager.OnNotification += CheckAllNodesQueried;
            manager.OnControllerStateChanged += state => Log.Debug(state);

            // once the driver is added it takes some time for the device to get ready
            manager.AddDriver(@"\\.\" + _settings.ControllerComPort);
            WaitForZWaveToInitialize();
            return manager;
        }

        private void CheckAllNodesQueried(ZWNotification notification)
        {
            var type = notification.GetType();
            if (type.Equals(ZWNotification.Type.AllNodesQueriedSomeDead) ||
                type.Equals(ZWNotification.Type.AllNodesQueried))
                _allNodesQueried = true;
        }

        private void WaitForZWaveToInitialize()
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

        private void SetOptions()
        {
            // the directory the config files are copied to in the post build
            _options.Create(_settings.ConfigurationPath, @"", @"");

            // logging options
            _options.AddOptionInt("SaveLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("QueueLogLevel", (int) ZWLogLevel.Debug);
            _options.AddOptionInt("DumpTriggerLevel", (int) ZWLogLevel.Error);

            // lock the options
            _options.Lock();
        }
    }
}