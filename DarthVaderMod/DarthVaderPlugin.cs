using BepInEx;
using DarthVaderMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using EmotesAPI;
using BepInEx.Bootstrap;
using DarthVaderMod.SkillStates;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DarthVaderMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class DarthVaderPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.PopcornFactory.DarthVaderMod";
        public const string MODNAME = "DarthVaderMod";
        public const string MODVERSION = "0.0.1";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "POPCORN";

        public static DarthVaderPlugin instance;

        public DarthVaderController DarthVadercon;
        public DarthVaderMasterController DarthVadermastercon;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new DarthVader().Initialize(false);

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            //if (Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            //{
            //    On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            //}
        }




        //EMOTES
        private void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig();

            foreach (var item in SurvivorCatalog.allSurvivorDefs)
            {
                if (item.bodyPrefab.name == "DarthVaderBody")
                {
                    CustomEmotesAPI.ImportArmature(item.bodyPrefab, Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("EmoteDarthVader"));
                }
            }
        }


        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
            {
                if (damageInfo != null && damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
                {
                    bool flag = (damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic;
                    if (!flag && damageInfo.damage > 0f)
                    {
                        if (self.body.HasBuff(Modules.Buffs.DeflectBuff.buffIndex))
                        {
                            damageInfo.rejected = true;

                            var damageInfo2 = new DamageInfo();

                            damageInfo2.damage = damageInfo.damage * 2f * self.body.master.luck;
                            damageInfo2.position = damageInfo.attacker.transform.position;
                            damageInfo2.force = Vector3.zero;
                            damageInfo2.damageColorIndex = DamageColorIndex.Default;
                            damageInfo2.crit = Util.CheckRoll(self.body.crit, self.body.master);
                            damageInfo2.attacker = self.gameObject;
                            damageInfo2.inflictor = null;
                            damageInfo2.damageType = DamageType.Generic;
                            damageInfo2.procCoefficient = 1f;
                            damageInfo2.procChainMask = default(ProcChainMask);

                            if (damageInfo.attacker.gameObject.GetComponent<CharacterBody>().baseNameToken
                                != DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME" && damageInfo.attacker != null)
                            {
                                damageInfo.attacker.GetComponent<CharacterBody>().healthComponent.TakeDamage(damageInfo2);
                            }

                            Vector3 enemyPos = damageInfo.attacker.transform.position;
                            EffectManager.SpawnEffect(Modules.Assets.blasterShotEffect, new EffectData
                            {
                                origin = self.body.transform.position,
                                scale = 1f,
                                rotation = Quaternion.LookRotation(enemyPos - self.body.transform.position)

                            }, true);


                        }

                    }


                }
            }
            orig.Invoke(self, damageInfo);
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
           
            orig.Invoke(self, damageInfo, victim);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if(self.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
            {
                if (self.HasBuff(Modules.Buffs.DeflectBuff))
                {
                    self.moveSpeed *= 0.5f;
                }

                if (!self.HasBuff(Modules.Buffs.RageBuff))
                {
                    float currentmovespeed = self.moveSpeed;
                    if (currentmovespeed > 6f)
                    {
                        self.moveSpeed = 6f;
                        float movespeedbonus = currentmovespeed - 6f;
                        self.armor += movespeedbonus;
                    }
                    float currentattackspeed = self.attackSpeed;
                    if (currentattackspeed > 1f)
                    {
                        self.attackSpeed = 1f;
                        float attackspeedbonus = currentattackspeed / 1f;
                        self.damage *= attackspeedbonus;
                    }



                }
                else  if(self.HasBuff(Modules.Buffs.RageBuff))
                {
                    self.moveSpeed *= 2f;
                    self.armor = (self.moveSpeed - 6f) * 2f;

                    self.attackSpeed *= 2f;
                    self.damage *= self.attackSpeed;


                }

            }
                        
        }

        //private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        //{
        //    orig(self);

        //    if (self)
        //    {
        //        if (self.body)
        //        {
        //            this.OverlayFunction(Modules.Assets.SpatialMovementBuffMaterial, self.body.HasBuff(Modules.Buffs.SpatialMovementBuff), self);
        //        }
        //    }
        //}

        //private void OverlayFunction(Material overlayMaterial, bool condition, CharacterModel model)
        //{
        //    if (model.activeOverlayCount >= CharacterModel.maxOverlays)
        //    {
        //        return;
        //    }
        //    if (condition)
        //    {
        //        Material[] array = model.currentOverlays;
        //        int num = model.activeOverlayCount;
        //        model.activeOverlayCount = num + 1;
        //        array[num] = overlayMaterial;
        //    }
        //}
    }
}