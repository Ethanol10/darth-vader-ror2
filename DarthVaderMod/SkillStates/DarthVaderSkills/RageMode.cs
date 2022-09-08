using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using R2API.Networking;
using RoR2;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class RageMode : BaseSkillState
    {
        public float timer;

        public EnergySystem energySystem;
        public DarthVaderController DarthVadercon;
        public DarthVaderPassive passiveSkillSlot;
        private GameObject blasteffectPrefab = Resources.Load<GameObject>("prefabs/effects/ImpBossBlink");
        public GameObject effectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");

        public bool isEnergy;

        public override void OnEnter()
        {            
            base.OnEnter();

            DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();
            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();
            energySystem = characterBody.gameObject.GetComponent<EnergySystem>();


            if (passiveSkillSlot.isEnergyPassive())
            {
                if (energySystem && DarthVadercon)
                {
                    isEnergy = true;

                    if (energySystem.currentForceEnergy > (energySystem.maxForceEnergy * 0.98f))
                    {
                        characterBody.skillLocator.special.AddOneStock();
                        energySystem.TriggerGlow(0.1f, 0.3f, Color.black);
                        characterBody.ApplyBuff(Modules.Buffs.RageBuff.buffIndex, 1, -1);
                        characterBody.healthComponent.Heal(characterBody.healthComponent.fullCombinedHealth, new ProcChainMask(), true);

                        RageEffectController ragecontroller = characterBody.gameObject.GetComponent<RageEffectController>();
                        if (!ragecontroller)
                        {
                            ragecontroller = characterBody.gameObject.AddComponent<RageEffectController>();
                            ragecontroller.charbody = characterBody;
                        }

                        if (base.isAuthority) 
                        {
                            DarthVadercon.PlayRageLoop();
                        }

                        EffectManager.SpawnEffect(blasteffectPrefab, new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = 1f,
                        }, true);

                    }
                    else
                    {
                        characterBody.skillLocator.special.AddOneStock();
                        energySystem.TriggerGlow(0.1f, 0.3f, Color.blue);
                        if (base.isAuthority)
                        {
                            this.outer.SetNextStateToMain();
                            return;
                        }
                    }
                    

                }
            }
            else
            {

                isEnergy = false;

                characterBody.AddTimedBuffAuthority(Modules.Buffs.RageBuff.buffIndex, Modules.StaticValues.ragebuffDuration);
                characterBody.healthComponent.Heal(characterBody.healthComponent.fullCombinedHealth, new ProcChainMask(), true);

                RageEffectController ragecontroller = characterBody.gameObject.GetComponent<RageEffectController>();
                if (!ragecontroller)
                {
                    ragecontroller = characterBody.gameObject.AddComponent<RageEffectController>();
                    ragecontroller.charbody = characterBody;
                }

                if (base.isAuthority)
                {
                    AkSoundEngine.PostEvent("DarthRage", this.gameObject);
                }

                EffectManager.SpawnEffect(blasteffectPrefab, new EffectData
                {
                    origin = base.characterBody.footPosition,
                    scale = 1f,
                }, true);

            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (timer > 0.1f)
            {

                if (isEnergy)
                {
                    if (energySystem)
                    {
                        energySystem.TriggerGlow(0.05f, 0.05f, new Color(UnityEngine.Random.Range(0f, 1.0f), UnityEngine.Random.Range(0f, 1.0f), UnityEngine.Random.Range(0f, 1.0f), 1f));
                    }
                }

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
            if (base.fixedAge > 0.5f && base.isAuthority)
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