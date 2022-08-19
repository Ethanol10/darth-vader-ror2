using RoR2;
using System;
using UnityEngine;
using RoR2.UI;
using TMPro;
using DarthVaderMod.Content.Controllers;
using UnityEngine.UI;

namespace DarthVaderMod.Modules.Survivors
{
    public class DarthVaderController : MonoBehaviour
    {
        string prefix = DarthVader.DARTHVADER_PREFIX;

        //breath timing
        public float breathtimer;

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private ChildLocator child;
        private CharacterMaster characterMaster;

        //public DarthVaderMasterController DarthVadermastercon;
        public DarthVaderController DarthVadercon;

        //for achievement
        public float maxDamage;


        //UI Forcemeter
        public GameObject CustomUIObject;
        public RectTransform forceMeter;
        public RectTransform forceMeterGlowRect;
        public Image forceMeterGlowBackground;
        public HGTextMeshProUGUI forceNumber;

        //Energy system
        public float maxForceEnergy;
        public float currentForceEnergy;
        public float regenForceEnergy;
        public float drainForceEnergy;
        public float costmultiplierForceEnergy;
        public float costflatForceEnergy;
        public float meleeForceEnergyGain;
        public DarthVaderPassive passiveSkillSlot;
        public bool ifEnergyRegenAllowed;
        public float rageTimer;
        public float increasingRageTimer;

        //Energy bar glow
        private enum GlowState
        {
            STOP,
            FLASH,
            DECAY
        }
        private float decayConst;
        private float flashConst;
        private float glowStopwatch;
        private Color targetColor;
        private Color originalColor;
        private Color currentColor;
        private GlowState state;

        public void Awake()
        {
            child = GetComponentInChildren<ChildLocator>();

            characterBody = gameObject.GetComponent<CharacterBody>();
            inputBank = gameObject.GetComponent<InputBankTest>();
        }


        public void Start()
        {
            //characterBody = gameObject.GetComponent<CharacterBody>();
            //characterMaster = characterBody.master;
            //if (!characterMaster.gameObject.GetComponent<DarthVaderMasterController>())
            //{
            //    DarthVadermastercon = characterMaster.gameObject.AddComponent<DarthVaderMasterController>();
            //}
            ifEnergyRegenAllowed = true;


            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();
            if (passiveSkillSlot.isEnergyPassive())
            {
                //Energy
                maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy);
                currentForceEnergy = maxForceEnergy;
                regenForceEnergy = maxForceEnergy * StaticValues.regenForceEnergyFraction;
                drainForceEnergy = maxForceEnergy * StaticValues.drainForceEnergyFraction;
                costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy;
                costflatForceEnergy = 0f;
                meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;

                //UI objects 
                CustomUIObject = UnityEngine.Object.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("darthCustomUI"));
                CustomUIObject.SetActive(true);
                forceMeter = CustomUIObject.transform.GetChild(0).GetComponent<RectTransform>();
                forceMeterGlowBackground = CustomUIObject.transform.GetChild(1).GetComponent<Image>();
                forceMeterGlowRect = CustomUIObject.transform.GetChild(1).GetComponent<RectTransform>();

                //setup the UI element for the min/max
                forceNumber = this.CreateLabel(CustomUIObject.transform, "forceNumber", $"{(int)currentForceEnergy} / {maxForceEnergy}", new Vector2(0, -110), 24f);
            }

