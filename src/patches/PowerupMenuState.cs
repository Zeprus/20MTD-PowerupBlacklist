using flanne;
using flanne.UI;
using flanne.Core;
using HarmonyLib;
using UnityEngine;
using static PowerupBlacklist.Plugin;

namespace PowerupBlacklist
{
    internal class PowerupMenuStatePatch
    {
        static GameController gameController;

        [HarmonyPatch(typeof(PowerupMenuState), "Enter")]
        [HarmonyPostfix]
        static void EnterPostfix()
        {
            if(gameController is null) {
                gameController = Object.FindObjectOfType<GameController>();
            }
            PowerupMenu powerupMenu = gameController.powerupMenu;
            GameObject powerupMenuGO = powerupMenu.gameObject;
            Transform path = powerupMenuGO.transform.Find("MenuMover").Find("DescriptionBox").Find("PowerupTree");
            path.Find("TreePowerup (0)").gameObject.AddComponent<BlacklistUI>();
            path.Find("TreePowerup (1)").gameObject.AddComponent<BlacklistUI>();
            path.Find("TreePowerup (2)").gameObject.AddComponent<BlacklistUI>();
            path.Find("TreePowerup (3)").gameObject.AddComponent<BlacklistUI>();
        }
    }
}