using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class RageMode : BaseSkillState
    {
        public float timer;

        public DarthVaderController DarthVadercon;
        public DarthVaderPassive passiveSkillSlot;
        private GameObject blasteffectPrefab = Resources.Load<GameObject>("prefabs/effects/ImpBossBlink");
        public GameObject effectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");
        private bool enoughEnergy;

        public override void OnEnter()
        {            
            base.OnEnter();

            characterBody.AddTimedBuffAuthority(Modules.Buffs.RageBuff.buffIndex, Modules.StaticValues.ragebuffDuration);
            characterBody.healthComponent.Heal(characterBody.healthComponent.fullCombinedHealth, new ProcChainMask(), true);

            RageEffectController ragecontroller = characterBody.gameObject.GetComponent<RageEffectController>();
            if (!ragecontroller)
            {
                ragecontroller = characterBody.gameObject.AddComponent<RageEffectController>();
                ragecontroller.charbody = characterBody;
            }

            AkSoundEngine.PostEvent("DarthRage", this.gameObject);

            EffectManager.SpawnEffect(blasteffectPrefab, new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = 1f,
            }, true);


            if (passiveSkillSlot.isEnergyPassive())
            {
                if (DarthVadercon)
                {
                    if (DarthVadercon.currentForceEnergy == DarthVadercon.maxForceEnergy)
                    {
                        DarthVadercon.TriggerGlow(0.1f, 0.3f, Color.black);
                        enoughEnergy = true;
                    }
                    else
                    {
                        DarthVadercon.TriggerGlow(0.1f, 0.3f, Color.blue);
                        enoughEnergy = false;
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (passiveSkillSlot.isEnergyPassive())
            {
                if (enoughEnergy)
                {
                    if (timer > 0.1f)
                    {
                        float num = 10f;
                        Quaternion rotation = Util.QuaternionSafeLookRotation(Vector3.up);
                        float num2 = 0.01f;
                        rotation.x += UnityEngine.Random.Range(-num2, num2) * num;
                        rotation.y += UnityEngine.Random.Range(-num2, num2) * num;

                        timer = 0f;
                        EffectManager.SpawnEffect(effectPrefab, new EffectData
                        {
                            origin = base.characterBody.corePosition,
                            scale = 1f,
                            rotation = rotation
                        }, true);
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                    }

                }
            }
            else if (timer > 0.1f)
            {
                float num = 10f;
                Quaternion rotation = Util.QuaternionSafeLookRotation(Vector3.up);
                float num2 = 0.01f;
                rotation.x += UnityEngine.Random.Range(-num2, num2) * num;
                rotation.y += UnityEngine.Random.Range(-num2, num2) * num;

                timer = 0f;
                EffectManager.SpawnEffect(effectPrefab, new EffectData
                {
                    origin = base.characterBody.corePosition,
                    scale = 1f,
                    rotation = rotation
                }, true);
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
            
            if(base.fixedAge > 0.5f && base.isAuthority)
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