            // Start timer on 1f to turn off the timer.
            state = GlowState.STOP;
            decayConst = 1f;
            flashConst = 1f;
            glowStopwatch = 1f;
            originalColor = new Color(1f, 1f, 1f, 0f);
            targetColor = new Color(1f, 1f, 1f, 1f);
            currentColor = originalColor;
        }

        public void TriggerGlow(float newDecayTimer, float newFlashTimer, Color newStartingColor)
        {
            decayConst = newDecayTimer;
            flashConst = newFlashTimer;
            originalColor = new Color(newStartingColor.r, newStartingColor.g, newStartingColor.b, 0f);
            targetColor = newStartingColor;
            glowStopwatch = 0f;
            state = GlowState.FLASH;
        }

        public void SpendEnergy(float Energy)
        {
            currentForceEnergy -= Energy * costmultiplierForceEnergy + costflatForceEnergy;
        }

        public void MeleeEnergyGain(float Energy)
        {
            currentForceEnergy += Energy * meleeForceEnergyGain;
        }

        private void CalculateEnergyStats()
        {
            //Energy updates
            if (characterBody) 
            {
                maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy) 
                    + (10f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine))
                    + (30f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine));
                regenForceEnergy = maxForceEnergy * StaticValues.regenForceEnergyFraction 
                    * (1 + 0.25f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.AlienHead))
                    + (1 + 5* characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck));
            }

            costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy
                * (-0.75f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.AlienHead));
            costflatForceEnergy -= (5 * characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck));
            if (costmultiplierForceEnergy > 1f)
            {
                costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy;
            }
            if (meleeForceEnergyGain < 1f)
            {
                meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;
            }

            //Energy Currently have
            if (ifEnergyRegenAllowed)
            {
                currentForceEnergy += regenForceEnergy * Time.fixedDeltaTime;
            }

            if (currentForceEnergy > maxForceEnergy)
            {
                currentForceEnergy = maxForceEnergy;
            }

            if (forceNumber) 
            {
                forceNumber.SetText($"{(int)currentForceEnergy} / {maxForceEnergy}");
            }

            if (forceMeter) 
            {
                // 2f because meter is too small probably.
                // Logarithmically scale.
                float logVal = Mathf.Log10( ( (maxForceEnergy / StaticValues.baseForceEnergy) * 10f ) + 1) * (currentForceEnergy / maxForceEnergy);
                forceMeter.localScale = new Vector3(2.0f * logVal, 0.05f, 1f);
                forceMeterGlowRect.localScale = new Vector3(2.3f * logVal, 0.1f, 1f);
            }

            //Chat.AddMessage($"{currentForceEnergy}/{maxForceEnergy}");
        }

        //Creates the label.
        private HGTextMeshProUGUI CreateLabel(Transform parent, string name, string text, Vector2 position, float textScale)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            gameObject.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;
            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = position;
            return hgtextMeshProUGUI;
        }


        public void SetMaxDamage(float newVal)
        {
            if (newVal > maxDamage)
            {
                maxDamage = newVal;
            }
        }

        public void Update()
        {
            if (state != GlowState.STOP)
            {
                glowStopwatch += Time.deltaTime;
                float lerpFraction;
                switch (state)
                {
                    // Lerp to target color
                    case GlowState.FLASH:

                        lerpFraction = glowStopwatch / flashConst;
                        currentColor = Color.Lerp(originalColor, targetColor, lerpFraction);

                        if (glowStopwatch > flashConst)
                        {
                            state = GlowState.DECAY;
                            glowStopwatch = 0f;
                        }
                        break;

                    //Lerp back to original color;
                    case GlowState.DECAY:
                        //Linearlly lerp.
                        lerpFraction = glowStopwatch / decayConst;
                        currentColor = Color.Lerp(targetColor, originalColor, lerpFraction);

                        if (glowStopwatch > decayConst)
                        {
                            state = GlowState.STOP;
                            glowStopwatch = 0f;
                        }
                        break;
                    case GlowState.STOP:
                        //State does nothing.
                        break;
                }
            }

            forceMeterGlowBackground.color = currentColor;
        }

        public void FixedUpdate()
        {
            if (passiveSkillSlot.isEnergyPassive()) 
            {
                CalculateEnergyStats();
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
                    rageTimer = 0f;
                    DarthVadercon.TriggerGlow(0.4f, 0.5f, new Color(UnityEngine.Random.Range(0f, 1.0f), UnityEngine.Random.Range(0f, 1.0f), UnityEngine.Random.Range(0f, 1.0f), 1f));
                }
                else
                {
                    rageTimer += Time.fixedDeltaTime;
                }
                increasingRageTimer += Time.fixedDeltaTime;

                if (characterBody.skillLocator.secondary.stock == 0)
                {
                    characterBody.skillLocator.secondary.AddOneStock();
                };
                if (characterBody.skillLocator.utility.stock == 0)
                {
                    characterBody.skillLocator.utility.AddOneStock();
                };

                ifEnergyRegenAllowed = false;
                currentForceEnergy -= (float)Math.Pow(increasingRageTimer, StaticValues.drainForceEnergyFraction);
                
            }
            else
            {
                increasingRageTimer = 0f;
            }
            if (characterBody.HasBuff(Buffs.DeflectBuff))
            {
                ifEnergyRegenAllowed = false;                
            }

            if(!characterBody.HasBuff(Buffs.RageBuff) && !characterBody.HasBuff(Buffs.DeflectBuff))
            {
                ifEnergyRegenAllowed = true;
            }

        }

        public void OnDestroy()
        {
            Destroy(CustomUIObject);
        }
    }
}


