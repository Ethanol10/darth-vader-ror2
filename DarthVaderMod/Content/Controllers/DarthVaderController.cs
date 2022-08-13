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
        public HGTextMeshProUGUI forceNumber;

        //Energy system
        public float maxForceEnergy;
        public float currentForceEnergy;
        public float regenForceEnergy;
        public float costmultiplierForceEnergy;
        public float meleeForceEnergyGain;

        public void Awake()
        {
            child = GetComponentInChildren<ChildLocator>();

            characterBody = gameObject.GetComponent<CharacterBody>();
            inputBank = gameObject.GetComponent<InputBankTest>();


            //Energy
            maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level-1) * StaticValues.levelForceEnergy);
            currentForceEnergy = maxForceEnergy;
            regenForceEnergy = StaticValues.baseRegenForceEnergy + ((characterBody.level - 1) * StaticValues.levelRegenForceEnergy);
            costmultiplierForceEnergy = StaticValues.basecostmultiplierForceEnergy;
            meleeForceEnergyGain = StaticValues.basemeleeForceEnergyGain;

            //UI objects 
            CustomUIObject = UnityEngine.Object.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("darthCustomUI"));
            CustomUIObject.SetActive(false);
            forceMeter = CustomUIObject.transform.GetChild(0).GetComponent<RectTransform>();
            //setup the UI element for the min/max
            forceNumber = this.CreateLabel(CustomUIObject.transform, "forceNumber", $"{0}/{0}", new Vector2(0, -110), 24f);
            forceNumber.enabled = false;

        }

        private void CalculateEnergyStats()
        {
            //Energy updates
            maxForceEnergy = StaticValues.baseForceEnergy + ((characterBody.level - 1) * StaticValues.levelForceEnergy);
            regenForceEnergy = StaticValues.baseRegenForceEnergy + ((characterBody.level - 1) * StaticValues.levelRegenForceEnergy);
            if(costmultiplierForceEnergy > 1f)
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

            Chat.AddMessage($"{currentForceEnergy}/{maxForceEnergy}");
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

        public void Start()
        {
            //characterBody = gameObject.GetComponent<CharacterBody>();
            //characterMaster = characterBody.master;
            //if (!characterMaster.gameObject.GetComponent<DarthVaderMasterController>())
            //{
            //    DarthVadermastercon = characterMaster.gameObject.AddComponent<DarthVaderMasterController>();
            //}

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
            CalculateEnergyStats();

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


