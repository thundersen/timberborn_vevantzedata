using Bindito.Core;
using HarmonyLib;
using Timberborn.Characters;
using Timberborn.FactionSystemGame;
using Timberborn.MapSystem;
using Timberborn.MasterScene;
using Timberborn.SingletonSystem;

namespace VeVantZeData.Collector
{
    class CaptureGameInstancesPatches
    {
        [HarmonyPatch(typeof(GlobalPopulation), "Load")]
        public static class CaptureGlobalPopulation
        {
            private static void Postfix(GlobalPopulation __instance)
            {
                Plugin.GlobalPopulation = __instance;
            }
        }

        [HarmonyPatch(typeof(EventBus), MethodType.Constructor)]
        public static class CaptureEventBus
        {
            private static void Postfix(EventBus __instance)
            {
                Plugin.SetEventBus(__instance);
            }
        }

        [HarmonyPatch(typeof(MapLoader), "Load")]
        public static class CaptureLoadedMap
        {
            private static void Postfix(MapFileReference mapFileReference)
            {
                Playthrough.MapName = mapFileReference.DisplayName;
            }
        }

        [HarmonyPatch(typeof(FactionService), "Load")]
        public static class CaptureLoadedFaction
        {
            private static void Postfix(FactionService __instance)
            {
                Playthrough.FactionName = __instance.Current.DisplayName;
            }
        }

        [HarmonyPatch(typeof(MasterSceneConfigurator), "Configure")]
        public static class MasterSceneBinditoRegistration
        {
            private static void Postfix(IContainerDefinition containerDefinition)
            {
                containerDefinition.Install(new VeVantZeDataConfigurator());
            }
        }

        [HarmonyPatch(typeof(MasterSceneLauncher), "Start")]
        public static class CaptureGameStart
        {
            private static void Postfix()
            {
                Plugin.OnGameStart();
            }
        }
    }
}