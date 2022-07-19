using R2API;
using System;

namespace DarthVaderMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region DarthVader
            string prefix = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_";

            string desc = "DarthVader is a juggernaut with high survivability at the cost of mobility.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Attackspeed and Movespeed don't increase." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Attackspeed bonuses are converted to Damage and Movespeed bonuses are converted to Armor" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..For one more second, he felt the light.";
            string outroFailure = "..He left full of rage and hate, ready for more training.";

            LanguageAPI.Add(prefix + "NAME", "Darth Vader");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One"); 
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Red Lightsaber");
            LanguageAPI.Add(prefix + "BLUESABER_SKIN_NAME", "Blue Lightsaber");
            LanguageAPI.Add(prefix + "YELLOWSABER_SKIN_NAME", "Yellow Lightsaber");
            LanguageAPI.Add(prefix + "DARKSABER_SKIN_NAME", "Darksaber");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "The Chosen One");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Attackspeed bonuses are converted to Damage. Movespeed bonuses are converted to Armor.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Lightsaber");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * StaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_FORCE_NAME", "Force Push/Pull");
            LanguageAPI.Add(prefix + "SECONDARY_FORCE_DESCRIPTION", Helpers.agilePrefix + $"Tap to push enemies for <style=cIsDamage>{100f * StaticValues.forcepushDamageCoefficient}% damage</style>. " +
                $"Hold to pull enemies for <style=cIsDamage>{100f * StaticValues.forcepullDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DEFLECT_NAME", "Deflect");
            LanguageAPI.Add(prefix + "UTILITY_DEFLECT_DESCRIPTION", $"<style=cIsUtility>Deflect all attacks for {StaticValues.deflectbuffDuration} seconds</style> " +
                $"for <style=cIsDamage>2x the damage, multiplied by luck</style>. " +
                $"Movespeed is halved and you're unable to attack with your lightsaber while deflecting.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_RAGE_NAME", "Rage Unleashed");
            LanguageAPI.Add(prefix + "SPECIAL_RAGE_DESCRIPTION", $"Unleashes your rage, <style=cIsHealing>fully healing yourself</style> and " +
                $"<style=cIsUtility>removing your attackspeed and movespeed limiters</style>. " +
                $"<style=cIsDamage>Double your attackspeed, movespeed and armor as well as remove all cooldowns for {StaticValues.ragebuffDuration} seconds</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "DarthVader: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As DarthVader, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "DarthVader: Mastery");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "DAMAGE_ACHIEVEMENT_ID_NAME", "Now I am the master!");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "DAMAGE_ACHIEVEMENT_ID_DESCRIPTION", "Deal 100,000 damage in one hit.");
            #endregion
            #endregion
        }
    }
}