using DarthVaderMod.Content.Controllers;
using RoR2;
using UnityEngine;

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
            characterBody = gameObject.GetComponent<CharacterBody>();
            inputBank = gameObject.GetComponent<InputBankTest>();
            passive = gameObject.GetComponent<DarthVaderPassive>();
            child = GetComponentInChildren<ChildLocator>();
        }

        public void Start()
        {


            Debug.Log($"Passive: {passive}");
            Debug.Log($"Passive.isEnergyPassive(): {passive.isEnergyPassive()}");

            if (passive)
            {
                if (passive.isEnergyPassive())
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
            if (passive.isEnergyPassive())
            {
                if (!characterBody.characterMotor.isGrounded && energySystem.currentForceEnergy > 5f)
                {
                    if (characterBody.inputBank.jump.justPressed)
                    {
                        energySystem.SpendEnergy(StaticValues.jumpEnergyCost);
                    }
                    if (characterBody.inputBank.jump.down)
                    {
                        characterBody.characterMotor.velocity.y = StaticValues.jumpFlySpeed * (characterBody.moveSpeed / 7f);
                        energySystem.currentForceEnergy -= (StaticValues.jumpDrainForceEnergyFraction * energySystem.maxForceEnergy) * Time.fixedDeltaTime;

                    }

                }
                else
                {
                    energySystem.TriggerGlow(0.1f, 0.3f, Color.blue);
                }
            }


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
                    energySystem.TriggerGlow(0.2f, 0.5f, new Color(0.3f,0f,0.59f,0.6f));
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


