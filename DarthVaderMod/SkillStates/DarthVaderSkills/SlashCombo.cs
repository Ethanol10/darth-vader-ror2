using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {
        public DarthVaderController DarthVadercon;
        public DarthVaderMasterController DarthVadermastercon;
        public HurtBox Target;
        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;

            this.damageCoefficient = Modules.StaticValues.swordDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = new Vector3(0f, -300f, 0f);
            this.baseDuration = 1f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = EntityStates.Merc.Weapon.GroundLight2.comboFinisherhitPauseDuration;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 10f;

            this.swingSoundString = "DarthVaderSwordSwing";
            this.hitSoundString = "";
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;

            this.impactSound = Modules.Assets.swordHitSoundEvent.index;

            DarthVadercon = base.GetComponent<DarthVaderController>();
            DarthVadermastercon = characterBody.master.gameObject.GetComponent<DarthVaderMasterController>();
            if (DarthVadercon && base.isAuthority)
            {
                Target = DarthVadercon.GetTrackingTarget();
            }
            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void SetNextState()
        {
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = index
            });
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}