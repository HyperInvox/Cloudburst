using EntityStates;


using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;
using static RoR2.DotController;
using Cloudburst.Cores.HAND.Components;
using Cloudburst.Cores.HAND.Skills;
using Cloudburst.Cores.States.HAND;
using Cloudburst.Cores.States.Wyatt;
using Cloudburst.Cores.Components.Wyatt;
using UnityEngine.Networking;
using Cloudburst.Cores.Components;
using System.Collections.Generic;
using Cloudburst.Cores.Skills;
using Cloudburst.Cores.States.Bombardier;
using R2API;
using R2API.Utils;

namespace Cloudburst.Cores.HAND
{
    public class WyattCore
    {
        #region Instance and logger
        public static WyattCore instance;
        #endregion
        #region Prefabs
        public GameObject winch;
        public GameObject wyattBody;
        public GameObject sunder;
        #endregion
        #region Components
        public SkillLocator skillLocator;
        //public OverclockComponent overclockComponent;
        public DroneComponent droneComponent;
        //public HANDPassiveController passiveContoller;
        public CharacterBody characterBody;
        public WyattComboScript script;
        #endregion

        public GameObject wyattMonsterMaster;

        public static string trashOutStateName;

        public const string lore = @"Can't stop now. Can't stop now. Every step I take is a step I can't take back. Come hell or high water I will find it.

It's all a rhythm, just a rhythm. Every time I step out of line is a punishment. I will obey the groove. Nothing can stop me now.

Every scar is worth it. I can feel it, I am coming closer to it. I will have it, and it will be mine.

No matter the blood, it's worth it. I will find what I want, and I will come home.

No matter how many I slaughter, it will be mine... It can't hide from me from me forever.

It's here, I can feel it. This security chest, it has it. I crack it open, and I find it.

It's... finally mine. I hold it in my bruised hands. Has it really been years? 

She'll love this, I know.

";

        public SkillDef throwPrimary;
        public SkillDef retrievePrimary;
        public WyattCore() => Load();

        public void Load()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };

            if (instance == null)
            {
                instance = this;
            }
            CreateWYATT();
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };

