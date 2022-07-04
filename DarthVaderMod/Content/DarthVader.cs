using BepInEx.Configuration;
using EntityStates;
using DarthVaderMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarthVaderMod.Modules.Survivors
{
    internal class DarthVader : SurvivorBase
    {
        public override string bodyName => "DarthVader";

        public const string DARTHVADER_PREFIX = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => DARTHVADER_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "DarthVaderBody",
            bodyNameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_NAME",
            subtitleNameToken = DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texDarthVaderIcon"),
            bodyColor = Color.red,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 377f,
            healthRegen = 1f,
            armor = 10f,
            damage = Config.basedamage.Value,
            healthGrowth = 78f,
            jumpCount = 1,
            moveSpeed = 6f,
            jumpPower = 15f,
            
        };

        //internal static Material DarthVaderMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("DarthVaderMat");
        internal static Material DarthVaderMat = Modules.Materials.CreateHopooMaterial("DarthVaderMat", false);
        internal static Material DarthVaderCulledMat = Modules.Materials.CreateHopooMaterial("DarthVaderMat", true);
        internal static Material LightSaberGripMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("LightSaberMat");
        internal static Material LightSaberRedMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("LightsaberRed");
        //internal static Material DarthVaderEmptyMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("EmptyMat");
        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Cape",
                    material = DarthVaderCulledMat,
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = DarthVaderMat,
                },
                new CustomRendererInfo
                {
                    childName = "LightsaberGrip",
                    material = LightSaberGripMat,
                },
                new CustomRendererInfo
                {
                    childName = "LightsaberBlade",
                    material = LightSaberRedMat,
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new DarthVaderItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter(bool isHidden)
        {
            base.InitializeCharacter(isHidden);
            bodyPrefab.AddComponent<DarthVaderController>();
            //EntityStateMachine DarthVaderEntityStateMachine = bodyPrefab.GetComponent<EntityStateMachine>();
            //DarthVaderEntityStateMachine.initialStateType = new SerializableEntityStateType(typeof(SkillStates.BaseStates.SpawnState));
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            //example of how to create a hitbox
            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = DarthVaderPlugin.DEVELOPER_PREFIX;

            #region Primary
            //Creates a skilldef for a typical primary 
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "_DARTHVADER_BODY_PRIMARY_SLASH_NAME",
                                                                                      prefix + "_DARTHVADER_BODY_PRIMARY_SLASH_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.SlashCombo)),
                                                                                      "Weapon",
                                                                                      true));


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef shootSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DARTHVADER_BODY_SECONDARY_GUN_NAME",
                skillNameToken = prefix + "_DARTHVADER_BODY_SECONDARY_GUN_NAME",
                skillDescriptionToken = prefix + "_DARTHVADER_BODY_SECONDARY_GUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Force)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, shootSkillDef);
            #endregion

            #region Utility
            SkillDef spatialSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DARTHVADER_BODY_UTILITY_ROLL_NAME",
                skillNameToken = prefix + "_DARTHVADER_BODY_UTILITY_ROLL_NAME",
                skillDescriptionToken = prefix + "_DARTHVADER_BODY_UTILITY_ROLL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Deflect)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, spatialSkillDef);
            #endregion

            #region Special
            SkillDef transformSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DARTHVADER_BODY_SPECIAL_BOMB_NAME",
                skillNameToken = prefix + "_DARTHVADER_BODY_SPECIAL_BOMB_NAME",
                skillDescriptionToken = prefix + "_DARTHVADER_BODY_SPECIAL_BOMB_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.RageMode)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 90f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, transformSkillDef);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            Material defaultMat = Modules.Materials.CreateHopooMaterial("DarthVaderMat", false);
            Material defaultculledMat = Modules.Materials.CreateHopooMaterial("DarthVaderMat", true);
            Material lightsabergripMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("LightSaberMat");
            Material lightsaberredMat = Modules.Assets.mainAssetBundle.LoadAsset<Material>("LightsaberRed");
            CharacterModel.RendererInfo[] defaultRendererInfo = SkinRendererInfos(defaultRenderers, new Material[] {
                defaultMat,
                defaultculledMat,
                lightsabergripMat,
                lightsaberredMat,

            });
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererInfo,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                //place your mesh replacements here
                //unnecessary if you don't have multiple skins
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("DarthVaderMesh"),
                    renderer = defaultRendererInfo[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("CapeMesh"),
                    renderer = defaultRendererInfo[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("LightsaberGripMesh"),
                    renderer = defaultRendererInfo[2].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("LightsaberBladeMesh"),
                    renderer = defaultRendererInfo[3].renderer
                }
            };

            skins.Add(defaultSkin);
            #endregion

            //masked skin
            #region maskedSkin
            //CharacterModel.RendererInfo[] maskedrendererInfos = SkinRendererInfos(defaultRenderers, new Material[] {
            //    defaultMat,
            //    defaultMat,
            //    defaultMat,
            //    defaultMat,
            //});
            //SkinDef maskedSkin = Modules.Skins.CreateSkinDef(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_MASKED_SKIN_NAME",
            //    Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
            //    maskedrendererInfos,
            //    mainRenderer,
            //    model);

            //maskedSkin.meshReplacements = new SkinDef.MeshReplacement[]
            //{
            //    //place your mesh replacements here
            //    //unnecessary if you don't have multiple skins
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("DarthVaderMaskMesh"),
            //        renderer = maskedrendererInfos[0].renderer
            //    },
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("DarthVaderWingMesh"),
            //        renderer = maskedrendererInfos[1].renderer
            //    },
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("DarthVaderSwordMesh"),
            //        renderer = maskedrendererInfos[2].renderer
            //    },
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("DarthVaderMesh"),
            //        renderer = maskedrendererInfos[3].renderer
            //    }
            //};

            //skins.Add(maskedSkin);
            #endregion
            //uncomment this when you have a mastery skin
            #region MasterySkin

            //Material masteryMat = Modules.Materials.CreateHopooMaterial("matDarthVaderAlt");
            //CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            //{
            //    masteryMat,
            //    masteryMat,
            //    masteryMat,
            //    masteryMat
            //});

            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_MASTERY_SKIN_NAME",
            //    Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    masteryRendererInfos,
            //    mainRenderer,
            //    model,
            //    masterySkinUnlockableDef);

            //masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            //{
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDarthVaderSwordAlt"),
            //        renderer = defaultRenderers[0].renderer
            //    },
            //    new SkinDef.MeshReplacement
            //    {
            //        mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDarthVaderAlt"),
            //        renderer = defaultRenderers[2].renderer
            //    }
            //};

            //skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();


        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[2].defaultMaterial = materials[2];
            newRendererInfos[3].defaultMaterial = materials[3];

            return newRendererInfos;
        }
    }
}
