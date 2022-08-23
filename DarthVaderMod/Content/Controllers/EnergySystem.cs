using System;
using DarthVaderMod.Modules;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarthVaderMod.Content.Controllers
{
    public class EnergySystem : MonoBehaviour
    {
        public CharacterBody characterBody;

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
        public float rageMeleeMultiplier;
        public float rageEnergyCost;
        public float energyDecayTimer;
        public bool ifEnergyUsed;

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
            characterBody = gameObject.GetComponent<CharacterBody>();
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
            if (characterBody.hasEffectiveAuthority)
            {
                if (passiveSkillSlot.isEnergyPassive())
                {
                    //Energy
                    maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy);
                    currentForceEnergy = maxForceEnergy;
                    regenForceEnergy = maxForceEnergy * StaticValues.regenForceEnergyFraction;
                    drainForceEnergy = maxForceEnergy * StaticValues.drainForceEnergyFraction;
                    costmultiplierForceEnergy = 1f;
                    costflatForceEnergy = 0f;
                    meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;
                    rageMeleeMultiplier = 1f;
                    rageEnergyCost = 1f;
                    ifEnergyUsed = false;

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
            hgtextMeshProUGUI.color = Color.red;
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

        private void CalculateEnergyStats()
        {
            //Energy updates
            if (characterBody)
            {
                maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy)
                    + (10f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.SecondarySkillMagazine))
                    + (30f * characterBody.master.inventory.GetItemCount(RoR2Content.Items.UtilitySkillMagazine));
                regenForceEnergy = maxForceEnergy * StaticValues.regenForceEnergyFraction;

                costmultiplierForceEnergy = (float)Math.Pow(0.75f, characterBody.master.inventory.GetItemCount(RoR2Content.Items.AlienHead));
                costflatForceEnergy = (5 * characterBody.master.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck));

                if (costmultiplierForceEnergy > 1f)
                {
                    costmultiplierForceEnergy = 1f;
                }
                if (meleeForceEnergyGain < 1f)
                {
                    meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;
                }
            }

            //Energy used
            if (ifEnergyUsed)
            {
                if (energyDecayTimer > 2f)
                {
                    energyDecayTimer = 0f;
                    ifEnergyRegenAllowed = true;
                    ifEnergyUsed = false;
                }
                else
                {
                    ifEnergyRegenAllowed = false;
                    energyDecayTimer += Time.fixedDeltaTime;
                }
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
            //stop Rage mode
            if (currentForceEnergy < 0f)
            {
                currentForceEnergy = 0f;
                characterBody.RemoveBuff(Modules.Buffs.RageBuff);
            }

            if (forceNumber)
            {
                forceNumber.SetText($"{(int)currentForceEnergy} / {maxForceEnergy}");
            }

            if (forceMeter)
            {
                // 2f because meter is too small probably.
                // Logarithmically scale.
                float logVal = Mathf.Log10(((maxForceEnergy / StaticValues.baseForceEnergy) * 10f) + 1) * (currentForceEnergy / maxForceEnergy);
                forceMeter.localScale = new Vector3(2.0f * logVal, 0.05f, 1f);
                forceMeterGlowRect.localScale = new Vector3(2.3f * logVal, 0.1f, 1f);
            }

            //Chat.AddMessage($"{currentForceEnergy}/{maxForceEnergy}");
        }

        public void FixedUpdate()
        {
            if (passiveSkillSlot.isEnergyPassive())
            {
                CalculateEnergyStats();
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

        public void MeleeEnergyGain(float Energy)
        {
            currentForceEnergy += rageMeleeMultiplier * (Energy * meleeForceEnergyGain);
            //Chat.AddMessage(rageMeleeMultiplier * (Energy * meleeForceEnergyGain) + "melee energy");
        }

        public void SpendEnergy(float Energy)
        {
            float energyflatCost = Energy - costflatForceEnergy;
            if (energyflatCost < 0f) energyflatCost = 0f;

            float energyCost = rageEnergyCost * costmultiplierForceEnergy * energyflatCost;
            if (energyCost < 0f) energyCost = 0f;

            currentForceEnergy -= energyCost;

            ifEnergyUsed = true;
            //Chat.AddMessage(rageEnergyCost + "rageEnergyCost");
            //Chat.AddMessage(Energy + "Energy");
            //Chat.AddMessage(costmultiplierForceEnergy + "costmultiplierForceEnergy");
            //Chat.AddMessage(costflatForceEnergy + "costflatForceEnergy");
            //Chat.AddMessage(rageEnergyCost * ((Energy - costflatForceEnergy) * costmultiplierForceEnergy) + "spentenergy");
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

        public void SetRageState()
        {
            //infinite energy, also reduced melee force gain
            rageEnergyCost = 0f;
            rageMeleeMultiplier = 0.5f;
            ifEnergyRegenAllowed = false;
            currentForceEnergy -= (float)Math.Pow(increasingRageTimer, StaticValues.drainForceEnergyFraction) * Time.fixedDeltaTime;
        }

        public void ExitRage()
        {
            rageMeleeMultiplier = 1f;
            increasingRageTimer = 0f;
            ifEnergyRegenAllowed = true;
            rageEnergyCost = 1f;
        }

        public void OnDestroy()
        {
            Destroy(CustomUIObject);
        }
    }
}

