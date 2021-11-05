using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace VeVantZeData.Collector
{
    [BepInPlugin("com.thundersen.vevantzedata.collector", "Ve Vant Ze Data Timberborn!", "0.1.4")]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; }
        private Collector _collector;
        private AlertManager _alertManager;

        private void Awake()
        {
            Log = base.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var config = new VeVantZeDataConfig(Config);
            _collector = new Collector(config);
            _alertManager = new AlertManager(config);


            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                // Get just the name of assmebly
                // Aseembly name excluding version and other metadata
                //string name = new Regex(",.*").Replace(args.Name, string.Empty);

                Log.LogDebug($">>>>>> loading of assembly failed: {args.Name}");

                var assemblies = string.Join("\n", AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().ToString().ToLower().Contains("newtonsoft")));

                Log.LogDebug($">>>>>> available: \n{assemblies}");

                // Load whatever version available
                return Assembly.Load(args.Name);
            };



            Logger.LogInfo($"Plugin com.thundersen.vevantzedata.collector is loaded!");
        }

        void Update()
        {
            _collector.Update();
            _alertManager.Update();
        }
    }
}
