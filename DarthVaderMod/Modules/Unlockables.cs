
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

        public static void AddUnlockables()
        {
            darthDamageUnlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            darthDamageUnlockable.nameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DAMAGE_UNLOCKABLE_ID";
            darthDamageUnlockable.cachedName = DarthVaderPlugin.DEVELOPER_PREFIX + "_DAMAGE_UNLOCKABLE_ID";
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


            ContentPacks.unlockableDefs.Add(darthDamageUnlockable);

        }
    }
}