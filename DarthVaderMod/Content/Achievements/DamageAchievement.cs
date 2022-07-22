using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;
using DarthVaderMod.Modules.Survivors;

namespace DarthVaderMod.Modules.Achievements
{
    [RegisterAchievement(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_ACHIEVEMENT_ID",
           DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DAMAGE_UNLOCKABLE_ID", null, null)]
    internal class DamageAchievement : BaseAchievement
    {
        private DarthVaderController darthCon;

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(Modules.Survivors.DarthVader.instance.fullBodyName);
        }

        public override void OnInstall()
        {
            base.OnInstall();
            On.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            On.RoR2.CharacterBody.RecalculateStats -= this.CharacterBody_RecalculateStats;
        }
        public void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
            {
                if (!darthCon)
                {
                    darthCon = self.gameObject.GetComponent<DarthVaderController>();
                }

                if (darthCon.maxDamage >= 100000)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }
    }
}