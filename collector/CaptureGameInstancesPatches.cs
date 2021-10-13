using Bindito.Core;
using HarmonyLib;
using Timberborn.FactionSystemGame;
using Timberborn.Goods;
using Timberborn.MapSystem;
using Timberborn.MasterScene;
using Timberborn.ResourceCountingSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;

namespace VeVantZeData.Collector
{
    class CaptureGameInstancesPatches
    {
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

        [HarmonyPatch(typeof(DayNightCycle), "Load")]
        public static class DayNightCycleLoad
        {
            private static void Postfix(DayNightCycle __instance)
            {
                TimberbornGame.DayNightCycle = __instance;
            }
        }

        [HarmonyPatch(typeof(WeatherService), "Load")]
        public static class WeatherServiceLoad
        {
            private static void Postfix(WeatherService __instance)
            {
                TimberbornGame.WeatherService = __instance;
            }
        }

        [HarmonyPatch(typeof(GoodService), "Initialize")]
        public static class GoodServiceInitialize
        {
            private static void Postfix(GoodService __instance)
            {
                TimberbornGame.GoodSpecs = __instance.GetGoodSpecifications();
            }
        }

        [HarmonyPatch(typeof(ResourceCountingService), "PostLoad")]
        public static class ResourceCountingServicePostLoad
        {
            private static void Postfix(ResourceCountingService __instance)
            {
                TimberbornGame.ResourceCountingService = __instance;
            }
        }
    }
}