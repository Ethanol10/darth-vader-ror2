using BepInEx.Configuration;
using UnityEngine;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;

namespace DarthVaderMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<bool> limitMovespeed;
        public static void ReadConfig()
        {
            limitMovespeed = DarthVaderPlugin.instance.Config.Bind<bool>("General", "Limit Movespeed", true, "Limits Movespeed but grants armor. Setting to false removes armor bonus while outside of rage.");


        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return DarthVaderPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }


        public static void SetupRiskOfOptions()
        {
            //Risk of Options intialization
            ModSettingsManager.AddOption(new CheckBoxOption(
                limitMovespeed));

        }
    }
}