using RoR2;
using System;
using UnityEngine;

namespace DarthVaderMod.Modules.Achievements
{
    internal class AnyDifficultyAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY";
        //the name of the sprite in your bundle
        public override string AchievementSpriteName => "blueskin";
        //the token of your character's unlock achievement if you have one
        public override string PrerequisiteUnlockableIdentifier => DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_REWARD_ID";

        public override string RequiredCharacterBody => "DarthVaderBody";
        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 0;

    }
}