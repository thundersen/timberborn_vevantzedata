using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using Timberborn.GameDistricts;
using Timberborn.Characters;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.timberborn.collector", "Ve Vant Ze Data Timberborn!", "0.0.0.1")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        private static Collector _collector;
        private static Writer _writer;
        private static EntityInitializationListener _entityInitializationListener;
        private static bool _gameStarted;

        private static Playthrough _playthrough;
        internal static Playthrough Playthrough
        {
            private get => _playthrough;
            set
            {
                _playthrough = value;
                Log.LogDebug($"Updated playthrough. Assuming new game has started.");
                OnGameStart();
            }
        }

        private static GlobalPopulation _globalPopulation;
        internal static GlobalPopulation GlobalPopulation
        {
            private get => _globalPopulation;
            set
            {
                _globalPopulation = value;
                Log.LogDebug($"Updated global pop.");
            }
        }

        internal static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.timberborn.collector is loaded!");
        }

        void Update()
        {
            secondsSinceLastInterval += Time.deltaTime;
            if (secondsSinceLastInterval > intervalSeconds)
            {
                CollectIfInitialized();
                secondsSinceLastInterval = 0.0f;
            }
        }

        void CollectIfInitialized()
        {
            if (!_gameStarted || Playthrough == null || !_writer.IsInitialized())
                return;

            var data = _collector.Collect();
            _writer.Write(data);
        }

        private static void OnGameStart()
        {
            _gameStarted = true;
            _collector = new Collector(GlobalPopulation);
            _writer = new Writer(Playthrough);
            Log.LogDebug("Reset after game start.");
        }

        internal static void SetEventBus(EventBus eventBus)
        {
            if (_entityInitializationListener != null)
                _entityInitializationListener.TearDown();

            _entityInitializationListener = new EntityInitializationListener(eventBus, AddDistrictCenterToCollector);
        }

        private static void AddDistrictCenterToCollector(EntityComponent ec)
        {
            if (!ec.HasDistrictCenter())
                return;

            var dc = (DistrictCenter)ec.RegisteredComponents.First(c => c.GetType() == typeof(DistrictCenter));

            Log.LogDebug($"adding dc to collector {(dc).DistrictName}");

            _collector.AddDistrictCenter(dc);
        }
    }
}
