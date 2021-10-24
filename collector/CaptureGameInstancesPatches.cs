using System;
using Bindito.Core;
using HarmonyLib;
using Timberborn.FactionSystemGame;
using Timberborn.Goods;
using Timberborn.MapSystem;
using Timberborn.MasterScene;
using Timberborn.ResourceCountingSystem;
using Timberborn.SettlementNameSystem;
using Timberborn.SettlementNameSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;
using VeVantZeData.Collector.GameAdapters;

namespace VeVantZeData.Collector
{
    class CaptureGameInstancesPatches
    {
        [HarmonyPatch(typeof(EventBus), MethodType.Constructor)]
        public static class CaptureEventBus
        {
            private static void Postfix(EventBus __instance)
            {
                EntityListener.Init(__instance);
            }
        }

        [HarmonyPatch(typeof(MapLoader), "Load")]
        public static class CaptureLoadedMap
        {
            private static void Postfix(MapFileReference mapFileReference)
            {
                TimberbornGame.CurrentMapName = mapFileReference.DisplayName;
            }
        }

        [HarmonyPatch(typeof(FactionService), "Load")]
        public static class CaptureLoadedFaction
        {
            private static void Postfix(FactionService __instance)
            {
                TimberbornGame.CurrentFactionName = __instance.Current.DisplayName;
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

        [HarmonyPatch(typeof(SettlementNameService), "Load")]
        public static class SettlementNameServiceLoad
        {
            private static void Postfix(SettlementNameService __instance)
            {
                TimberbornGame.SettlementName = __instance.SettlementName;
            }
        }

        [HarmonyPatch(typeof(SettlementNameBoxShower), "ShowDisallowingCancelling")]
        public static class SettlementNameBoxShowerShowDisallowingCancelling
        {
            private static void Prefix(ref Action<string> confirmButtonCallback)
            {
                confirmButtonCallback = DecorateCallback(confirmButtonCallback);
            }

            private static Action<string> DecorateCallback(Action<string> confirmButtonCallback)
            {
                return name =>
                {
                    TimberbornGame.SettlementName = name;
                    confirmButtonCallback.Invoke(name);
                };
            }
        }
    }
}