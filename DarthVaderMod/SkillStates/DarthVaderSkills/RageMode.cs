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
                scale = 2f,
            }, true);
        }


        public override void OnExit()
        {
            base.OnExit();
        }
    }
}