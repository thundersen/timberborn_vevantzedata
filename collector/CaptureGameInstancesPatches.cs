using HarmonyLib;
using Timberborn.Characters;
using Timberborn.GameDistricts;

namespace VeVantZeData.Collector
{
    class CaptureGameInstancesPatches
    {
        [HarmonyPatch(typeof(DistrictCenter), "Load")]
        public static class CaptureDistrictCenter
        {
            private static void Postfix(DistrictCenter __instance)
            {
                Collector.AddDistrictCenter(__instance);
            }
        }

        [HarmonyPatch(typeof(GlobalPopulation), "Load")]
        public static class CaptureGlobalPopulation
        {
            private static void Postfix(GlobalPopulation __instance)
            {
                Collector.SetGlobalPopulation(__instance);
            }
        }
    }
}