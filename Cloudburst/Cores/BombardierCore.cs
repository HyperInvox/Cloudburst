﻿/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using RoR2.Skills;
using EntityStates;
using RoR2;
using Cloudburst.Cores.States.Bombardier;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.Skills;
using BepInEx;
using UnityEngine.Networking;
using KinematicCharacterController;


namespace Cloudburst.Cores
{
    class BombardierCore
    {
        public static BombardierCore instance;
        public BombardierCore() => CreateBombardier();

        protected internal GameObject characterPrefab;
        protected internal CharacterBody characterBody;
        protected internal SkillLocator skillLocator;
        protected void CreateBombardier() {
            instance = this;

            LogCore.LogI("Initializing Core: " + base.ToString());

            CreatePrefab();
            SetComponents();
            CreateSkills();
            FixSkin();
            AddToSurvivorSelect();
        }

        protected void FixSkin()
        {
            GameObject model = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
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
                defaultSkinInfo.Icon = Cloudburst.Content.ContentHandler.CreateSkinIcon(
                    //top - filled
                    new Color(250f / 255f, 234f / 255f, 202f / 255f),
                    //right 
                    new Color(53f / 255f, 55f / 255f, 72f / 255f),
                    //bottom - filled
                    new Color(28f / 255f, 131f / 255f, 148f / 255f),
                    //left - fillfed
                    new Color(106f / 255f, 108f / 255f, 130f / 255f));

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

        internal GameObject CreateModel(GameObject main)
        {
            BaseUnityPlugin.Destroy(main.transform.Find("ModelBase").gameObject);
            BaseUnityPlugin.Destroy(main.transform.Find("CameraPivot").gameObject);
            BaseUnityPlugin.Destroy(main.transform.Find("AimOrigin").gameObject);

            // make sure it's set up right in the unity project
            GameObject model = AssetsCore.bombAssetBundle.LoadAsset<GameObject>("mdlExampleSurvivor"); //Assets/Cloudburst/Survivors/Bombardier/Flan/ExecutionerModel.prefab"); //.prefab");
                
            return model;
        }

        internal  void CreatePrefab()
        {
            //https://twitter.com/UndyingPlace814/status/1314421118917316609


            // first clone the commando prefab so we can turn that into our own survivor
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "Bombardier", true);

            characterPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            /* create the model here, we're gonna replace commando's model with our own
            GameObject model = CreateModel(characterPrefab);

            GameObject gameObject = new GameObject("ModelBase");
            gameObject.transform.parent = characterPrefab.transform;
            gameObject.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject gameObject2 = new GameObject("CameraPivot");
            gameObject2.transform.parent = gameObject.transform;
            gameObject2.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;

            GameObject gameObject3 = new GameObject("AimOrigin");
            gameObject3.transform.parent = gameObject.transform;
            gameObject3.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            gameObject3.transform.localRotation = Quaternion.identity;
            gameObject3.transform.localScale = Vector3.one;
                
            Transform transform = model.transform;
            transform.parent = gameObject.transform;
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            transform.localRotation = Quaternion.identity;

            CharacterDirection characterDirection = characterPrefab.GetComponent<CharacterDirection>();
            characterDirection.moveVector = Vector3.zero;
            characterDirection.targetTransform = gameObject.transform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = model.GetComponentInChildren<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;

            // set up the character body here
            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();

            // the charactermotor controls the survivor's movement and stuff
            CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            characterMotor.walkSpeedPenaltyCoefficient = 1f;
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 100f;
            characterMotor.airControl = 0.25f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;
            characterMotor.useGravity = true;
            characterMotor.isFlying = false;

            InputBankTest inputBankTest = characterPrefab.GetComponent<InputBankTest>();
            inputBankTest.moveVector = Vector3.zero;

            CameraTargetParams cameraTargetParams = characterPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = null;
            cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;

            // this component is used to locate the character model(duh), important to set this up here
            ModelLocator modelLocator = characterPrefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = transform;
            modelLocator.modelBaseTransform = gameObject.transform;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.noCorpse = false;
            modelLocator.normalizeToFloor = false; // set true if you want your character to rotate on terrain like acrid does
            modelLocator.preserveModel = false;

            // childlocator is something that must be set up in the unity project, it's used to find any child objects for things like footsteps or muzzle flashes
            // also important to set up if you want quality
            //ChildLocator childLocator = model.GetComponent<ChildLocator>();

            // this component is used to handle all overlays and whatever on your character, without setting this up you won't get any cool effects like burning or freeze on the character
            // it goes on the model object of course
            CharacterModel characterModel = model.AddComponent<CharacterModel>();
            characterModel.body = bodyComponent;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                // set up multiple rendererinfos if needed, but for this example there's only the one
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = model.GetComponentInChildren<SkinnedMeshRenderer>().material,
                    renderer = model.GetComponentInChildren<SkinnedMeshRenderer>(),
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                }
            };

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            TeamComponent teamComponent = null;
            if (characterPrefab.GetComponent<TeamComponent>() != null) teamComponent = characterPrefab.GetComponent<TeamComponent>();
            else teamComponent = characterPrefab.GetComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = false;
            teamComponent.teamIndex = TeamIndex.None;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.health = 90f;
            healthComponent.shield = 0f;
            healthComponent.barrier = 0f;
            healthComponent.magnetiCharge = 0f;
            healthComponent.body = null;
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;

            characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
            characterPrefab.GetComponent<InteractionDriver>().highlightInteractor = true;

            // this disables ragdoll since the character's not set up for it, and instead plays a death animation
            CharacterDeathBehavior characterDeathBehavior = characterPrefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = characterPrefab.GetComponent<EntityStateMachine>();
            characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));

            // edit the sfxlocator if you want different sounds
            SfxLocator sfxLocator = characterPrefab.GetComponent<SfxLocator>();
            sfxLocator.deathSound = "Play_ui_player_death";
            sfxLocator.barkSound = "";
            sfxLocator.openSound = "";
            sfxLocator.landingSound = "Play_char_land";
            sfxLocator.fallDamageSound = "Play_char_land_fall_damage";
            sfxLocator.aliveLoopStart = "";
            sfxLocator.aliveLoopStop = "";

            Rigidbody rigidbody = characterPrefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.None;

            CapsuleCollider capsuleCollider = characterPrefab.GetComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = false;
            capsuleCollider.material = null;
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;

            KinematicCharacterMotor kinematicCharacterMotor = characterPrefab.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor.Rigidbody = rigidbody;

            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.center = new Vector3(0, 0, 0);
            capsuleCollider.material = null;

            kinematicCharacterMotor.DetectDiscreteCollisions = false;
            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.PreventSnappingOnLedges = false;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;
            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;
            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = Vector3.up;
            kinematicCharacterMotor.StepHandling = StepHandlingMethod.None;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;

            // this sets up the character's hurtbox, kinda confusing, but should be fine as long as it's set up in unity right
            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();

            HurtBox componentInChildren = model.GetComponentInChildren<CapsuleCollider>().gameObject.AddComponent<HurtBox>();
            componentInChildren.gameObject.layer = LayerIndex.entityPrecise.intVal;
            componentInChildren.healthComponent = healthComponent;
            componentInChildren.isBullseye = true;
            componentInChildren.damageModifier = HurtBox.DamageModifier.Normal;
            componentInChildren.hurtBoxGroup = hurtBoxGroup;
            componentInChildren.indexInGroup = 0;

            hurtBoxGroup.hurtBoxes = new HurtBox[]
            {
                componentInChildren
            };

            hurtBoxGroup.mainHurtBox = componentInChildren;
            hurtBoxGroup.bullseyeCount = 1;

            // this is for handling footsteps, not needed but polish is always good
            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");

            // ragdoll controller is a pain to set up so we won't be doing that here..
            RagdollController ragdollController = model.AddComponent<RagdollController>();
            ragdollController.bones = null;
            ragdollController.componentsToDisableOnRagdoll = null;

            // this handles the pitch and yaw animations, but honestly they are nasty and a huge pain to set up so i didn't bother
            AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBankTest;
            aimAnimator.directionComponent = characterDirection;
            aimAnimator.pitchRangeMax = 55f;
            aimAnimator.pitchRangeMin = -50f;
            aimAnimator.yawRangeMin = -44f;
            aimAnimator.yawRangeMax = 44f;
            aimAnimator.pitchGiveupRange = 30f;
            aimAnimator.yawGiveupRange = 10f;
            aimAnimator.giveupDuration = 8f;

            CloudUtils.RegisterNewBody(characterPrefab);
        }


        protected void AddToSurvivorSelect() {
            GameObject display = characterPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;

            string desc = "Bombardier is a master of unconventional warfare who can use his explosives to control the battle field<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Rocket Blast is weak against single targets, but excels in crowds. Blast into a large crowd of enemies for maximum destruction." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use your Sticky Charges to set up bosses for huge damage potential. Keep in mind they only detonate when you use your primary, so use as many as you have." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Concussive Blast is an excellent escape and mobility tool, but it also pulls enemies towards you when used. Jump away and shoot straight down to quickly dispose of a crowd" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > The key to success is realizing all of the Bombardier's abilities propel him into the air. Try to gain as much height as possible when using Grand Slam to annihilate your enemies." + Environment.NewLine + Environment.NewLine;

            R2API.LanguageAPI.Add("BOMBARDIER_BASENAMETOKEN", "Bomber");
            R2API.LanguageAPI.Add("BOMBARDIER_DESCRIPTION", desc);
            R2API.LanguageAPI.Add("BOMBARDIER_FLAVORTOKEN", "...and so he left, lighting the fuse for a new adventure.");

            SurvivorDef def = new SurvivorDef() {
                bodyPrefab = characterPrefab,
                descriptionToken = "BOMBARDIER_DESCRIPTION",
                displayNameToken = "BOMBARDIER_BASENAMETOKEN",
                //nigga stop smoking and work
                displayPrefab = display,
                name = "Bombardier",
                outroFlavorToken = "BOMBARDIER_FLAVORTOKEN",
                primaryColor = new Color(253f / 255f, 118f / 255f, 26f), /// 255), 
                unlockableName = ""
            };
            SurvivorAPI.AddSurvivor(def);
            
        }

        private void SetCharacterBody()
        {
            //On.RoR2.BulletAttack.ProcessHit += BulletAttack_ProcessHit;
            //characterBody.baseAcceleration = 70f;
            //characterBody.baseArmor = 20; //Base armor this character has, set to 20 if this character is melee 
            characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
            characterBody.baseCrit = 1;  //Base crit, usually one
            characterBody.baseDamage = 14; //Base damage
            characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
            characterBody.baseJumpPower = 16; //Base jump power
            characterBody.baseMaxHealth = 160; //Base health, basically the health you have when you start a new run
            characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
            characterBody.baseMoveSpeed = 7; //Base move speed, this is usual 7
            characterBody.baseNameToken = "BOMBARDIER_BODY_NAME"; //The base name token. 
            characterBody.subtitleNameToken = "BOMBARDIER_BODY_SUBTITLE"; //Set this if its a boss
            characterBody.baseRegen = 1f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.IgnoreFallDamage); ///Base body flags, should be self explanatory 
            characterBody.crosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/TiltedBracketCrosshair"); ; //The crosshair prefab.
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
            characterBody.levelRegen = 0.2f; //Regen gained when leveling up. 

            R2API.LanguageAPI.Add(characterBody.subtitleNameToken, "Stranded Fuse Lighter");
            R2API.LanguageAPI.Add(characterBody.baseNameToken, "Bomber");

            //fuck you moffein no credit
            //characterBody.portraitIcon = AssetsCore.mainAssetBundle.LoadAsset<Texture>("Assets/Cloudburst/Survivors/Bombardier/icon.png"); ; //The portrait icon, shows up in multiplayer and the death UI
            //characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod");
        }

        private bool BulletAttack_ProcessHit(On.RoR2.BulletAttack.orig_ProcessHit orig, BulletAttack self, ref BulletAttack.BulletHit hitInfo)
        {
            LogCore.LogI(hitInfo.collider.name);
            return orig(self, ref hitInfo);
        }

        private void CreateSkills()
        {
            CloudUtils.CreateEmptySkills(characterPrefab);

            skillLocator = characterPrefab.GetComponent<SkillLocator>();
            characterBody = characterPrefab.GetComponent<CharacterBody>();

            SetCharacterBody();

            characterPrefab.AddComponent<BombardierStickyBombManager>();

            skillLocator.passiveSkill.enabled = true;

            var passive = skillLocator.passiveSkill;
            passive.skillDescriptionToken = "BOMBARDIER_PASSIVE_DESC";
            passive.skillNameToken = "BOMBARDIER_PASSIVE_NAME";
            passive.icon = null;
            skillLocator.passiveSkill = passive;

            R2API.LanguageAPI.Add("BOMBARDIER_PASSIVE_DESC", "You are <style=cIsUtility>immune</style> to fall damage.");
            R2API.LanguageAPI.Add("BOMBARDIER_PASSIVE_NAME", "Lessons Learned");

            

            CreatePassive();
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();
        }

        private void CreatePassive() {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(BombardierMain));

            //var stateMachine = body.GetComponent<EntityStateMachine>();
            //stateMachine.mainStateType = new SerializableEntityStateType(typeof(BombardierMain));
        }

        private void SetComponents() {
            //var tracker = body.AddComponent<BombardierTracker>();
            //tracker.maxTrackingDistance = 150;
        }

        private void CreatePrimary()
        {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(FireRocket));

            SkillDef primarySkillDef = ScriptableObject.CreateInstance<SkillDef>();

            primarySkillDef.activationState = new SerializableEntityStateType(typeof(FireRocket));
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
            primarySkillDef.skillDescriptionToken = "BOMBARDIER_PRIMARY_DESC";
            primarySkillDef.skillName = "Rocket";
            primarySkillDef.skillNameToken = "BOMBARDIER_PRIMARY_NAME";
            primarySkillDef.icon = null;
            primarySkillDef.keywordTokens = Array.Empty<String>();

            R2API.LanguageAPI.Add(primarySkillDef.skillDescriptionToken, "Fire a rocket that does <style=cIsDamage>385%</style> damage and <style=cIsDamage>explodes on contact</style>.");
            R2API.LanguageAPI.Add(primarySkillDef.skillNameToken, "Rocket Blast");

           Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(primarySkillDef);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primarySkillDef,

                viewableNode = new ViewablesCatalog.Node(primarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateSecondary() {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(FireFlameRocket));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(ThrowSticky));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(ThrowStickySimple));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<BombardierStickySkillDef>();
            secondarySkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowStickySimple));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.skillName = "BOMBARDIER_SECONDARY_NAME";
            secondarySkillDef.skillNameToken = "BOMBARDIER_SECONDARY_NAME";
            secondarySkillDef.skillDescriptionToken = "BOMBARDIER_SECONDARY_DESCRIPTION";
            secondarySkillDef.icon = null;
            secondarySkillDef.baseMaxStock = 1;
            secondarySkillDef.baseRechargeInterval = 3f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = false;
            secondarySkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = true;
            secondarySkillDef.cancelSprintingOnActivation = true;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;
            secondarySkillDef.shootDelay = 0f;
            secondarySkillDef.stockToConsume = 1;

            /*SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(ThrowSticky));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 3;
            secondarySkillDef.baseRechargeInterval = 3f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = false;
            secondarySkillDef.interruptPriority = InterruptPriority.PrioritySkill;

            secondarySkillDef.isCombatSkill = false;
            secondarySkillDef.mustKeyPress = false;
            secondarySkillDef.cancelSprintingOnActivation = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;
            secondarySkillDef.shootDelay = 0.85f;
            secondarySkillDef.stockToConsume = 1;
            secondarySkillDef.skillDescriptionToken = "BOMBARDIER_SECONDARY_DESC";
            secondarySkillDef.skillName = "StunningRocket";
            secondarySkillDef.skillNameToken = "BOMBARDIER_SECONDARY_NAME";
            secondarySkillDef.icon = null;
            secondarySkillDef.keywordTokens = new string[] {
                 "KEYWORD_STUNNING",
             };

            R2API.LanguageAPI.Add(secondarySkillDef.skillDescriptionToken, "Toss a satchel charge that deals <style=cIsDamage>450% damage</style>. <style=cIsDamage>Detonate all deployed charges</style> <style=cIsUtility>by using while on cooldown.</style>");
            R2API.LanguageAPI.Add(secondarySkillDef.skillNameToken, "Satchel Charge");

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
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(UpwardsBlast));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(RocketJump));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(FireRocketInThrees));

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(RocketJump));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 4f;
            utilitySkillDef.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef.canceledFromSprinting = false;
            utilitySkillDef.fullRestockOnAssign = false;
            utilitySkillDef.interruptPriority = InterruptPriority.Skill;

            utilitySkillDef.isCombatSkill = true;
            utilitySkillDef.mustKeyPress = false;
            utilitySkillDef.cancelSprintingOnActivation = false;
            utilitySkillDef.rechargeStock = 1;
            utilitySkillDef.requiredStock = 1;

            utilitySkillDef.stockToConsume = 1;
            utilitySkillDef.skillDescriptionToken = "BOMBARDIER_UTILITY_DESC";
            utilitySkillDef.skillName = "aaa";
            utilitySkillDef.skillNameToken = "BOMBARDIER_UTILITY_NAME";
            utilitySkillDef.icon = null;
            utilitySkillDef.keywordTokens = new string[] {
                 "KEYWORD_WHIFF",
            };

            R2API.LanguageAPI.Add("KEYWORD_FORCEFUL", "<style=cKeywordName>Forceful</style><style=cSub>This skill applies the Forceful debuff, which doubles force applied to enemies.</style>");
            R2API.LanguageAPI.Add("KEYWORD_WHIFF", "<style=cKeywordName>Whiffing</style><style=cSub>This skill does 200% more damage when you are in mid-air.</style>");

            //R2API.LanguageAPI.Add(utilitySkillDef.skillDescriptionToken, "<style=cIsDamage>Forceful</style>. Pull enemies towards you for <style=cIsDamage>500%</style> damage and <style=cIsUtility>boost yourself upwards</style>.");
            //R2API.LanguageAPI.Add(utilitySkillDef.skillNameToken, "Plan B");

            R2API.LanguageAPI.Add(utilitySkillDef.skillDescriptionToken, "<style=cIsDamage>Stunning</style>. <style=cIsDamage>Whiffing</style>. Fire a powerful rocket that does <style=cIsDamage>500%</style> damage and <style=cIsDamage>explodes in a large radius on contact</style>.");
            R2API.LanguageAPI.Add(utilitySkillDef.skillNameToken, "Concussive Payload");

           Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(utilitySkillDef);
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,

                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)
            };
        }

        private void CreateSpecial() {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Mortar));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(GrandSlam));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(ThrowTesla));

            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(ThrowTesla));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 10;
            specialSkillDef.beginSkillCooldownOnSkillEnd = true;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = true;
            specialSkillDef.interruptPriority = InterruptPriority.Skill;

            specialSkillDef.isCombatSkill = true;
            specialSkillDef.mustKeyPress = true;
            specialSkillDef.cancelSprintingOnActivation = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;

            specialSkillDef.stockToConsume = 1;
            specialSkillDef.skillDescriptionToken = "BOMBARDIER_SPECIAL_DESC";
            specialSkillDef.skillName = "GrandSlam";
            specialSkillDef.keywordTokens = new string[] {  };
            specialSkillDef.skillNameToken = "BOMBARDIER_SPECIAL_NAME";
            specialSkillDef.icon = null;

            R2API.LanguageAPI.Add(specialSkillDef.skillDescriptionToken, "This content is filler.");
            R2API.LanguageAPI.Add(specialSkillDef.skillNameToken, "FILLER");

           Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(specialSkillDef);
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;

            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,

                viewableNode = new ViewablesCatalog.Node(specialSkillDef.skillNameToken, false, null)
            };
        }
    }
}
*/