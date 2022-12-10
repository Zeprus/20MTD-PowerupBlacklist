using flanne.UI;
using flanne.TitleScreen;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static PowerupBlacklist.Plugin;

namespace PowerupBlacklist
{
    internal class TitleMainMenuStatePatch
    {
        [HarmonyPatch(typeof(TitleMainMenuState), "Enter")]
        [HarmonyPostfix]
        private static void EnterPostfix(ref TitleScreenController ___owner)
        {
            if(___owner.mainMenuPanel.gameObject.transform.Find("PowerupBlacklistReset") is null) {
                var button = Object.Instantiate<Button>(___owner.optionsButton, ___owner.optionsButton.transform.parent);
                button.gameObject.transform.SetSiblingIndex(button.transform.GetSiblingIndex() - 1);
                Object.Destroy(button.gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextLocalizerUI>());
                TextMeshProUGUI text = button.gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
                text.text = "Reset Blacklist";
                button.name = "PowerupBlacklistReset";
                button.onClick.AddListener(delegate {resetBlacklistAction.Invoke(button);});
            }
        }
    }
}