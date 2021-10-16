using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.collector", "Ve Vant Ze Data Timberborn!", "0.1.0.0")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; }
        private Collector _collector;

        private void Awake()
        {
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            _collector = new Collector(new VeVantZeDataConfig(Config));

            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.collector is loaded!");
        }

        void Update()
        {
            _collector.Update();
        }
    }
}
