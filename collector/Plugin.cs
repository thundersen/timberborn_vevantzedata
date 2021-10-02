using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
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
        private Collector _collector;
        private Writer _writer;

        private void Awake()
        {
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.timberborn.collector is loaded!");

            _collector = new Collector();

            _writer = new Writer();
        }

        void Update()
        {
            secondsSinceLastInterval += Time.deltaTime;
            if (secondsSinceLastInterval > intervalSeconds)
            {
                var data = _collector.Collect();
                _writer.Write(data);
                secondsSinceLastInterval = 0.0f;
            }
        }
    }
}
