using EntityStates;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class DarthVaderPrimary : BaseSkillState
    {
        public DarthVaderController DarthVadercon;
        public DarthVaderMasterController DarthVadermastercon;
        public HurtBox Target;
        public override void OnEnter()
        {
            DarthVadercon = base.GetComponent<DarthVaderController>();
            DarthVadermastercon = characterBody.master.gameObject.GetComponent<DarthVaderMasterController>();
            if (DarthVadercon && base.isAuthority)
            {
                Target = DarthVadercon.GetTrackingTarget();
            }

            if(Target)
            {
                float num = 10f;
                if (!base.isGrounded)
                {
                    num = 7f;
                }
                float num2 = Vector3.Distance(base.transform.position, Target.transform.position);
                if (num2 >= num)
                {
                    this.outer.SetNextState(new DashAttack
                    {

                    });
                }
                else
                {
                    this.outer.SetNextState(new SlashCombo
                    {

                    });

                }
            }
            else if (!Target)
            {
                this.outer.SetNextState(new SlashCombo
                {

                });
            }

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}