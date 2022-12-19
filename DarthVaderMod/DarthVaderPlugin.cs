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
using DarthVaderMod.Modules.Networking;
using DarthVaderMod.Modules;
using R2API;
using DarthVaderMod.Content.Controllers;
using R2API.Networking.Interfaces;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DarthVaderMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.sound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.networking", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.unlockable", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class DarthVaderPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.PopcornFactory.DarthVaderMod";
        public const string MODNAME = "DarthVaderMod";
        public const string MODVERSION = "2.1.3";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "POPCORN";

        public static DarthVaderPlugin instance;

        //Controller and energy system
        public EnergySystem energySystem;
        public DarthVaderPassive passiveSkillSlot;
        //public DarthVaderMasterController DarthVadermastercon;

        private uint entranceID;
        private uint entranceVoiceID;

        
        public float currentattackspeed;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions")) //risk of options support
            {
                Modules.Config.SetupRiskOfOptions();
            }
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.Unlockables.AddUnlockables(); //add unlockables
            // survivor initialization
            new DarthVader().Initialize(false);

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
            SetupNetworkMessages();

        }
        private void SetupNetworkMessages()
        {
            //Networking
            NetworkingAPI.RegisterMessageType<PerformForceNetworkRequest>();
            NetworkingAPI.RegisterMessageType<EndRageBuffNetworkRequest>();
            NetworkingAPI.RegisterMessageType<DeflectClientHandlerNetworkRequest>();
            NetworkingAPI.RegisterMessageType<InflictDamageOnDarthNetworkRequest>();
            NetworkingAPI.RegisterMessageType<InflictDamageOnEnemyFromDeflectNetworkRequest>();
        }
        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
            On.RoR2.CharacterModel.Awake += CharacterModel_Awake;

            if (Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }

            //Changing death message
            GlobalEventManager.onCharacterDeathGlobal += (damageReport) =>
            {
                // This should never happen, but protect against it just in case
                if (damageReport == null) return;

                // Don't activate for non-player entities
                if (!damageReport.victimBody.isPlayerControlled || !damageReport.victimBody) return;

                // Util.GetBestMasterName gets the userName while checking for null
                var userName = Util.GetBestMasterName(damageReport.victimMaster);

                // For Darth Vader only
                if (damageReport.victimBody.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "There was too much Sand.",
                    });
                }

            };
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
        private void CharacterModel_Awake(On.RoR2.CharacterModel.orig_Awake orig, CharacterModel self)
        {
            orig(self);
            if (self.gameObject.name.Contains("DarthVaderDisplay"))
            {
                entranceVoiceID = AkSoundEngine.PostEvent("DarthVoice", self.gameObject);
                entranceID = AkSoundEngine.PostEvent("DarthIntroTheme", self.gameObject);
            }
            else
            {
                AkSoundEngine.StopPlayingID(entranceVoiceID);
                AkSoundEngine.StopPlayingID(entranceID);
            }

        }
        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            orig.Invoke(self);

            if (self.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
            {
                 AkSoundEngine.PostEvent("DarthDeath", self.gameObject);
            }
        }


        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (damageInfo.attacker != null && damageInfo != null)
            {
                if (damageInfo.attacker.name.Contains("DarthVaderBody"))
                {
                    DarthVaderController darthCon = damageInfo.attacker.GetComponent<DarthVaderController>();

                    if (darthCon)
                    {
                        darthCon.SetMaxDamage(damageInfo.damage);
                    }
                }
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            // What the fuck?
            if (self.healthComponent) 
            {
                orig(self);
                if (self)
                {
                    if (self.baseNameToken == DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME")
                    {
                        EnergySystem darthCon = self.GetComponent<EnergySystem>();
                        if (self.HasBuff(Modules.Buffs.DeflectBuff))
                        {
                            self.moveSpeed *= 0.5f;
                        }

                        if (!self.HasBuff(Modules.Buffs.RageBuff))
                        {
                            float currentmovespeed = self.moveSpeed;
                            if (Modules.Config.limitMovespeed.Value)
                            {
                                if (currentmovespeed > 7f)
                                {
                                    self.moveSpeed = 7f;
                                    float movespeedbonus = currentmovespeed - 7f;
                                    self.armor += movespeedbonus;
                                }
                            }

                            float currentattackspeed = self.attackSpeed;
                            if (darthCon)
                            {
                                darthCon.meleeForceEnergyGain = currentattackspeed;
                            }
                            if (currentattackspeed > 1f)
                            {
                                self.attackSpeed = 1f;
                                float attackspeedbonus = currentattackspeed / 1f;
                                self.damage *= attackspeedbonus;
                            }
                        }
                        else if (self.HasBuff(Modules.Buffs.RageBuff))
                        {
                            self.moveSpeed *= 2f;
                            self.armor = (self.moveSpeed - 7f) * 2f;

                            self.attackSpeed *= 2f;
                            self.damage *= self.attackSpeed;
                        }
                    }
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