using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DarthVaderMod.Modules.Networking
{
    internal class InflictDamageOnDarthNetworkRequest : INetMessage
    {
        public NetworkInstanceId attackerNetID;
        public NetworkInstanceId darthNetID;
        public float damage;

        public InflictDamageOnDarthNetworkRequest()
        {

        }

        public InflictDamageOnDarthNetworkRequest(NetworkInstanceId attackerNetID, NetworkInstanceId darthNetID, float damage)
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

            //We are here because darth failed to deflect damage. Deal damage to darth. Make the attacker the attacker lmao
            if (NetworkServer.active)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = damage;
                damageInfo.position = darthBody.gameObject.transform.position;
                damageInfo.force = Vector3.zero;
                damageInfo.damageColorIndex = DamageColorIndex.Default;
                damageInfo.crit = false;
                damageInfo.attacker = attackerBody.gameObject;
                damageInfo.inflictor = null;
                damageInfo.damageType = DamageType.Generic;
                damageInfo.procCoefficient = 1f;
                damageInfo.procChainMask = default(ProcChainMask);

                //apply damage to enemy
                darthBody.healthComponent.TakeDamage(damageInfo);
            }
        }
    }
}
