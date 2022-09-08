using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Networking;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
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

        public bool isEnergy;

        public override void OnEnter()
        {            
            base.OnEnter();

            DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();
            energySystem = characterBody.gameObject.GetComponent<EnergySystem>();
            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();


            characterBody.ApplyBuff(Modules.Buffs.DeflectBuff.buffIndex, 1, -1);
            if (passiveSkillSlot.isEnergyPassive())
            {
                isEnergy = true;
                if (energySystem) energySystem.ifEnergyRegenAllowed = false;
                characterBody.skillLocator.utility.AddOneStock();


                PlayAnimation("RightArm, Override", "Deflect", "Attack.playbackRate", 10000f);
                //this.outer.SetNextState(new DeflectCancel());
            }
            else
            {
                isEnergy = false;
                PlayAnimation("RightArm, Override", "Deflect", "Attack.playbackRate", 6f);
            }

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public override void Update()
        {
            base.Update();
            PlayCrossfade("RightArm, Override", "Deflect", "Attack.playbackRate", 1f, 0.01f);
            if (base.isAuthority)
            {
                if (isEnergy)
                {
                    if (base.IsKeyDownAuthority())
                    {

                    }
                    else if (!base.IsKeyDownAuthority())
                    {
                        this.outer.SetNextStateToMain();
                        return;

                    }
                }
                else
                {
                    if ((base.fixedAge > Modules.StaticValues.deflectbuffDuration && base.isAuthority))
                    {
                        this.outer.SetNextStateToMain();
                        return;

                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            

        }


        public override void OnExit()
        {
            base.OnExit();
            if(energySystem) energySystem.ifEnergyRegenAllowed = true;
            characterBody.SetBuffCount(Modules.Buffs.DeflectBuff.buffIndex, 0);

            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }


        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self)
            {
                if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
                {
                    bool flag = (damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic;
                    if (!flag && damageInfo.damage > 0f)
                    {
                        if (self.body.HasBuff(Modules.Buffs.DeflectBuff.buffIndex))
                        {
                            damageInfo.rejected = true;

                            DamageInfo damageInfo2 = new DamageInfo();

                            damageInfo2.damage = damageInfo.damage * 2f * (1f + self.body.master.luck);
                            damageInfo2.position = damageInfo.attacker.transform.position;
                            damageInfo2.force = Vector3.zero;
                            damageInfo2.damageColorIndex = DamageColorIndex.Default;
                            damageInfo2.crit = Util.CheckRoll(self.body.crit, self.body.master);
                            damageInfo2.attacker = self.gameObject;
                            damageInfo2.inflictor = null;
                            damageInfo2.damageType = DamageType.Generic;
                            damageInfo2.procCoefficient = 1f;
                            damageInfo2.procChainMask = default(ProcChainMask);

                            passiveSkillSlot = self.gameObject.GetComponent<DarthVaderPassive>();
                            energySystem = self.body.gameObject.GetComponent<EnergySystem>();

                            Vector3 enemyPos = damageInfo.attacker.transform.position;
                            Vector3 distance = (enemyPos - self.body.transform.position);

                            //Energy passive
                            if (passiveSkillSlot.isEnergyPassive() && self.body.hasEffectiveAuthority)
                            {
                                new DeflectClientHandlerNetworkRequest(damageInfo.attacker.gameObject.GetComponent<CharacterBody>().masterObjectId,
                                        self.body.masterObjectId, damageInfo.damage).Send(NetworkDestination.Clients);
                            }

                            //Cooldown passive
                            else if (!passiveSkillSlot.isEnergyPassive())
                            {
                                AkSoundEngine.PostEvent("DarthDeflect", self.body.gameObject);

                                damageInfo.rejected = true;

                                if (damageInfo.attacker.gameObject.GetComponent<CharacterBody>().baseNameToken
                                    != DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME" && damageInfo.attacker != null)
                                {
                                    damageInfo.attacker.GetComponent<CharacterBody>().healthComponent.TakeDamage(damageInfo2);
                                }

                                if (distance.magnitude >= 3)
                                {
                                    EffectManager.SpawnEffect(Modules.Assets.blasterShotEffect, new EffectData
                                    {
                                        origin = self.body.transform.position,
                                        scale = 1f,
                                        rotation = Quaternion.LookRotation(distance)

                                    }, true);

                                }
                                else if (distance.magnitude < 3)
                                {
                                    EffectManager.SpawnEffect(Modules.Assets.swordHitImpactEffect, new EffectData
                                    {
                                        origin = enemyPos,
                                        scale = 1f,
                                        rotation = Quaternion.LookRotation(distance)

                                    }, true);
                                }
                            }
                        }
                    }
                }
            }
            orig.Invoke(self, damageInfo);

        }
    }
}