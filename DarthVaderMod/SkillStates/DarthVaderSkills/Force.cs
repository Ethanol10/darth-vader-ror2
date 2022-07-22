using DarthVaderMod.Modules.Survivors;
using DarthVaderMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarthVaderMod.SkillStates
{
    public class Force : BaseSkillState
    {
        public DarthVaderController DarthVadercon;
        public DarthVaderMasterController DarthVadermastercon;
        public HurtBox Target;
        public float maxTrackingDistance = 100f;
        public float maxTrackingAngle = 30f;
        public float pullRange = -80f;
        public float pushRange = 100f;
        private ChildLocator child;
        public GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");
        public float chargeTime = 0.3f;
        public float castTime = 0.2f;
        public float duration;
        public bool hasFired;

        public override void OnEnter()
        {            
            base.OnEnter();
            PlayCrossfade("LeftArm, Override", "ForceStart", "Attack.playbackRate", chargeTime, 0.05f);
            hasFired = false;
            duration = chargeTime + castTime;
            base.StartAimMode(0.5f + this.duration, false);
        }


        public override void OnExit()
        {
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge > chargeTime && base.isAuthority)
            {
                if(base.IsKeyDownAuthority() && !hasFired)
                {
                    AkSoundEngine.PostEvent("DarthForcePull", this.gameObject);
                    hasFired = true;
                    ForcePull();
                    PlayCrossfade("LeftArm, Override", "ForcePull", "Attack.playbackRate", castTime, 0.05f);

                }
                else if (!base.IsKeyDownAuthority() && !hasFired)
                {
                    AkSoundEngine.PostEvent("DarthForcePull", this.gameObject);
                    hasFired = true;
                    ForcePush();
                    PlayCrossfade("LeftArm, Override", "ForcePush", "Attack.playbackRate", castTime, 0.05f);

                }

            }
            
            if(base.fixedAge > duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }


        }

        public void ForcePull()
        {
            Ray aimRay = base.GetAimRay();
            BullseyeSearch search = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
                filterByLoS = true,
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = this.maxTrackingDistance,
                maxAngleFilter = this.maxTrackingAngle,
            };

            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);

            List<HurtBox> target = search.GetResults().ToList<HurtBox>();
            foreach (HurtBox singularTarget in target)
            {
                if (singularTarget)
                {
                    Vector3 a = singularTarget.transform.position - characterBody.corePosition;
                    float magnitude = a.magnitude;
                    Vector3 vector = a / magnitude;

                    if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                    {
                        float Weight = 1f;
                        if (singularTarget.healthComponent.body.characterMotor)
                        {
                            Weight = singularTarget.healthComponent.body.characterMotor.mass;
                        }
                        else if (singularTarget.healthComponent.body.rigidbody)
                        {
                            Weight = singularTarget.healthComponent.body.rigidbody.mass;
                        }
                        Vector3 a2 = vector;
                        float d = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(pullRange - magnitude)) * Mathf.Sign(pullRange - magnitude);
                        a2 *= d;
                        a2.y = -10f;
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = base.gameObject,
                            damage = characterBody.damage * Modules.StaticValues.forcepullDamageCoefficient,
                            position = singularTarget.transform.position,
                            procCoefficient = 1f,
                            damageType = DamageType.Stun1s,

                        };
                        singularTarget.healthComponent.TakeDamageForce(a2 * (Weight / 2), true, true);
                        singularTarget.healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, singularTarget.healthComponent.gameObject);


                        EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
                        {
                            origin = singularTarget.transform.position,
                            scale = 1f,
                            rotation = Quaternion.LookRotation(singularTarget.transform.position - characterBody.corePosition),

                        }, true);

                    }
                }
            }
        }
        public void ForcePush()
        {
            Ray aimRay = base.GetAimRay();
            BullseyeSearch search = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
                filterByLoS = true,
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = this.maxTrackingDistance,
                maxAngleFilter = this.maxTrackingAngle,
            };

            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);

            List<HurtBox> target = search.GetResults().ToList<HurtBox>();
            foreach (HurtBox singularTarget in target)
            {
                if (singularTarget)
                {
                    Vector3 a = singularTarget.transform.position - characterBody.corePosition;
                    float magnitude = a.magnitude;
                    Vector3 vector = a / magnitude;

                    if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                    {
                        float Weight = 1f;
                        if (singularTarget.healthComponent.body.characterMotor)
                        {
                            Weight = singularTarget.healthComponent.body.characterMotor.mass;
                        }
                        else if (singularTarget.healthComponent.body.rigidbody)
                        {
                            Weight = singularTarget.healthComponent.body.rigidbody.mass;
                        }
                        Vector3 a2 = vector;
                        float d = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(pushRange - magnitude)) * Mathf.Sign(pushRange - magnitude);
                        a2 *= d;
                        a2.y = 10f;
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = base.gameObject,
                            damage = characterBody.damage * Modules.StaticValues.forcepushDamageCoefficient,
                            position = singularTarget.transform.position,
                            procCoefficient = 1f,
                            damageType = DamageType.Stun1s,

                        };
                        singularTarget.healthComponent.TakeDamageForce(a2 * (Weight), true, true);
                        singularTarget.healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, singularTarget.healthComponent.gameObject);


                        EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
                        {
                            origin = singularTarget.transform.position,
                            scale = 1f,
                            rotation = Quaternion.LookRotation(singularTarget.transform.position - characterBody.corePosition),

                        }, true);
                    }
                }
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}