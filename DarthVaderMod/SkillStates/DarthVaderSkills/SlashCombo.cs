using DarthVaderMod.Modules;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {
        public DarthVaderController DarthVadercon;
        //public DarthVaderMasterController DarthVadermastercon;
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
            this.hitStopDuration = 0.2f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 10f;

            this.swingSoundString = "DarthLightSaberSwing";
            this.hitSoundString = "";
            this.muzzleString = ChooseAnimationString();
            this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;

            this.impactSound = Modules.Assets.swordHitSoundEvent.index;

            base.OnEnter();
            DarthVadercon = gameObject.GetComponent<DarthVaderController>();
        }

        private string ChooseAnimationString() 
        {
            string returnVal = "SwingLeft";
            switch (this.swingIndex) 
            {
                case 0:
                    returnVal = "SwingLeft";
                    break;
                case 1:
                    returnVal = "SwingRight";
                    break;
                case 2:
                    returnVal = "SwingCenter";
                    break;
                case 3:
                    returnVal = "SwingRight";
                    break;
            }

            return returnVal;
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
            if (DarthVadercon) 
            {
                DarthVadercon.currentForceEnergy += StaticValues.meleeOnHitForceEnergyGain;
                DarthVadercon.TriggerGlow(0.1f, 0.3f, Color.white);
            }
        }

        protected override void SetNextState()
        {
            int index = this.swingIndex;
            index += 1;
            if (index > 3) 
            {
                index = 0;
            }

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