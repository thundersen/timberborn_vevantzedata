using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.timberborn.collector", "Ve Vant Ze Data Timberborn!", "0.0.0.1")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private const float intervalSeconds = 10.0f;
        private float secondsSinceLastInterval;
        internal static ManualLogSource Log;
        private static Collector _collector = new Collector();
        private static Writer _writer = new Writer();
        private static EntityInitializationListener _entityInitializationListener;

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
            if (!_writer.IsInitialized())
                return;

            var data = _collector.Collect();
            _writer.Write(data);
        }

        internal static void SetEventBus(EventBus eventBus)
        {
            if (_entityInitializationListener != null)
                _entityInitializationListener.TearDown();

            _entityInitializationListener = new EntityInitializationListener(eventBus, SetPlaythroughFromDistrictCenters);
        }

        private static void SetPlaythroughFromDistrictCenters(EntityComponent ec)
        {
            if (!ec.HasDistrictCenter())
                return;

            _writer.Update(Playthrough.FromDistrictCenter(ec));
        }
    }
}
