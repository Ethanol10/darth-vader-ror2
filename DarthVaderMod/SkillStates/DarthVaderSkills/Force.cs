using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Networking;
using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DarthVaderMod.SkillStates
{
    public class Force : BaseSkillState
    {
        public DarthVaderController DarthVadercon;
        public EnergySystem energySystem;
        public DarthVaderPassive passiveSkillSlot;
        //public DarthVaderMasterController DarthVadermastercon;
        public HurtBox Target;
        public float maxTrackingDistance = 100f;
        public float maxTrackingAngle = 30f;
        public float pullRange;
        public float pushRange;
        private ChildLocator child;
        public GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");
        public float chargeTime = 0.25f;
        public float castTime = 0.25f;
        public float duration;
        public bool hasFired;
        public bool isPull;

        public override void OnEnter()
        {            
            base.OnEnter();
            
            isPull = false;
            if (!base.HasBuff(Modules.Buffs.RageBuff))
            {
                pushRange = 150f;
                pullRange = 0f;
            }
            else //nerf rage buff since its infinite spam
            if (base.HasBuff(Modules.Buffs.RageBuff))
            {
                pushRange = 50f;
                pullRange = 0f;
            }

            hasFired = false;

            duration = chargeTime + castTime;
            base.StartAimMode(0.5f + this.duration, false);

            DarthVadercon = characterBody.gameObject.GetComponent<DarthVaderController>();
            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();
            energySystem = characterBody.gameObject.GetComponent<EnergySystem>();

            if (passiveSkillSlot.isEnergyPassive())
            {
                if (energySystem)
                {
                    characterBody.skillLocator.secondary.AddOneStock();

                    if (energySystem.currentForceEnergy > Modules.StaticValues.forcePushPullCost || characterBody.HasBuff(Modules.Buffs.RageBuff))
                    {

                        AkSoundEngine.PostEvent("DarthForcePush", this.gameObject);
                        PlayCrossfade("LeftArm, Override", "ForceStart", "Attack.playbackRate", chargeTime, 0.05f);
                        energySystem.SpendEnergy(Modules.StaticValues.forcePushPullCost);
                        energySystem.TriggerGlow(0.1f, 0.3f, Color.black);

                    }
                    else
                    {
                        energySystem.TriggerGlow(0.1f, 0.3f, Color.blue);
                        this.outer.SetNextStateToMain();
                        return;
                    }
                    

                }

            }
            else
            {
                AkSoundEngine.PostEvent("DarthForcePush", this.gameObject);
                PlayCrossfade("LeftArm, Override", "ForceStart", "Attack.playbackRate", chargeTime, 0.05f);
            }

            
        }


        public override void OnExit()
        {
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > chargeTime && base.isAuthority)
            {
                
                if (base.inputBank.skill2.down && !hasFired)
                {
                    isPull = true;
                    hasFired = true;
                    PlayCrossfade("LeftArm, Override", "ForcePull", "Attack.playbackRate", castTime, 0.05f);

                }
                else if (!hasFired)
                {
                    isPull = false;
                    hasFired = true;
                    PlayCrossfade("LeftArm, Override", "ForcePush", "Attack.playbackRate", castTime, 0.05f);

                }

                if (base.fixedAge > duration && base.isAuthority)
                {
                    if (isPull)
                    {
                        new PerformForceNetworkRequest(base.characterBody.masterObjectId, base.GetAimRay().origin - GetAimRay().direction, base.GetAimRay().direction, pullRange).Send(NetworkDestination.Clients);
                    }
                    else
                    {
                        new PerformForceNetworkRequest(base.characterBody.masterObjectId, base.GetAimRay().origin - GetAimRay().direction, base.GetAimRay().direction, pushRange).Send(NetworkDestination.Clients);
                    }

                    this.outer.SetNextStateToMain();
                    return;
                }

                
            }

            
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}