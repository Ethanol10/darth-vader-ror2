using BepInEx.Configuration;
using UnityEngine;

namespace DarthVaderMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> basedamage;
        public static void ReadConfig()
        {
            basedamage = DarthVaderPlugin.instance.Config.Bind<float>("General", "base damage multiplier", 10f, "Adjusts base damage.");


        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return DarthVaderPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}