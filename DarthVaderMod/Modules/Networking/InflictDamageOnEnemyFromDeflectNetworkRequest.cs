using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DarthVaderMod.Modules.Networking
{
    internal class InflictDamageOnEnemyFromDeflectNetworkRequest : INetMessage
    {
        public NetworkInstanceId attackerNetID;
        public NetworkInstanceId darthNetID;
        public float damage;

        public InflictDamageOnEnemyFromDeflectNetworkRequest()
        {

        }

        public InflictDamageOnEnemyFromDeflectNetworkRequest(NetworkInstanceId attackerNetID, NetworkInstanceId darthNetID, float damage)
        {
            this.attackerNetID = attackerNetID;
            this.darthNetID = darthNetID;
            this.damage = damage;
        }

        public void Deserialize(NetworkReader reader)
        {
            attackerNetID = reader.ReadNetworkId();
            darthNetID = reader.ReadNetworkId();
            damage = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(attackerNetID);
            writer.Write(darthNetID);
            writer.Write(damage);
        }

        public void OnReceived()
        {
            GameObject attackerMasterObj = Util.FindNetworkObject(attackerNetID);
            CharacterMaster attackerMaster = attackerMasterObj.GetComponent<CharacterMaster>();
            CharacterBody attackerBody = attackerMaster.GetBody();
            GameObject darthMasterObj = Util.FindNetworkObject(darthNetID);
            CharacterMaster darthMaster = darthMasterObj.GetComponent<CharacterMaster>();
            CharacterBody darthBody = darthMaster.GetBody();

            if (NetworkServer.active)
            {
                if (darthBody && attackerBody)
                {
                    DamageInfo damageInfo2 = new DamageInfo();

                    damageInfo2.damage = damage * 2f * (1f + darthMaster.luck);
                    damageInfo2.position = attackerBody.gameObject.transform.position;
                    damageInfo2.force = Vector3.zero;
                    damageInfo2.damageColorIndex = DamageColorIndex.Default;
                    damageInfo2.crit = Util.CheckRoll(darthBody.crit, darthMaster);
                    damageInfo2.attacker = darthBody.gameObject;
                    damageInfo2.inflictor = null;
                    damageInfo2.damageType = DamageType.Generic;
                    damageInfo2.procCoefficient = 1f;
                    damageInfo2.procChainMask = default(ProcChainMask);

                    //apply damage to enemy
                    attackerBody.healthComponent.TakeDamage(damageInfo2);
                }
            }

            //Visual effect of shooting the laser.
            Vector3 enemyPos = attackerBody.gameObject.transform.position;
            Vector3 distance = (enemyPos - darthBody.gameObject.transform.position);

            if (distance.magnitude >= 3)
            {
                EffectManager.SpawnEffect(Modules.Assets.blasterShotEffect, new EffectData
                {
                    origin = darthBody.gameObject.transform.position,
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
