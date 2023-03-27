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
        public float jumpTimer;

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
            if (passive)
            {
                if (passive.isEnergyPassive())
                {
                    energySystem = gameObject.AddComponent<EnergySystem>();

                }
            }
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
                if (!characterBody.characterMotor.isGrounded)
                {
                    if(characterBody.characterMotor.velocity.y > 0f)
                    {
                        if (energySystem.currentForceEnergy > 5f)
                        {
                            if (characterBody.inputBank.jump.justPressed)
                            {
                                energySystem.SpendEnergy(StaticValues.jumpEnergyCost);
                                energySystem.TriggerGlow(0.1f, 0.3f, Color.white);
                            }
                            if (characterBody.inputBank.jump.down)
                            {
                                jumpTimer += Time.fixedDeltaTime;
                                if (jumpTimer < 1f)
                                {
                                    characterBody.characterMotor.velocity.y += StaticValues.jumpFlySpeed;
                                    //energySystem.currentForceEnergy -= (StaticValues.jumpDrainForceEnergyFraction * energySystem.maxForceEnergy) * Time.fixedDeltaTime;
                                    energySystem.SpendEnergy(StaticValues.jumpDrainForceEnergyFraction * energySystem.maxForceEnergy);
                                }
                            }
                        }
                        else
                        {
                            energySystem.TriggerGlow(0.1f, 0.3f, Color.blue);
                        }

                    }
                    else if (characterBody.characterMotor.velocity.y <= -10f)
                    {
                        float currentFallSpeed = characterBody.characterMotor.velocity.y;
                        if (characterBody.inputBank.jump.down)
                        {
                            characterBody.characterMotor.velocity.y = -10f;
                            //energySystem.currentForceEnergy -= (StaticValues.jumpDrainForceEnergyFraction * energySystem.maxForceEnergy) * Time.fixedDeltaTime;
                            energySystem.SpendEnergy(StaticValues.jumpDrainForceEnergyFraction * energySystem.maxForceEnergy);
                        }
                        else
                        {
                            characterBody.characterMotor.velocity.y = currentFallSpeed;
                        }

                    }


                }
                else 
                {
                    if (characterBody.characterMotor.isGrounded)
                    {
                        jumpTimer = 0f;
                    }
                    
                }
            }
            else
            {
                if (!characterBody.characterMotor.isGrounded)
                {
                    if (characterBody.characterMotor.velocity.y <= -10f)
                    {
                        float currentFallSpeed = characterBody.characterMotor.velocity.y;
                        if (characterBody.inputBank.jump.down)
                        {
                            characterBody.characterMotor.velocity.y = currentFallSpeed;
                        }
                        else
                        {
                            characterBody.characterMotor.velocity.y = -10f;
                            
                        }
                        
                    }
                }
            }


            if (breathtimer > 3f)
            {
                if (Modules.Config.enableBreathing.Value) 
                {
                    AkSoundEngine.PostEvent("DarthBreathing", this.gameObject);
                }
                breathtimer = 0f;
            }
            else
            {
                breathtimer += Time.fixedDeltaTime;
            }

            if (characterBody.HasBuff(Buffs.RageBuff))
            {
                if (rageTimer > 1f)
                {
                    //random glowing while rage
                    rageTimer = 0f;
                    if (energySystem) energySystem.TriggerGlow(0.2f, 0.5f, new Color(0.3f, 0f, 0.59f, 0.6f));
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
                if (energySystem) energySystem.SetRageState(increasingRageTimer);
            }
            else 
            {
                if (energySystem)
                {
                    if (energySystem.currentForceEnergy < 1f)
                    {
                        increasingRageTimer = 0f;
                    }
                }
            }

        }

        private void OnDestroy()
        {
            StopRageLoop();
        }

        public void PlayRageLoop() 
        {
            if (Modules.Config.enableMusic.Value)
            {
                rageLoopID = AkSoundEngine.PostEvent("DarthRageLooped", characterBody.gameObject);
            }
        }

        public void StopRageLoop() 
        {
            AkSoundEngine.StopPlayingID(rageLoopID);
        }
    }
}


