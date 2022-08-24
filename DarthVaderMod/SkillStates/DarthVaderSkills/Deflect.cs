using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class Deflect : BaseSkillState
    {

        //Controller and energy system
        public DarthVaderController DarthVadercon;
        public DarthVaderPassive passiveSkillSlot;
        public EnergySystem energySystem;

        public override void OnEnter()
        {            
            base.OnEnter();

            DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();
            energySystem = characterBody.gameObject.GetComponent<EnergySystem>();
            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();
            if (passiveSkillSlot.isEnergyPassive())
            {
                if(energySystem) energySystem.ifEnergyRegenAllowed = false;
                characterBody.skillLocator.utility.AddOneStock();
                characterBody.AddBuff(Modules.Buffs.DeflectBuff.buffIndex);


                PlayAnimation("RightArm, Override", "Deflect", "Attack.playbackRate", 10000f);
                //this.outer.SetNextState(new DeflectCancel());
            }
            else
            {
                characterBody.AddTimedBuffAuthority(Modules.Buffs.DeflectBuff.buffIndex, Modules.StaticValues.deflectbuffDuration);
                PlayAnimation("RightArm, Override", "Deflect", "Attack.playbackRate", 6f);
            }

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            PlayCrossfade("RightArm, Override", "Deflect", "Attack.playbackRate", 1f, 0.1f);

            if (passiveSkillSlot.isEnergyPassive())
            {
                if (base.IsKeyDownAuthority())
                {
                    PlayCrossfade("RightArm, Override", "Deflect", "Attack.playbackRate", 1f, 0.1f);

                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;

                }
            }
            else if (base.fixedAge > Modules.StaticValues.deflectbuffDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            

        }


        public override void OnExit()
        {
            base.OnExit();
            if(energySystem) energySystem.ifEnergyRegenAllowed = true;
            characterBody.RemoveBuff(Modules.Buffs.DeflectBuff.buffIndex);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}