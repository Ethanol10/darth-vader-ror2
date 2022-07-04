using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class RageMode : BaseSkillState
    {
        private GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/ImpBossBlink");
        public override void OnEnter()
        {            
            base.OnEnter();

            characterBody.AddTimedBuffAuthority(Modules.Buffs.RageBuff.buffIndex, Modules.StaticValues.ragebuffDuration);
            characterBody.healthComponent.Heal(characterBody.healthComponent.fullCombinedHealth, new ProcChainMask(), true);

            EffectManager.SpawnEffect(effectPrefab, new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = 1f,
            }, true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if(base.fixedAge > 0.1f && base.isAuthority)
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
            return InterruptPriority.PrioritySkill;
        }
    }
}