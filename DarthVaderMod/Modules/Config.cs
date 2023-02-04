using BepInEx.Configuration;
using UnityEngine;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;

namespace DarthVaderMod.Modules
{
    public static class Config
    {
        //Config Update
        /*
            Primary Coefficient / Cooldown (Cooldown system)
            Secondary Coefficient / Cooldown (Cooldown system)
            Utility multiplier / Cooldown (Cooldown system)
            Special Duration / Cooldown (Cooldown system)
            Energy Recovery (Energy System)
            Passive energy recovery (Energy System)
            Energy gained on hit (Energy System)
            base damage
            Rage multiplier

         */
        public static ConfigEntry<bool> limitMovespeed;
        public static ConfigEntry<float> primaryCoefficient;
        public static ConfigEntry<float> secondaryCoefficient;
        public static ConfigEntry<float> secondaryCooldown;
        public static ConfigEntry<float> utilityMultiplier;
        public static ConfigEntry<float> utilityCooldown;
        public static ConfigEntry<float> specialDuration;
        public static ConfigEntry<float> specialCooldown;
        public static ConfigEntry<float> specialMultiplier;
        public static ConfigEntry<float> passiveEnergyRecovery;
        public static ConfigEntry<float> energyGainedOnHit;
        public static ConfigEntry<float> baseDamage;
        public static ConfigEntry<float> damageGainedPerLevel;

        public static void ReadConfig()
        {
            #region General - 01
            limitMovespeed = DarthVaderPlugin.instance.Config.Bind<bool>("01 - General", "01 - Limit Movespeed", true, "Limits Movespeed but grants armor. Setting to false removes armor bonus while outside of rage.");
            baseDamage = DarthVaderPlugin.instance.Config.Bind<float>("01 - General", "02 - Base Damage", 10f, "Sets the base damage at Level 1. Requires a restart to apply.");
            damageGainedPerLevel = DarthVaderPlugin.instance.Config.Bind<float>("01 - General", "02 - Damage growth per level", 2.4f, "Sets the damage gained per level. Requires a restart to apply.");
            #endregion
            #region Primary - 02
            primaryCoefficient = DarthVaderPlugin.instance.Config.Bind<float>("02 - Primary", "02 - Primary Coefficient", Modules.StaticValues.swordDamageCoefficient, "Sets the coefficient for primary, 1.0 = 100%. Requires a restart to show in the UI.");
            #endregion
            #region Secondary - 03
            secondaryCoefficient = DarthVaderPlugin.instance.Config.Bind<float>("02 - Primary", "02 - Primary Coefficient", Modules.StaticValues.swordDamageCoefficient, "Sets the coefficient for primary, 1.0 = 100%. Requires a restart to show in the UI.");

            #endregion
            #region Utility - 04
            #endregion
            #region Special - 05
            #endregion
            #region Passives - 06
            #endregion

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

            #region General - 01
            ModSettingsManager.AddOption(new CheckBoxOption(
                limitMovespeed));
            ModSettingsManager.AddOption(new StepSliderOption(
                baseDamage,
                new StepSliderConfig 
                {
                    min = 1f,
                    max = 100f,
                    increment = 0.5f
                }
                ));
            ModSettingsManager.AddOption(new StepSliderOption(
                damageGainedPerLevel,
                new StepSliderConfig
                {
                    min = 1f,
                    max = 100f,
                    increment = 0.5f
                }
                ));
            #endregion

            #region Primary - 02
            ModSettingsManager.AddOption(new StepSliderOption(
                primaryCoefficient,
                new StepSliderConfig
                {
                    min = 1f,
                    max = 100f,
                    increment = 0.1f
                }
                ));
            #endregion

            #region Secondary - 03
            #endregion
            #region Utility - 04
            #endregion
            #region Special - 05
            #endregion
            #region Passives - 06
            #endregion

        }
    }
}