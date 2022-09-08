using DarthVaderMod.Content.Controllers;
using DarthVaderMod.Modules.Survivors;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace DarthVaderMod.Modules.Networking
{
    internal class DeflectClientHandlerNetworkRequest : INetMessage
    {
        //Network these ones.
        public NetworkInstanceId attackerNetID;
        public NetworkInstanceId darthNetID;
        public float damage;


        public DeflectClientHandlerNetworkRequest()
        {

        }

        public DeflectClientHandlerNetworkRequest(NetworkInstanceId attackerNetID, NetworkInstanceId darthNetID, float damage)
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

            if (darthBody.hasEffectiveAuthority) 
            {
                EnergySystem energySystem = darthBody.GetComponent<EnergySystem>();

                if (energySystem) 
                {
                    //If we have enough energy, spend it and deflect.
                    if (energySystem.currentForceEnergy > Modules.StaticValues.deflectPerHitCost) 
                    {
                        Chat.AddMessage("Spending Energy");
                        energySystem.SpendEnergy(Modules.StaticValues.deflectPerHitCost);
                        energySystem.TriggerGlow(0.1f, 0.3f, Color.black);
                        AkSoundEngine.PostEvent("DarthDeflect", darthBody.gameObject);
                        new InflictDamageOnEnemyFromDeflectNetworkRequest(attackerNetID, darthNetID, damage).Send(NetworkDestination.Clients);
                        return;   
                    }

                    //Otherwise, make darth take damage.
                    Chat.AddMessage("Inflicting damage on darth.");
                    energySystem.TriggerGlow(0.1f, 0.3f, Color.blue);
                    new InflictDamageOnDarthNetworkRequest(attackerNetID, darthNetID, damage).Send(NetworkDestination.Clients);
                }
            }

            return;
        }
    }
}
