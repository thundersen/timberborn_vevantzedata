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

        internal static Playthrough Playthrough { private get; set; }
        internal static GlobalPopulation GlobalPopulation { private get; set; }

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

        internal static void OnGameStart()
        {
            _gameStarted = true;
            _collector = new Collector(GlobalPopulation);
            _writer = new Writer(Playthrough);
            Log.LogInfo("New game started. Resetting.");
            Log.LogDebug("debuggo.");
        }

        internal static void SetEventBus(EventBus eventBus)
        {
            if (_entityInitializationListener != null)
                _entityInitializationListener.TearDown();

            _entityInitializationListener = new EntityInitializationListener(eventBus, AddDistrictCenterToCollector);
        }

        private static void AddDistrictCenterToCollector(EntityComponent ec) {
            if (!ec.HasDistrictCenter())
                 return;

            var dc = ec.RegisteredComponents.First(c => c.GetType() == typeof(DistrictCenter));
            _collector.AddDistrictCenter((DistrictCenter)dc);
        }
    }
}