#if Release
            LogW("You're on a debug build. If you see this after downloading from the thunderstore, panic!");
            
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };
#endif
        }
        private void CreateWYATT()
        {
            CreateTokens();
         //   CreateWYATTPrefab();
            CreateSunderPrefab();
            //CreateWinchPrefab();
            SetComponents();
            SetSkills();
            CreateUmbra();

            On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += SlowDownProjectiles_OnTriggerEnter;
        }

        private void CreateUmbra()
        {
            wyattMonsterMaster = Resources.Load<GameObject>("prefabs/charactermasters/LoaderMonsterMaster").InstantiateClone("WyattMonsterMaster", true);
            CloudUtils.RegisterNewMaster(wyattMonsterMaster);

            wyattMonsterMaster.GetComponent<CharacterMaster>().bodyPrefab = wyattBody;
        }

        private void SlowDownProjectiles_OnTriggerEnter(On.RoR2.Projectile.SlowDownProjectiles.orig_OnTriggerEnter orig, SlowDownProjectiles self, Collider other)
        {
            bool shouldOrigSelf = true;
            //LogCore.LogI(other.gameObject.name);
            //var sphere = other as SphereCollider;
            if (other.gameObject.name == "WyattWinch(Clone)")
            {
                shouldOrigSelf = false;

            }
            var con = other.gameObject.GetComponent<ProjectileController>();
            if (con)
            {
                if (con.teamFilter && con.teamFilter.teamIndex == TeamIndex.Player)
                {
                    shouldOrigSelf = false;
                }
            }

            if (shouldOrigSelf)
            {
                orig(self, other);
            }
        }
        #region Orbs

        #endregion
        #region Prefab Creation
        private void CreateSunderPrefab()
        {
            sunder = Resources.Load<GameObject>("prefabs/projectiles/Sunder").InstantiateClone("WYATTSunder", true);
            CloudUtils.RegisterNewProjectile(sunder);
            UnityEngine.Object.Destroy(sunder.GetComponent<ProjectileOverlapAttack>());

            var overlap = sunder.AddComponent<ShockwaveProjectileComponent>();
            overlap.damageCoefficient = 2;
            overlap.impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFX");
            overlap.forceVector = new Vector3(0.0f, 1400.0f, 0.0f);
            overlap.overlapProcCoefficient = 1;
            overlap.maximumOverlapTargets = 100;
            overlap.fireFrequency = 60;
            overlap.resetInterval = -1f;
        }

        private void CreateWinchPrefab()
        {
            GameObject groveWinch = Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectileSimple");

            this.winch = Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile").InstantiateClone("WYATTWinch", true);

            var ghost = UnityEngine.Object.Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlSatchel"));

            var controller = ghost.AddComponent<ProjectileGhostController>();
            var attributes = ghost.AddComponent<VFXAttributes>();

            attributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            attributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            //attributes.optionalLights = Array.Empty<Light>();
            //attributes.secondaryParticleSystem = Array.Empty<ParticleSystem>();


            this.winch.GetComponent<ProjectileController>().ghostPrefab = ghost;

            //this is retarded
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<ProjectileSingleTargetImpact>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<ProjectileDamage>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<ProjectileSimple>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<ProjectileController>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<TeamFilter>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<ProjectileNetworkTransform>());
            UnityEngine.Object.DestroyImmediate(winch.GetComponent<NetworkIdentity>());

            winch.layer = LayerIndex.projectile.intVal;

            NetworkIdentity networkIdentity = winch.AddComponent<NetworkIdentity>();
            ProjectileController projectileController = winch.AddComponent<ProjectileController>();
            ProjectileNetworkTransform projectileNetworkTransform = winch.AddComponent<ProjectileNetworkTransform>();
            ProjectileSimple projectileSimple = winch.AddComponent<ProjectileSimple>();
            TeamFilter teamFilter = winch.AddComponent<TeamFilter>();
            ProjectileSingleTargetImpact projectileSingleTargetImpact = winch.AddComponent<ProjectileSingleTargetImpact>();
            ProjectileDamage projectileDamage = winch.AddComponent<ProjectileDamage>();
            SphereCollider sphereCollider = winch.GetComponent<SphereCollider>();

            networkIdentity.localPlayerAuthority = true;
            networkIdentity.serverOnly = false;

            projectileController.ghostPrefab = ghost;
            projectileController.isPrediction = false;
            projectileController.allowPrediction = true;
            projectileController.predictionId = 0;
            projectileController.procChainMask = default;
            projectileController.procCoefficient = 1;

            projectileNetworkTransform.positionTransmitInterval = 0.03333334f;
            projectileNetworkTransform.interpolationFactor = 1;
            projectileNetworkTransform.allowClientsideCollision = false;

            projectileSimple.velocity = 280;
            projectileSimple.lifetime = 3;
            projectileSimple.updateAfterFiring = false;
            projectileSimple.enableVelocityOverLifetime = false;
            projectileSimple.velocityOverLifetime = groveWinch.GetComponent<ProjectileSimple>().velocityOverLifetime;

            projectileSingleTargetImpact.destroyWhenNotAlive = true;
            projectileSingleTargetImpact.destroyOnWorld = true;
            projectileSingleTargetImpact.impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffects/omniimpactvfxslash");

            projectileDamage.damage = 0;
            projectileDamage.crit = false;
            projectileDamage.force = 0;
            projectileDamage.damageColorIndex = DamageColorIndex.Default;
            projectileDamage.damageType = DamageType.BypassArmor | DamageType.Stun1s;

            CloudUtils.RegisterNewProjectile(winch);
            ProjectileProximityBeamController beamController = winch.AddComponent<ProjectileProximityBeamController>();

            ProjectileDamage damage = winch.GetComponent<ProjectileDamage>();

            MissileController missile = winch.GetComponent<MissileController>();

            winch.GetComponent<ProjectileSingleTargetImpact>().impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffects/omniimpactvfxslash");

            beamController.attackFireCount = 1;
            beamController.attackInterval = 0.01f;
            beamController.listClearInterval = 0.33f;
            beamController.attackRange = 5;
            beamController.minAngleFilter = 0;
            beamController.maxAngleFilter = 180;
            beamController.procCoefficient = 0;
            beamController.damageCoefficient = 0.3f;
            beamController.bounces = 0;
            beamController.inheritDamageType = false;
            beamController.lightningType = RoR2.Orbs.LightningOrb.LightningType.Ukulele;

            damage.damageType = DamageType.Stun1s;
            damage.force = -5000;

            missile.deathTimer = 4;
            missile.giveupTimer = 4;
            missile.giveupTimer = 25;

         //   API.RegisterNewProjectile(this.winch);
            #region Old
            #endregion
        }

        #endregion
        #region Components
        private void SetComponents()
        {
            skillLocator = wyattBody.GetComponent<SkillLocator>();
            //overclockComponent = wyattBody.AddComponent<OverclockComponent>();
            //  droneComponent = wyattBody.AddComponent<DroneComponent>();
            characterBody = wyattBody.GetComponent<CharacterBody>();
            MAIDManager janniePower = wyattBody.AddComponent<MAIDManager>();
            SfxLocator sfxLocator = wyattBody.GetComponent<SfxLocator>();
            //CharacterDeathBehavior characterDeathBehavior = wyattBody.GetComponent<CharacterDeathBehavior>();
            HANDDroneTracker tracker = wyattBody.AddComponent<HANDDroneTracker>();
            script = wyattBody.AddComponent<WyattComboScript>();
            wyattBody.AddComponent<WyattWalkmanBehavior>();
            //kil
            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(DeathState));
            //characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(DeathState));

            //sfx
            sfxLocator.fallDamageSound = "Play_MULT_shift_hit";
            sfxLocator.landingSound = "play_char_land";

            var indicator = Resources.Load<GameObject>("Prefabs/EngiShieldRetractIndicator").transform.Find("Holder").GetComponent<SpriteRenderer>(); //
            indicator.sprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("texWyattIndicator");
            indicator.color = CloudUtils.HexToColor("00A86B");

            tracker.maxTrackingAngle = 20;
            tracker.maxTrackingDistance = 55;
            tracker.trackerUpdateFrequency = 5;
            tracker.indicatorPrefab = Resources.Load<GameObject>("Prefabs/EngiShieldRetractIndicator");

            SetCharacterBody();
            FixSetStateOnHurt();
        }

        private void FixSetStateOnHurt()
        {
            SetStateOnHurt hurtState = wyattBody.AddOrGetComponent<SetStateOnHurt>(); //AddOrGetComponent is because 

            hurtState.canBeFrozen = true; //If this gameobject can be frozen. Set to true if this is a survivor
            hurtState.canBeHitStunned = false; //If this gameobject can be stunned by being hit. Set to false if this is a survivor.
            hurtState.canBeStunned = false; //If this gameobject is able to be stunned. Set to false if this is a survivor
            hurtState.hitThreshold = 5f; //The hit threshold, set to 5 if this is a survivor.

            var mach = wyattBody.AddComponent<EntityStateMachine>();
            int i = 0;
            EntityStateMachine[] esmr = new EntityStateMachine[3];
            mach.customName = "TrashOut";
            mach.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            mach.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));

            foreach (EntityStateMachine esm in wyattBody.GetComponentsInChildren<EntityStateMachine>())
            {
                // LogCore.LogI(esm.gameObject.name);
                //LogCore.LogI(esm.customName);
                switch (esm.customName)
                {
                    case "Body":
                        hurtState.targetStateMachine = esm;
                        break;
                    default:
                        if (i < 2)
                        {
                            esmr[i] = esm;
                        }
                        i++;
                        break;
                }
            }

            esmr[2] = mach;
            hurtState.idleStateMachine = esmr;
        }

        private void SetCharacterBody()
        {
            characterBody.baseAcceleration = 70f;
            characterBody.baseArmor = 20; //Base armor this character has, set to 20 if this character is melee 
            characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
            characterBody.baseCrit = 1;  //Base crit, usually one
            characterBody.baseDamage = 12; //Base damage
            characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
            characterBody.baseJumpPower = 16; //Base jump power
            characterBody.baseMaxHealth = 150; //Base health, basically the health you have when you start a new run
            characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
            characterBody.baseMoveSpeed = 7; //Base move speed, this is usual 7
            characterBody.baseNameToken = "WYATT_BODY_NAME"; //The base name token. 
            R2API.LanguageAPI.Add("WYATT_BODY_LORE", lore);
            characterBody.subtitleNameToken = "WYATT_BODY_SUBTITLE"; //Set this if its a boss
            characterBody.baseRegen = 1.5f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes); ///Base body flags, should be self explanatory 
            characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
            characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
            characterBody.hullClassification = HullClassification.Human; //The hull classification, usually used for AI
            characterBody.isChampion = false; //Set this to true if its A. A boss or B. A miniboss
            characterBody.levelArmor = 0; //Armor gained when leveling up. 
            characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
            characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
            characterBody.levelDamage = 2.4f; //Damage gained when leveling up. 
            characterBody.levelArmor = 0; //Armor gained when leveling up. 
            characterBody.levelJumpPower = 0; //Jump power gained when leveling up. 
            characterBody.levelMaxHealth = 42; //Health gained when leveling up. 
            characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
            characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
            characterBody.levelRegen = 0.5f; //Regen gained when leveling up. 
            //credits to moffein for the icon
            //no 
            //credit
            //fuck you moffein
            characterBody.portraitIcon = AssetsCore.mainAssetBundle.LoadAsset<Texture>("WyattPortrait"); //The portrait icon, shows up in multiplayer and the death UI
            characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/SurvivorPod");
        }
        #endregion
        #region Skill Creation

        private void SetSkills()
        {
            CloudUtils.CreateEmptySkills(wyattBody);
            FixSkin();
            RegisterDamageTypes();
         //   CreatePassives();
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();

        }

        private void FixSkin()
        {
            GameObject model = wyattBody.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            SkinnedMeshRenderer FixRenderInfo()
            {
                SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer"); //Reflection.GetFieldValue<CharacterModel.RendererInfo>(modelController, "baseRenderInfos")
                if (mainRenderer == null)
                {
                    CharacterModel.RendererInfo[] info = Reflection.GetFieldValue<CharacterModel.RendererInfo[]>(characterModel, "baseRendererInfos");
                    if (info != null)
                    {
                        foreach (CharacterModel.RendererInfo rendererInfo in info)
                        {
                            if (rendererInfo.renderer is SkinnedMeshRenderer)
                            {
                                mainRenderer = (SkinnedMeshRenderer)rendererInfo.renderer;
                                break;
                            }
                        }
                        if (mainRenderer != null)
                        {
                            characterModel.SetFieldValue<SkinnedMeshRenderer>("mainSkinnedMeshRenderer", mainRenderer);
                        }
                    }
                }
                return mainRenderer;
            }

            void CreateSkinInfo(SkinnedMeshRenderer mainRenderer)
            {
                ModelSkinController skinController = model.AddOrGetComponent<ModelSkinController>();

                Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo defaultSkinInfo = default(Cloudburst.Content.ContentHandler.Loadouts.SkinDefInfo);
                defaultSkinInfo.BaseSkins = Array.Empty<SkinDef>();
                defaultSkinInfo.GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
                defaultSkinInfo.Icon = Cloudburst.Content.ContentHandler.Loadouts.CreateSkinIcon(new Color(0f, 156f / 255f, 188f / 255f), new Color(186f / 255f, 128f / 255f, 52f / 255f), new Color(58f / 255f, 49f / 255f, 24f / 255f), new Color(2f / 255f, 29f / 255f, 55f / 255f));

                defaultSkinInfo.MeshReplacements = new SkinDef.MeshReplacement[] {
                    new SkinDef.MeshReplacement {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                    }
                };
                defaultSkinInfo.MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>();
                defaultSkinInfo.Name = "DEFAULT_SKIN";
                defaultSkinInfo.NameToken = "DEFAULT_SKIN";
                defaultSkinInfo.ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>();
                defaultSkinInfo.RendererInfos = characterModel.baseRendererInfos;
                defaultSkinInfo.RootObject = model;
                defaultSkinInfo.UnlockableName = "";


                //, ));

                SkinDef defaultSkin = Cloudburst.Content.ContentHandler.Loadouts.CreateNewSkinDef(defaultSkinInfo);

                skinController.skins = new SkinDef[1]
                {
                defaultSkin,
                };
            }

            CreateSkinInfo(FixRenderInfo());
        }

        private void CreatePrimary()
        {

            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(FullSwing));

            SteppedSkillDef primarySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();

            primarySkillDef.activationState = new SerializableEntityStateType(typeof(FullSwing));
            primarySkillDef.stepCount = 3;
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 1;
            primarySkillDef.baseRechargeInterval = 0f;
            primarySkillDef.beginSkillCooldownOnSkillEnd = true;
            primarySkillDef.canceledFromSprinting = false;
            primarySkillDef.fullRestockOnAssign = true;
            primarySkillDef.interruptPriority = InterruptPriority.Any;

            primarySkillDef.isCombatSkill = true;
            primarySkillDef.mustKeyPress = false;
            primarySkillDef.cancelSprintingOnActivation = false;
            primarySkillDef.rechargeStock = 1;
            primarySkillDef.requiredStock = 1;

            primarySkillDef.stockToConsume = 0;

            primarySkillDef.icon = AssetsCore.wyattPrimary;
            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(primarySkillDef);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primarySkillDef,

                viewableNode = new ViewablesCatalog.Node(primarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateSecondary()
        {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(ChargeHomeRun));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(HomeRun));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(AnimationTest));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut2));
            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut3));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<HANDDroneSkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(TrashOut));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 2;
            secondarySkillDef.baseRechargeInterval = 3f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = false;
            secondarySkillDef.interruptPriority = InterruptPriority.Skill;

            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = true;
            secondarySkillDef.cancelSprintingOnActivation = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;

            secondarySkillDef.dontAllowPastMaxStocks = true;
            secondarySkillDef.stockToConsume = 1;
            secondarySkillDef.skillDescriptionToken = "WYATT_SECONDARY_DESCRIPTION";
            secondarySkillDef.skillName = "aaa";
            secondarySkillDef.skillNameToken = "WYATT_SECONDARY_NAME";
            secondarySkillDef.icon = AssetsCore.wyattSecondary;
            secondarySkillDef.keywordTokens = new string[] {
                // "KEYWORD_AGILE",
                // "KEYWORD_CHARGEABLE",
                "KEYWORD_SPIKED"
             };

            R2API.LanguageAPI.Add(secondarySkillDef.skillNameToken, "Trash Out");
            R2API.LanguageAPI.Add(secondarySkillDef.skillDescriptionToken, "Deploy a winch that reels you towards an enemy, and <style=cIsDamage>Spike</style> for <style=cIsDamage>X%</style>.");

            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(secondarySkillDef);
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondarySkillDef,

                viewableNode = new ViewablesCatalog.Node(secondarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateUtility()
        {
        }

        private void CreateSpecial()
        {
            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Drone));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(DeployMaid));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(RetrieveMaid));

            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(DeployMaid));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 3; ;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = true;
            specialSkillDef.interruptPriority = InterruptPriority.Any;

            specialSkillDef.isCombatSkill = false;
            specialSkillDef.mustKeyPress = true;
            specialSkillDef.cancelSprintingOnActivation = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;

            specialSkillDef.stockToConsume = 0;
            specialSkillDef.skillDescriptionToken = "WYATT_SPECIAL_DESCRIPTION";
            specialSkillDef.skillName = "aaa";
            specialSkillDef.skillNameToken = "WYATT_SPECIAL_NAME";
            specialSkillDef.icon = AssetsCore.wyattSpecial;
            specialSkillDef.keywordTokens = new string[] {
                 "KEYWORD_WEIGHTLESS"
            };

            throwPrimary = specialSkillDef;

            SkillDef specialSkillDef2 = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef2.activationState = new SerializableEntityStateType(typeof(RetrieveMaid));
            specialSkillDef2.activationStateMachineName = "Weapon";
            specialSkillDef2.baseMaxStock = 1;
            specialSkillDef2.baseRechargeInterval = 3;
            specialSkillDef2.beginSkillCooldownOnSkillEnd = true;
            specialSkillDef2.canceledFromSprinting = false;
            specialSkillDef2.fullRestockOnAssign = true;
            specialSkillDef2.interruptPriority = InterruptPriority.Any;
            specialSkillDef2.isCombatSkill = false;
            specialSkillDef2.mustKeyPress = true;
            specialSkillDef2.cancelSprintingOnActivation = false;
            specialSkillDef2.rechargeStock = 1;
            specialSkillDef2.requiredStock = 1;
            specialSkillDef2.stockToConsume = 1;
            specialSkillDef2.skillDescriptionToken = "WYATT_SPECIAL2_DESCRIPTION";
            specialSkillDef2.skillName = "aaa";
            specialSkillDef2.skillNameToken = "WYATT_SPECIAL2_NAME";
            specialSkillDef2.icon = AssetsCore.wyattSpecial2;
            specialSkillDef2.keywordTokens = new string[] {
                 "KEYWORD_WEIGHTLESS"
            };

            retrievePrimary = specialSkillDef2;

            R2API.LanguageAPI.Add(specialSkillDef.skillNameToken, "G22 MAID");
            R2API.LanguageAPI.Add(specialSkillDef.skillDescriptionToken, "Deploy a floating MAID unit that generates an anti-gravity bubble that <style=cIsUtility>pulls enemies</style> and <style=cIsUtility>applies Weightless</style> to all enemies, <style=cIsUtility>while giving Survivors free movement</style>.");

            R2API.LanguageAPI.Add(specialSkillDef2.skillNameToken, "Retrival");
            R2API.LanguageAPI.Add(specialSkillDef2.skillDescriptionToken, "Throw a winch towards the deployed MAID unit, bringing her back.");


            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(specialSkillDef);
            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(specialSkillDef2);
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;

            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,

                viewableNode = new ViewablesCatalog.Node(specialSkillDef.skillNameToken, false, null)
            };
        }
        #endregion
        #region Hooks
        private void Hook()
        {
            On.RoR2.CameraRigController.OnEnable += FixFadingInLobby;
        }


        private void FixFadingInLobby(On.RoR2.CameraRigController.orig_OnEnable orig, CameraRigController rig)
        {
            var def = SceneCatalog.GetSceneDefForCurrentScene();
            if (def && def.baseSceneName is "lobby") rig.enableFading = false;
            orig(rig);
        }

        #endregion
        #region DamageTypes
        private void RegisterDamageTypes()
        {
            //BonusToStunned = DamageTypeExtension.AddDamageType();
        }

        #endregion
        #region Misc
        private void CreateTokens()
        {
            R2API.LanguageAPI.Add("WYATT_BODY_NAME", "Custodian");
            R2API.LanguageAPI.Add("WYATT_BODY_SUBTITLE", "Lean, Mean, Cleaning Machine");

        }

    }
}
#endregion