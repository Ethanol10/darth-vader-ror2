﻿using DarthVaderMod.Modules.Survivors;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace DarthVaderMod.Modules.Networking
{
    internal class EndRageBuffNetworkRequest : INetMessage
    {
        //Network these ones.
        NetworkInstanceId netID;

        //Don't network these.
        GameObject bodyObj;
        private BullseyeSearch search;
        private List<HurtBox> trackingTargets;
        private GameObject blastEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/effects/SonicBoomEffect");

        public EndRageBuffNetworkRequest()
        {

        }

        public EndRageBuffNetworkRequest(NetworkInstanceId netID)
        {
            this.netID = netID;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
        }

        public void OnReceived()
        {
            GameObject masterobject = Util.FindNetworkObject(netID);
            CharacterMaster charMaster = masterobject.GetComponent<CharacterMaster>();
            CharacterBody charBody = charMaster.GetBody();
            if (NetworkServer.active)
            {
                if (charBody)
                {
                    charBody.RemoveBuff(Buffs.RageBuff.buffIndex);
                }
            }

            DarthVaderController darthVaderCon = charBody.gameObject.GetComponent<DarthVaderController>();
            if (darthVaderCon) 
            {
                darthVaderCon.StopRageLoop();
            }
        }
    }
}