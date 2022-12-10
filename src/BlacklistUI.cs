using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using flanne.UI;
using System.Linq;
using System.Collections.Generic;
using static PowerupBlacklist.Plugin;

namespace PowerupBlacklist
{
    internal class BlacklistUI : MonoBehaviour, IPointerClickHandler
    {
        static List<BlacklistUI> objects = new List<BlacklistUI>();
        RectTransform rectTransform;
        GameObject imgGameObject;
        Image blacklistImage;
        PowerupIcon powerupIcon;
        string powerupName;
        string groupName;
        bool blacklisted = false;
        int width = 512;
        int height = 512;

        void Start()
        {            
            imgGameObject = new GameObject("BlacklistImage");
            imgGameObject.transform.SetParent(gameObject.transform);

            rectTransform = imgGameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition= new Vector2(0f, 0f);
            rectTransform.sizeDelta = new Vector2(0.8f, 0.8f);

            blacklistImage = imgGameObject.AddComponent<Image>();
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.LoadImage(blacklistImageBytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0,0, width, height), new Vector2(1f, 1f));
            blacklistImage.sprite = sprite;

            updateBlacklisted();
            BlacklistUI.objects.Add(this);
        }

        internal void updateBlacklisted() {
            powerupIcon = gameObject.GetComponent<PowerupIcon>();
            if(powerupIcon.data.powerupTreeUIData is not null) {
                groupName = powerupIcon.data.powerupTreeUIData.name.Replace("_", string.Empty);
            }
            powerupName = powerupIcon.data.name;
            blacklistImage.enabled = blacklisted;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            blacklisted = !blacklisted;
            updateBlacklisted();
            writeConfig();
        }

        internal static void updateUI()
        {
            foreach (BlacklistUI blacklistUI in BlacklistUI.objects)
            {
                blacklistUI.setBlacklisted();
                blacklistUI.updateBlacklisted();
            }
        }

        internal void setBlacklisted()
        {
            powerupIcon = gameObject.GetComponent<PowerupIcon>();
            if(powerupIcon.data.powerupTreeUIData is not null) {
                groupName = powerupIcon.data.powerupTreeUIData.name.Replace("_", string.Empty);
            }
            powerupName = powerupIcon.data.name;
            List<string> blacklist = ConfigBlacklist.Value.Split(',').ToList();
            blacklisted = blacklist.Contains(powerupName) || (groupName is not null && blacklist.Contains(groupName));
        }

        internal void writeConfig()
        {
            List<string> blacklist = ConfigBlacklist.Value.Split(',').ToList();
            if(blacklisted) {
                if(!blacklist.Contains(powerupName) && (groupName is not null && !blacklist.Contains(groupName))) {
                    blacklist.Add(powerupName);
                }
            } else {
                if(blacklist.Contains(powerupName)) {
                    blacklist.Remove(powerupName);
                }
                if(groupName is not null && blacklist.Contains(groupName)) {
                    blacklist.Remove(groupName);
                }
            }

            string newConfig = string.Join(",", blacklist);
            if(ConfigBlacklist.Value != newConfig) {
                ConfigBlacklist.Value = newConfig;
            }
        }

        void OnDestroy()
        {
            BlacklistUI.objects.Remove(this);
        }
    }
}