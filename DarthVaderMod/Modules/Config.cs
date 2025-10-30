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
        public static ConfigEntry<bool> enableMusic;
        public static ConfigEntry<bool> enableBreathing;
        public static ConfigEntry<float> primaryCoefficient;
        public static ConfigEntry<float> secondaryCoefficientPull;
        public static ConfigEntry<float> secondaryCoefficientPush;
        public static ConfigEntry<float> secondaryForceOnWeightMultiplier;
        public static ConfigEntry<float> secondaryForceReductionForBosses;
        public static ConfigEntry<float> secondaryCooldown;
        public static ConfigEntry<float> utilityMultiplier;
        public static ConfigEntry<float> utilityCooldown;
        public static ConfigEntry<float> specialDuration;
        public static ConfigEntry<float> specialCooldown;
        public static ConfigEntry<float> specialMultiplier;
        public static ConfigEntry<float> passiveEnergyRecovery;
        public static ConfigEntry<float> energyGainedOnHit;
        public static ConfigEntry<float> cooldownReducedOnHit;
        public static ConfigEntry<float> baseDamage;
        public static ConfigEntry<float> damageGainedPerLevel;

        public static ConfigEntry<bool> armorBonusPassiveActive;
        public static ConfigEntry<bool> speedCapBaseActive;
        public static ConfigEntry<bool> speedCapRageActive;
        public static ConfigEntry<float> maxSpeedNormal;
        public static ConfigEntry<float> maxSpeedRage;
            
        public static void ReadConfig()
        {
            #region General - 01
            baseDamage = DarthVaderPlugin.instance.Config.Bind<float>("01 - General", "02 - Base Damage", 10f, "Sets the base damage at Level 1. Requires a restart to apply.");
            damageGainedPerLevel = DarthVaderPlugin.instance.Config.Bind<float>("01 - General", "03 - Damage growth per level", 2.4f, "Sets the damage gained per level. Requires a restart to apply.");
            enableMusic = DarthVaderPlugin.instance.Config.Bind<bool>("01 - General", "04 - Music is enabled", true, "Enables music. Setting to false mutes music.");
            enableBreathing = DarthVaderPlugin.instance.Config.Bind<bool>("01 - General", "05 - Darth Breathing enabled", true, "Enables periodic breathing SFX. Set to False to disable.");
            #endregion
            #region Primary - 02
            primaryCoefficient = DarthVaderPlugin.instance.Config.Bind<float>("02 - Primary", "01 - Lightsaber Coefficient", Modules.StaticValues.swordDamageCoefficient, "Sets the coefficient for lightsabre, 1.0 = 100%. Can change in runtime, but requires a restart to show in the UI.");
            #endregion
            #region Secondary - 03
            secondaryCoefficientPull = DarthVaderPlugin.instance.Config.Bind<float>("03 - Secondary", "01 - Force Push Coefficient", Modules.StaticValues.forcepushDamageCoefficient, "Sets the coefficient for Force Push, 1.0 = 100%. Can change in runtime, but requires a restart to show in the UI.");
            secondaryCoefficientPush = DarthVaderPlugin.instance.Config.Bind<float>("03 - Secondary", "02 - Force Pull Coefficient", Modules.StaticValues.forcepullDamageCoefficient, "Sets the coefficient for Force Pull, 1.0 = 100%. Can change in runtime, but requires a restart to show in the UI.");
            secondaryForceOnWeightMultiplier = DarthVaderPlugin.instance.Config.Bind<float>("03 - Secondary", "03 - Mulitplier on Force", 1f, "Multiplies the force applied on an enemy affected by pull/push by the set value.");
            secondaryForceReductionForBosses = DarthVaderPlugin.instance.Config.Bind<float>("03 - Secondary", "04 - Reduction on boss enemies", 5f, "Reduces the amount of force applied for boss enemies affected by Push/Pull");
            secondaryCooldown = DarthVaderPlugin.instance.Config.Bind<float>("03 - Secondary", "05 - Force Push/Pull Cooldown", 5f, "Sets the cooldown for Force Pull/Push. Requires a restart to apply.");
            #endregion

            #region Misc - 04
            armorBonusPassiveActive = DarthVaderPlugin.instance.Config.Bind<bool>("04 - Misc", "01 - Armor Bonus Passive", true, "Determines if Armor should scale based on movespeed.");
            speedCapBaseActive = DarthVaderPlugin.instance.Config.Bind<bool>("04 - Misc", "02 - Move Speed Limiter", true, "Determines if the move speed should be limited in Normal Mode, outside of Rage. If unticked, speed will be uncapped.");
            maxSpeedNormal = DarthVaderPlugin.instance.Config.Bind<float>("04 - Misc", "03 - Max Speed in Normal Mode", 0f, "Determines the maximum speed Darth Vader can move at. Setting it to 0 uncaps the speed.");
            speedCapRageActive = DarthVaderPlugin.instance.Config.Bind<bool>("04 - Misc", "04 - Rage: Move Speed Limiter", true, "Determines if the move speed should be limited in Rage Mode. If unticked, speed will be uncapped.");
            maxSpeedRage = DarthVaderPlugin.instance.Config.Bind<float>("04 - Misc", "05 - Max Speed in Rage Mode", 0f, "Determines the maximum speed Darth Vader can move at in Rage mode. Setting it to 0 uncaps the speed.");
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
            ModSettingsManager.AddOption(new CheckBoxOption(
                enableMusic));
            ModSettingsManager.AddOption(new CheckBoxOption(
                enableBreathing));
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

            #region Misc - 04
            ModSettingsManager.AddOption(new CheckBoxOption(armorBonusPassiveActive));
            ModSettingsManager.AddOption(new CheckBoxOption(speedCapBaseActive));
            ModSettingsManager.AddOption(new StepSliderOption(
                maxSpeedNormal,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1000f,
                    increment = 0.5f
                }
            ));
            ModSettingsManager.AddOption(new CheckBoxOption(speedCapRageActive));
            ModSettingsManager.AddOption(new StepSliderOption(
                maxSpeedRage,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1000f,
                    increment = 0.5f
                }
            ));

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