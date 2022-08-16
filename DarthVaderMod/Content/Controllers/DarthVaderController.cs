using EntityStates;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityStates.LunarExploderMonster;
using RoR2.Projectile;
using EntityStates.MiniMushroom;
using UnityEngine.Networking;
using R2API.Networking;
using RoR2.UI;
using TMPro;
using DarthVaderMod.Modules;
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
        public Image forceMeterGlowBackground;
        public HGTextMeshProUGUI forceNumber;

        //Energy system
        public float maxForceEnergy;
        public float currentForceEnergy;
        public float regenForceEnergy;
        public float costmultiplierForceEnergy;
        public float meleeForceEnergyGain;
        public DarthVaderPassive passiveSkillSlot;

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
            passiveSkillSlot = gameObject.GetComponent<DarthVaderPassive>();
            if (passiveSkillSlot.isEnergyPassive())
            {
                //Energy
                maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy);
                currentForceEnergy = maxForceEnergy;
                regenForceEnergy = StaticValues.baseRegenForceEnergy + ((characterBody.level - 1) * StaticValues.levelRegenForceEnergy);
                costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy;
                meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;

                //UI objects 
                CustomUIObject = UnityEngine.Object.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("darthCustomUI"));
                CustomUIObject.SetActive(true);
                forceMeter = CustomUIObject.transform.GetChild(0).GetComponent<RectTransform>();
                forceMeterGlowBackground = CustomUIObject.transform.GetChild(1).GetComponent<Image>();

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

        private void CalculateEnergyStats()
        {
            //Energy updates
            if (characterBody) 
            {
                maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy);
                regenForceEnergy = StaticValues.baseRegenForceEnergy + ((characterBody.level - 1) * StaticValues.levelRegenForceEnergy);
            }

            if (costmultiplierForceEnergy > 1f)
            {
                costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy;
            }
            if(meleeForceEnergyGain < 1f)
            {
                meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;
            }

            //Energy Currently have
            currentForceEnergy += regenForceEnergy * Time.fixedDeltaTime;
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
                forceMeter.localScale = new Vector3(2.0f * Mathf.Log10( (currentForceEnergy/maxForceEnergy) * 10f ), 0.05f, 1f);
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

            if (characterBody.HasBuff(Modules.Buffs.RageBuff))
            {
                if(characterBody.skillLocator.secondary.stock == 0)
                {
                    characterBody.skillLocator.secondary.AddOneStock();
                };
                if (characterBody.skillLocator.utility.stock == 0)
                {
                    characterBody.skillLocator.utility.AddOneStock();
                };
            }

        }

        public void OnDestroy()
        {
            Destroy(CustomUIObject);
        }
    }
}


