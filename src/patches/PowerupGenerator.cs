using flanne;
using HarmonyLib;
using UnityEngine;
using static PowerupBlacklist.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace PowerupBlacklist
{
    internal class PowerupGeneratorPatch
    {
        [HarmonyPatch(typeof(PowerupGenerator), "SetCharacterPowerupPool")] //runs after InitPowerupPool
        [HarmonyPostfix]
        static void SetCharacterPowerupPoolPostfix(ref List<PowerupPoolItem> ___powerupPool, ref List<PowerupPoolItem> ___devilPool, ref List<PowerupPoolItem> ___characterPool, PowerupPoolProfile characterExclusiveProfile)
        {
            List<string> blacklist = ConfigBlacklist.Value.Split(',').ToList();
            List<PowerupPoolItem> powerupPoolCopy = new List<PowerupPoolItem>(___powerupPool);
            List<PowerupPoolItem> devilPoolCopy = new List<PowerupPoolItem>(___devilPool);
            List<PowerupPoolItem> characterPoolCopy = new List<PowerupPoolItem>(___characterPool);

            foreach (PowerupPoolItem item in ___powerupPool)
            {
                if(blacklist.Contains(item.powerup.name) || (item.powerup.powerupTreeUIData is not null && blacklist.Contains(item.powerup.powerupTreeUIData.name.Replace("_", string.Empty)))){
                    powerupPoolCopy.Remove(item);
                }
            }
            ___powerupPool.Clear();
            ___powerupPool.AddRange(powerupPoolCopy);

            foreach (PowerupPoolItem item in ___devilPool)
            {
                if(blacklist.Contains(item.powerup.name) || (item.powerup.powerupTreeUIData is not null && blacklist.Contains(item.powerup.powerupTreeUIData.name.Replace("_", string.Empty)))){
                    devilPoolCopy.Remove(item);
                }
            }
            ___devilPool.Clear();
            ___devilPool.AddRange(devilPoolCopy);

            foreach (PowerupPoolItem item in ___characterPool)
            {
                if(blacklist.Contains(item.powerup.name) || (item.powerup.powerupTreeUIData is not null && blacklist.Contains(item.powerup.powerupTreeUIData.name.Replace("_", string.Empty)))){
                    characterPoolCopy.Remove(item);
                }
            }
            ___characterPool.Clear();
            ___characterPool.AddRange(characterPoolCopy);
            
            if(blacklist.Contains("ListAll")){
                List<Powerup> powerups = Resources.FindObjectsOfTypeAll<Powerup>().ToList();
                List<List<Powerup>> groupPowerups = powerups.Where(p => p.powerupTreeUIData is not null).ToList().GroupBy(p => p.powerupTreeUIData.name).Select(group => group.ToList()).ToList();
                List<Powerup> singlePowerups = powerups.Where(p => p.powerupTreeUIData is null).ToList();
                Log.LogInfo("Single Powerups:");
                foreach (Powerup powerup in singlePowerups)
                {
                    Log.LogInfo("   " + powerup.name);
                }
                Log.LogInfo("Grouped Powerups:");
                foreach (List<Powerup> list in groupPowerups)
                {
                    Log.LogInfo("   " + list[0].powerupTreeUIData.name.Replace("_", string.Empty) + ":");
                    foreach (Powerup powerup in list)
                    {
                        Log.LogInfo("       " + powerup.name);
                    }                   
                }
            }
        }
    }
}