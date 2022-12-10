using flanne.UI;
using HarmonyLib;
using static PowerupBlacklist.Plugin;

namespace PowerupBlacklist
{
    internal class PowerupTreeUIPatch
    {
        [HarmonyPatch(typeof(PowerupTreeUI), "Refresh")]
        [HarmonyPostfix]
        static void RefreshPostfix()
        {
            BlacklistUI.updateUI();
        }
    }
}