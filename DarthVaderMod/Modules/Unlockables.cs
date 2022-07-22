
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DarthVaderMod.Modules
{
    internal static class Unlockables
    {
        public static UnlockableDef darthDamageUnlockable;
        public static UnlockableDef darthAnyDifficultyUnlockable;
        public static UnlockableDef darthStageUnlockable;
        public static void AddUnlockables()
        {
            // Damage Unlockable
            darthDamageUnlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            darthDamageUnlockable.nameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_UNLOCKABLE_ID";
            darthDamageUnlockable.cachedName = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_UNLOCKABLE_ID";
            darthDamageUnlockable.getHowToUnlockString = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthDamageUnlockable.getUnlockedString = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthDamageUnlockable.sortScore = 200;
            darthDamageUnlockable.achievementIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("yellowskin");

            Modules.Content.AddUnlockableDef(darthDamageUnlockable);

            // AnyDifficulty Unlockable
            darthAnyDifficultyUnlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            darthAnyDifficultyUnlockable.nameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_UNLOCKABLE_ID";
            darthAnyDifficultyUnlockable.cachedName = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_UNLOCKABLE_ID";
            darthAnyDifficultyUnlockable.getHowToUnlockString = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthAnyDifficultyUnlockable.getUnlockedString = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthAnyDifficultyUnlockable.sortScore = 200;
            darthAnyDifficultyUnlockable.achievementIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("blueskin");

            Modules.Content.AddUnlockableDef(darthAnyDifficultyUnlockable);


            // Stage Unlockable
            darthStageUnlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            darthStageUnlockable.nameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_UNLOCKABLE_ID";
            darthStageUnlockable.cachedName = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_UNLOCKABLE_ID";
            darthStageUnlockable.getHowToUnlockString = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthStageUnlockable.getUnlockedString = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_ACHIEVEMENT_ID_NAME"),
                                Language.GetString("ACHIEVEMENT_" + DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_ACHIEVEMENT_ID_DESCRIPTION")
                            }));
            darthStageUnlockable.sortScore = 200;
            darthStageUnlockable.achievementIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("blueskin");

            Modules.Content.AddUnlockableDef(darthStageUnlockable);


        }
    }
}