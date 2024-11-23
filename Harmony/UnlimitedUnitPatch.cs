using HarmonyLib;
using Il2CppBrokenArrow.Client.Ecs.Utils;

namespace XDClient.Harmony;

[HarmonyPatch(typeof(UnitStatistic), nameof(UnitStatistic.AddUnit))]
public static class UnlimitedUnitPatch
{
    public static bool Prefix()
    {
        return false;
    }
}