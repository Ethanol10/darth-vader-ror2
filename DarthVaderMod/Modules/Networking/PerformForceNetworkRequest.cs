﻿using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace DarthVaderMod.Modules.Networking
{
    internal class PerformForceNetworkRequest : INetMessage
    {
        //Network these ones.
        NetworkInstanceId netID;
        Vector3 origin;
        Vector3 direction;
        private float pullRange;

        //Don't network these.
        GameObject bodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;
        private GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");

        public PerformForceNetworkRequest()
        {

        }

        public PerformForceNetworkRequest(NetworkInstanceId netID, Vector3 origin, Vector3 direction, float pullRange)
        {
            this.netID = netID;
            this.origin = origin;
            this.direction = direction;
            this.pullRange = pullRange;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            origin = reader.ReadVector3();
            direction = reader.ReadVector3();
            pullRange = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(origin);
            writer.Write(direction);
            writer.Write(pullRange);
        }

        public void OnReceived()
        {

            if (NetworkServer.active)
            {
                search = new BullseyeSearch();
                //Spawn the effect around this object.
                GameObject masterobject = Util.FindNetworkObject(netID);
                CharacterMaster charMaster = masterobject.GetComponent<CharacterMaster>();
                CharacterBody charBody = charMaster.GetBody();
                bodyObj = charBody.gameObject;

                //Check targets in range
                SearchForTarget(charBody);
                //Pull targets and stun
                PullTargets(charBody);
            }
        }

        //Don't do anything until we have a target.
        //Pull target: let's use a blast attack pointed towards the player with a slight upwards force.
        //directional vector: terminal point - initial point.
        //get mass, apply force. object mass * forcemultiplier for enemies.
        //Make direction point towards player.
        private void PullTargets(CharacterBody charBody)
        {
            if (trackingTargets.Count > 0)
            {
                foreach (HurtBox singularTarget in trackingTargets)
                {

                    Vector3 a = singularTarget.transform.position - origin;
                    float magnitude = a.magnitude;
                    Vector3 vector = a / magnitude;

                    if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                    {
                        float Weight = 1f;
                        if (singularTarget.healthComponent.body.isBoss)
                        {
                            if (singularTarget.healthComponent.body.characterMotor)
                            {
                                Weight = singularTarget.healthComponent.body.characterMotor.mass/5;
                            }
                            else if (singularTarget.healthComponent.body.rigidbody)
                            {
                                Weight = singularTarget.healthComponent.body.rigidbody.mass/5;
                            }

                        }
                        else
                        {
                            if (singularTarget.healthComponent.body.characterMotor)
                            {
                                Weight = singularTarget.healthComponent.body.characterMotor.mass;
                            }
                            else if (singularTarget.healthComponent.body.rigidbody)
                            {
                                Weight = singularTarget.healthComponent.body.rigidbody.mass;
                            }

                        }

                        Vector3 a2 = vector;
                        float d = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(pullRange - magnitude)) * Mathf.Sign(pullRange - magnitude);
                        a2 *= d;
                        //a2.y = -30f;
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = bodyObj,
                            damage = charBody.damage * Modules.StaticValues.forcepullDamageCoefficient,
                            position = singularTarget.transform.position,
                            procCoefficient = 1f,
                            damageType = DamageType.Stun1s,
                            crit = charBody.RollCrit(),

                        };

                        singularTarget.healthComponent.TakeDamageForce(a2 * (Weight), true, true);
                        singularTarget.healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, singularTarget.healthComponent.gameObject);


                        EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
                        {
                            origin = singularTarget.transform.position,
                            scale = 1f,
                            rotation = Quaternion.LookRotation(singularTarget.transform.position - origin),

                        }, true);
                    }
                }
            }
        }

        private void SearchForTarget(CharacterBody charBody)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(charBody.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = origin;
            this.search.searchDirection = direction;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = Modules.StaticValues.forceMaxRange;
            this.search.maxAngleFilter = Modules.StaticValues.forceMaxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(charBody.gameObject);
            this.trackingTargets = this.search.GetResults().ToList<HurtBox>();
        }
    }
}
