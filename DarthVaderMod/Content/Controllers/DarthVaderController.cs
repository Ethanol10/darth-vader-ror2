using RoR2;
using System;
using UnityEngine;
using RoR2.UI;
using TMPro;
using DarthVaderMod.Content.Controllers;
using UnityEngine.UI;
using R2API.Networking;

namespace DarthVaderMod.Modules.Survivors
{
    public class DarthVaderController : MonoBehaviour
    {
        string prefix = DarthVader.DARTHVADER_PREFIX;

        //breath timing
        public float breathtimer;

        private CharacterBody characterBody;
        private DarthVaderPassive passive;
        private InputBankTest inputBank;
        private ChildLocator child;
        private CharacterMaster characterMaster;
        private EnergySystem energySystem;

        //public DarthVaderMasterController DarthVadermastercon;
        public DarthVaderController DarthVadercon;

        //for achievement
        public float maxDamage;

        public float rageTimer;
        public float increasingRageTimer;

        //rage loop sound
        public uint rageLoopID;

        public void Awake()
        {
            child = GetComponentInChildren<ChildLocator>();

            characterBody = gameObject.GetComponent<CharacterBody>();
            inputBank = gameObject.GetComponent<InputBankTest>();
            passive = gameObject.GetComponent<DarthVaderPassive>();
            Debug.Log($"Passive: {passive}");
            Debug.Log($"Passive.isEnergyPassive(): {passive.isEnergyPassive()}");
            
            if (passive) 
            {
                if(passive.isEnergyPassive())
                {
                    energySystem = gameObject.AddComponent<EnergySystem>();
                }
            }
            Debug.Log($"energySystem: {energySystem}");
        }

        public void SetMaxDamage(float newVal)
        {
            if (newVal > maxDamage)
            {
                maxDamage = newVal;
            }
        }

        public void FixedUpdate()
        {
            if (breathtimer > 3f)
            {
                AkSoundEngine.PostEvent("DarthBreathing", this.gameObject);
                breathtimer = 0f;
            }
            else
            {
                breathtimer += Time.fixedDeltaTime;
            }

            if (characterBody.HasBuff(Buffs.RageBuff))
            {
                if(rageTimer > 1f)
                {
                    //random glowing while rage
                    rageTimer = 0f;
                    energySystem.TriggerGlow(0.4f, 0.5f, Color.red);
                }
                else
                {
                    rageTimer += Time.fixedDeltaTime;
                }
                increasingRageTimer += Time.fixedDeltaTime;

                //infinite skills
                if (characterBody.skillLocator.secondary.stock == 0)
                {
                    characterBody.skillLocator.secondary.AddOneStock();
                };
                if (characterBody.skillLocator.utility.stock == 0)
                {
                    characterBody.skillLocator.utility.AddOneStock();
                };

                //infinite energy, also reduced melee force gain
                if(energySystem) energySystem.SetRageState(increasingRageTimer);
            }
            else if (!characterBody.HasBuff(Buffs.RageBuff))
            {
                if(energySystem) energySystem.ExitRage();
                increasingRageTimer = 0f;
                AkSoundEngine.StopPlayingID(rageLoopID);
            }

            if(characterBody.HasBuff(Buffs.RageBuff) || characterBody.HasBuff(Buffs.DeflectBuff))
            {
                if (energySystem) energySystem.ifEnergyRegenAllowed = false;
            }
            else
            {
                if (!characterBody.HasBuff(Buffs.DeflectBuff))
                {
                    if (energySystem) energySystem.ifEnergyRegenAllowed = true;
                }
                if (!characterBody.HasBuff(Buffs.RageBuff))
                {
                    if (energySystem) energySystem.ifEnergyRegenAllowed = true;
                }
            }
        }
    }
}


