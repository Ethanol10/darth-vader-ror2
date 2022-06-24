using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using DarthVaderMod.Modules.Survivors;

namespace DarthVaderMod.SkillStates
{
    public class Analyze : BaseSkillState
    {
        public DarthVaderController DarthVadercon;
        public DarthVaderMasterController DarthVadermastercon;
        public HurtBox Target;

        public float duration = 0.1f;
        public override void OnEnter()
        {
            base.OnEnter();
            DarthVadercon = base.GetComponent<DarthVaderController>();
            DarthVadermastercon = characterBody.master.gameObject.GetComponent<DarthVaderMasterController>();
            if (DarthVadercon && base.isAuthority)
            {
                Target = DarthVadercon.GetTrackingTarget();
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge > duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (!Target)
            {
                return;
                base.skillLocator.utility.AddOneStock();

            }
            if (Target)
            {
                Target.healthComponent.body.AddTimedBuffAuthority(Modules.Buffs.CritDebuff.buffIndex, Modules.StaticValues.analyzedebuffDuration);
            }

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}