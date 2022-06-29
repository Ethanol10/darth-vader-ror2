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

namespace DarthVaderMod.Modules.Survivors
{
    public class DarthVaderController : MonoBehaviour
    {
        string prefix = DarthVader.DARTHVADER_PREFIX;
        

        public float maxTrackingDistance = 60f;
        public float maxTrackingAngle = 30f;
        public float trackerUpdateFrequency = 10f;
        private Indicator indicator;
        private HurtBox trackingTarget;
        public HurtBox Target;

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private ChildLocator child;
        private readonly BullseyeSearch search = new BullseyeSearch();
        private CharacterMaster characterMaster;

        public DarthVaderMasterController DarthVadermastercon;
        public DarthVaderController DarthVadercon;

        public void Awake()
        {
            child = GetComponentInChildren<ChildLocator>();


            //indicator = new Indicator(gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            characterBody = gameObject.GetComponent<CharacterBody>();
            inputBank = gameObject.GetComponent<InputBankTest>();


        }


        public void Start()
        {

            characterMaster = characterBody.master;
            if (!characterMaster.gameObject.GetComponent<DarthVaderMasterController>())
            {
                DarthVadermastercon = characterMaster.gameObject.AddComponent<DarthVaderMasterController>();
            }

            characterBody.skillLocator.special.RemoveAllStocks();


        }

        //public HurtBox GetTrackingTarget()
        //{
        //    return this.trackingTarget;
        //}

        //private void OnEnable()
        //{
        //    this.indicator.active = true;
        //}

        //private void OnDisable()
        //{
        //    this.indicator.active = false;
        //}


        public void FixedUpdate()
        {

            //this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            //if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            //{
            //    this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
            //    Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
            //    this.SearchForTarget(aimRay);
            //    HurtBox hurtBox = this.trackingTarget;
            //    this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);

                
            //}


        }


        //private void SearchForTarget(Ray aimRay)
        //{
        //    this.search.teamMaskFilter = TeamMask.all;
        //    this.search.filterByLoS = true;
        //    this.search.searchOrigin = aimRay.origin;
        //    this.search.searchDirection = aimRay.direction;
        //    this.search.sortMode = BullseyeSearch.SortMode.Distance;
        //    this.search.maxDistanceFilter = this.maxTrackingDistance;
        //    this.search.maxAngleFilter = this.maxTrackingAngle;
        //    this.search.RefreshCandidates();
        //    this.search.FilterOutGameObject(base.gameObject);
        //    this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        //}


    }





}


