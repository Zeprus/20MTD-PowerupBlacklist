using System;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace PowerupBlacklist
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {        
        internal static ManualLogSource Log;
        public static ConfigEntry<string> ConfigBlacklist;
        internal static List<string> blacklist;
        private void Awake()
        {
            // Plugin startup logic
            Log = base.Logger;

            ConfigBlacklist = Config.Bind<string>("Powerups", "Blacklist", "ListAll",
                "List all Powerups or Powerup-Groups you want to blacklist comma seperated." + System.Environment.NewLine +
                "WARNING! BLACKLISTING TOO MANY POWERUPS MAY CAUSE INSTABILITY!" + System.Environment.NewLine + 
                "Example:" + System.Environment.NewLine +
                "Blacklist = ReloadTree,Frostbite,Shatter,HaloPiece1,HaloPiece2,HaloPiece3" + System.Environment.NewLine +
                "Set the value to \"ListAll\" to print a list of all powerups and their groups into the BepInEx console upon battle start.");
            blacklist = ConfigBlacklist.Value.Split(',').ToList();
            
            try
            {
                Harmony.CreateAndPatchAll(typeof(PowerupBlacklist.PowerupGeneratorPatch));
            }
            catch (Exception e)
            {
                Log.LogError(e.GetType() + " " + e.Message);
            }

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
