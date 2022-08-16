using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class DeflectCancel : BaseSkillState
    {

        //Controller and energy system
        public DarthVaderController DarthVadercon;
        public DarthVaderPassive passiveSkillSlot;

        public override void OnEnter()
        {            
            base.OnEnter();

            DarthVadercon = characterBody.GetComponent<DarthVaderController>();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayCrossfade("RightArm, Override", "Deflect", "Attack.playbackRate", 1f, 0.1f);

            if (base.IsKeyDownAuthority())
            {
                this.outer.SetNextStateToMain();
                return;
            }
            

        }


        public override void OnExit()
        {
            base.OnExit();

            DarthVadercon.ifEnergyRegenAllowed = true;
            characterBody.RemoveBuff(Modules.Buffs.DeflectBuff.buffIndex);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}