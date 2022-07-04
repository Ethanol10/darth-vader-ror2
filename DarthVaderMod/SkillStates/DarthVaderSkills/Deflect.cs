using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class Deflect : BaseSkillState
    {
        public override void OnEnter()
        {            
            base.OnEnter();

            characterBody.AddTimedBuffAuthority(Modules.Buffs.DeflectBuff.buffIndex, Modules.StaticValues.deflectbuffDuration);
            PlayAnimation("RightArm, Override", "Deflect", "Attack.playbackRate", 6f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayCrossfade("RightArm, Override", "Deflect", "Attack.playbackRate", 1f, 0.1f);


            if (base.fixedAge > Modules.StaticValues.deflectbuffDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}