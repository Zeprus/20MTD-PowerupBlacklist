using System;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Reflection;
using UnityEngine.Events;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

namespace PowerupBlacklist
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {        
        internal static ManualLogSource Log;
        public static ConfigEntry<string> ConfigBlacklist;
        internal static byte[] blacklistImageBytes = new byte[0];
        internal static UnityAction<Button> resetBlacklistAction;

        private void Awake()
        {
            Log = base.Logger;
            Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PowerupBlacklist.res.blacklist.png");
            MemoryStream memoryStream = new MemoryStream();
            imageStream.CopyTo(memoryStream);
            blacklistImageBytes = memoryStream.ToArray();
            imageStream.Close();
            memoryStream.Close();
            Config.SaveOnConfigSet = true;
            ConfigBlacklist = Config.Bind<string>("Powerups", "Blacklist", "ListAll",
                "List all Powerups or Powerup-Groups you want to blacklist comma seperated." + System.Environment.NewLine +
                "WARNING! BLACKLISTING TOO MANY POWERUPS MAY CAUSE INSTABILITY!" + System.Environment.NewLine + 
                "Example:" + System.Environment.NewLine +
                "Blacklist = ReloadTree,Frostbite,Shatter,HaloPiece1,HaloPiece2,HaloPiece3" + System.Environment.NewLine +
                "Set the value to \"ListAll\" to print a list of all powerups and their groups into the BepInEx console upon battle start.");

            try
            {
                Harmony.CreateAndPatchAll(typeof(PowerupBlacklist.PowerupGeneratorPatch));
                Harmony.CreateAndPatchAll(typeof(PowerupBlacklist.PowerupMenuStatePatch));
                Harmony.CreateAndPatchAll(typeof(PowerupBlacklist.PowerupTreeUIPatch));
                Harmony.CreateAndPatchAll(typeof(PowerupBlacklist.TitleMainMenuStatePatch));
            }
            catch (Exception e)
            {
                Log.LogError(e.GetType() + " " + e.Message);
            }

            resetBlacklistAction += resetBlacklist;

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal void resetBlacklist(Button button)
        {
            ConfigBlacklist.Value = "";
            IEnumerator coroutine = updateMenuText(button);
            StartCoroutine(coroutine);
        }

        IEnumerator updateMenuText(Button button)
        {
            TextMeshProUGUI tmp = button.gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
            tmp.text = "Success";
            button.interactable = false;
            yield return new WaitForSeconds(2f);
            tmp.text = "Reset Blacklist";
            button.interactable = true;
        }
    }
}
