using EntityStates;
using R2API;
using R2API.Utils;
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
            CreateWYATTPrefab();
            CreateSunderPrefab();
            //CreateWinchPrefab();
            SetComponents();
            SetSkills();
            CreateSurvivorDef();
            Hook();
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
                if (con.teamFilter && con.teamFilter.teamIndex == TeamIndex.Player) {
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
            /*ProjectileProximityBeamController beamController = winch.AddComponent<ProjectileProximityBeamController>();

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

            API.RegisterNewProjectile(this.winch);*/
            #region Old
            #endregion
        }
        private void CreateWYATTPrefab()
        {
            if (!wyattBody)
            {
                PrefabBuilder builder = new PrefabBuilder();
                builder.prefabName = "WyattBody";
                builder.masteryAchievementUnlockable = AchievementCore.GetUnlockableString("WyattMastery");
                builder.model = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlWyatt");
                builder.defaultSkinIcon = LoadoutAPI.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));
                builder.masterySkinIcon = LoadoutAPI.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));
                builder.masterySkinDelegate = material;
                builder.GetAdditionalRenderInfos += Builder_getAdditionalEntries; ;
                builder.GetAdditionalItemDisplays += Builder_GetAdditionalItemDisplays;
                builder.GetAdditionalEquipmentDisplays += Builder_GetAdditionalEquipmentDisplays;

                //On.RoR2.BuffWard.BuffTeam += BuffWard_BuffTeam;

                wyattBody = builder.CreatePrefab();
                Material material()
                {
                    return Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

                }
                //this code will break if we change the name of the broom model
                void Builder_getAdditionalEntries(List<CharacterModel.RendererInfo> arg1, Transform arg2)
                {
                    var broom = arg2.Find("Brom");
                    var mat = broom.GetComponentInChildren<SkinnedMeshRenderer>();
                    arg1.Add(new CharacterModel.RendererInfo {
                        defaultMaterial = mat.material,
                        defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                        ignoreOverlays = false,
                        renderer = mat,
                    });
                }
            }
        }

        private void BuffWard_BuffTeam(On.RoR2.BuffWard.orig_BuffTeam orig, BuffWard self, IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            orig(self, recipients, radiusSqr, currentPosition);
            var maid = self.GetComponent<MAID>();
            foreach (TeamComponent teamComponent in recipients)
            {
                CharacterBody component = teamComponent.GetComponent<CharacterBody>();
                bool isInRadius = (teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr;
                if (component) {
                    if (isInRadius && teamComponent.teamIndex != TeamIndex.Player && component.HasBuff(BuffCore.instance.antiGravIndex))
                    {
                        LogCore.LogI("entry");
                        if (component && component.characterMotor)
                        {
                            LogCore.LogI("entry1");
                            maid.OnEntry(component.characterMotor, null);
                        }
                        if (component && component.rigidbody && !component.characterMotor)
                        {
                            maid.OnEntry(null, component.rigidbody);
                        }
                        // self.GetComponent<MAID>().OnEntry(null, component.rigidbody);
                    }
                else if (!component.HasBuff(BuffCore.instance.antiGravIndex) && !isInRadius) {
                        if (component.characterMotor)
                        {
                            maid.characterMotors.Remove(component.characterMotor);
                        }
                        if (component.rigidbody && !component.characterMotor)
                        {
                            maid.rigidBodies.Remove(component.rigidbody);
                        }
                    }
                }
            }
        }

        private void Builder_GetAdditionalEquipmentDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
        {
            //LogCore.LogI("SETUP DISPLAY");
            obj.Add(CloudUtils.CreateGenericDisplayRule("CritOnUse", "DisplayNeuralImplant", "Head_end", new Vector3(-0.008f, 0.006f, 0), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
        }

        private void Builder_GetAdditionalItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
        {

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
            //LoadoutAPI.AddSkill(typeof(DeathState));
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
            characterBody.subtitleNameToken = "WYATT_BODY_SUBTITLE"; //Set this if its a boss
            characterBody.baseRegen = 1.5f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical); ///Base body flags, should be self explanatory 
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
            CreatePassives();
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();

        }

        private void FixSkin()
        {
            /*GameObject model = wyattBody.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
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

                LoadoutAPI.SkinDefInfo defaultSkinInfo = default(LoadoutAPI.SkinDefInfo);
                defaultSkinInfo.BaseSkins = Array.Empty<SkinDef>();
                defaultSkinInfo.GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
                defaultSkinInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0f, 156f / 255f, 188f / 255f), new Color(186f / 255f, 128f / 255f, 52f / 255f), new Color(58f / 255f, 49f / 255f, 24f / 255f), new Color(2f / 255f, 29f / 255f, 55f / 255f));

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

                SkinDef defaultSkin = LoadoutAPI.CreateNewSkinDef(defaultSkinInfo);

                skinController.skins = new SkinDef[1]
                {
                defaultSkin,
                };
            }

            CreateSkinInfo(FixRenderInfo());*/
        }


        private void CreatePassives()
        {
            var passive = skillLocator.passiveSkill;

            passive.enabled = true;
            passive.skillNameToken = "WYATT_PASSIVE_NAME";
            passive.skillDescriptionToken = "WYATT_PASSIVE_DESCRIPTION";
            passive.keywordToken = "KEYWORD_VELOCITY";
            passive.icon = AssetsCore.wyattPassive;

            LanguageAPI.Add(passive.skillNameToken, "Walkman");
            LanguageAPI.Add(passive.skillDescriptionToken, "On hit, gain a stack of <style=cIsUtility>Velocity</style>, up to 10. <style=cIsHealth>Lose two stacks every two seconds</style>");

            skillLocator.passiveSkill = passive;

            LanguageAPI.Add(passive.keywordToken, "<style=cKeywordName>Velocity</style><style=cSub>Increases movement speed by X% and health regeneration by X; all stacks lost when out of combat.</style>");

            /*var passiveFamily = ScriptableObject.CreateInstance<SkillFamily>();

            var skillSlot = wyattBody.AddComponent<GenericSkill>();

            var armorDef = ScriptableObject.CreateInstance<SkillDef>();

            armorDef.activationState = new SerializableEntityStateType(typeof(Uninitialized));
            armorDef.activationStateMachineName = "Weapon";
            armorDef.baseMaxStock = 1;
            armorDef.baseRechargeInterval = 0f;
            armorDef.beginSkillCooldownOnSkillEnd = true;
            armorDef.canceledFromSprinting = false;
            armorDef.fullRestockOnAssign = true;
            armorDef.interruptPriority = InterruptPriority.Any;
            armorDef.isBullets = false;
            armorDef.isCombatSkill = true;
            armorDef.mustKeyPress = false;
            armorDef.noSprint = false;
            armorDef.rechargeStock = 1;
            armorDef.requiredStock = 1;
            armorDef.shootDelay = 0.1f;
            armorDef.stockToConsume = 0;
            armorDef.skillDescriptionToken = "FILLMEOUTDADDY";
            armorDef.skillName = "Armor";
            armorDef.skillNameToken = "UWU";
            armorDef.icon = AssetsCore.overclockIcon;
            armorDef.keywordTokens = Array.Empty<string>();
            armorDef.skillDescriptionToken = "OWO WHAT's";
            armorDef.icon = AssetsCore.overclockIcon;
            armorDef.skillNameToken = "WYATT_PASSIVE_NAME";

            passiveFamily.variants = new SkillFamily.Variant[1] {
                new SkillFamily.Variant {
                    skillDef = armorDef,
                    unlockableName = ""
                },
            };

            LoadoutAPI.AddSkillDef(armorDef);
            LoadoutAPI.AddSkillFamily(passiveFamily);

            skillSlot.SetFieldValue("_skillFamily", passiveFamily);

            passiveContoller.armorDef = armorDef;
            passiveContoller.regenDef = regenDef;
            passiveContoller.speedDef = speedDef;
            passiveContoller.passiveSkillSlot = skillSlot;*/
        }

        private void CreatePrimary()
        {
            //--FIXED--
            //Alright, here's the problem:
            //Interrupt priorities are weird, and because they are weird
            //they cause the primary to not fire off after the secondary is fired.
            //Obvious problem is obvious, but setting the primary's interrupt priority to PrioritySkill 
            //makes it so you can't hold it down. Extremely obvious problem is obvious, but I have no idea how's to fix this.
            //FIX: Apparently I forgot to call base.FixedUpdate(n.k); in its FixedUpdate void, whoops!

            LoadoutAPI.AddSkill(typeof(FullSwing));
            LoadoutAPI.AddSkill(typeof(FullSwing2));
            LoadoutAPI.AddSkill(typeof(Cleanup));
            LoadoutAPI.AddSkill(typeof(WyattBaseMeleeAttack));

            SteppedSkillDef primarySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();

            primarySkillDef.activationState = new SerializableEntityStateType(typeof(WyattBaseMeleeAttack));
            primarySkillDef.stepCount = 3;
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 1;
            primarySkillDef.baseRechargeInterval = 0f;
            primarySkillDef.beginSkillCooldownOnSkillEnd = true;
            primarySkillDef.canceledFromSprinting = false;
            primarySkillDef.fullRestockOnAssign = true;
            primarySkillDef.interruptPriority = InterruptPriority.Any;
            primarySkillDef.isBullets = false;
            primarySkillDef.isCombatSkill = true;
            primarySkillDef.mustKeyPress = false;
            primarySkillDef.noSprint = false;
            primarySkillDef.rechargeStock = 1;
            primarySkillDef.requiredStock = 1;
            primarySkillDef.shootDelay = 0.1f;
            primarySkillDef.stockToConsume = 0;
            primarySkillDef.skillDescriptionToken = "WYATT_PRIMARY_DESCRIPTION";
            primarySkillDef.skillName = "WYATT_PRIMARY_NAME";
            primarySkillDef.skillNameToken = "WYATT_PRIMARY_NAME";
            primarySkillDef.icon = AssetsCore.wyattPrimary;
            primarySkillDef.keywordTokens = new string[] {
                 "KEYWORD_AGILE",
                 "KEYWORD_WEIGHTLESS",
                 "KEYWORD_SPIKED",
            };

            LanguageAPI.Add(primarySkillDef.skillNameToken, "G22 Grav-Broom");
            LanguageAPI.Add(primarySkillDef.skillDescriptionToken, "<style=cIsUtility>Agile</style>. Swing in front for X% damage. [NOT IMPLEMENTED] Every 4th hit <style=cIsDamage>Spikes</style>.");
            //LanguageAPI.Add(primarySkillDef.keywordTokens[1], "<style=cKeywordName>Weightless</style><style=cSub>Slows and removes gravity from target.</style>");
            LanguageAPI.Add(primarySkillDef.keywordTokens[2], "<style=cKeywordName>Spikes</style><style=cSub>Knocks an enemy directly toward the ground at dangerous speeds.</style>");

            LoadoutAPI.AddSkillDef(primarySkillDef);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primarySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateSecondary()
        {
            LoadoutAPI.AddSkill(typeof(ChargeHomeRun));
            LoadoutAPI.AddSkill(typeof(HomeRun));
            LoadoutAPI.AddSkill(typeof(AnimationTest));
            LoadoutAPI.AddSkill(typeof(TrashOut));
            LoadoutAPI.AddSkill(typeof(TrashOut2));
            //LoadoutAPI.AddSkill(typeof(TrashOut3));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<HANDDroneSkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(TrashOut));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 2;
            secondarySkillDef.baseRechargeInterval = 3f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = false;
            secondarySkillDef.interruptPriority = InterruptPriority.Skill;
            secondarySkillDef.isBullets = false;
            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = true;
            secondarySkillDef.noSprint = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;
            secondarySkillDef.shootDelay = 0.08f;
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

            LanguageAPI.Add(secondarySkillDef.skillNameToken, "Trash Out");
            LanguageAPI.Add(secondarySkillDef.skillDescriptionToken, "Deploy a winch that reels you towards an enemy, and <style=cIsDamage>Spike</style> for <style=cIsDamage>X%</style>.");

            LoadoutAPI.AddSkillDef(secondarySkillDef);
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondarySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(secondarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateUtility()
        {
            LoadoutAPI.AddSkill(typeof(Winch));
            LoadoutAPI.AddSkill(typeof(FireWinch));
            LoadoutAPI.AddSkill(typeof(DeepClean));
            LoadoutAPI.AddSkill(typeof(DeeperClean));
            LoadoutAPI.AddSkill(typeof(SS2Dies));

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<IsNotroundedSkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(GrandSlam));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 5f;
            utilitySkillDef.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef.canceledFromSprinting = false;
            utilitySkillDef.fullRestockOnAssign = false;
            utilitySkillDef.interruptPriority = InterruptPriority.Skill;
            utilitySkillDef.isBullets = false;
            utilitySkillDef.isCombatSkill = true;
            utilitySkillDef.mustKeyPress = false;
            utilitySkillDef.noSprint = false;
            utilitySkillDef.rechargeStock = 1;
            utilitySkillDef.requiredStock = 1;
            utilitySkillDef.shootDelay = 0.08f;
            utilitySkillDef.stockToConsume = 1;
            utilitySkillDef.skillDescriptionToken = "WYATT_UTILITY_DESCRIPTION";
            utilitySkillDef.skillName = "aaa";
            utilitySkillDef.skillNameToken = "WYATT_UTILITY_NAME";
            utilitySkillDef.icon = AssetsCore.wyattUtility;
            utilitySkillDef.keywordTokens = new string[] {
                 "KEYWORD_RUPTURE",
             };

            SkillDef utilitySkillDef2 = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef2.activationState = new SerializableEntityStateType(typeof(FireWinch));
            utilitySkillDef2.activationStateMachineName = "Weapon";
            utilitySkillDef2.baseMaxStock = 1;
            utilitySkillDef2.baseRechargeInterval = 4f;
            utilitySkillDef2.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef2.canceledFromSprinting = false;
            utilitySkillDef2.fullRestockOnAssign = false;
            utilitySkillDef2.interruptPriority = InterruptPriority.Skill;
            utilitySkillDef2.isBullets = false;
            utilitySkillDef2.isCombatSkill = true;
            utilitySkillDef2.mustKeyPress = false;
            utilitySkillDef2.noSprint = false;
            utilitySkillDef2.rechargeStock = 1;
            utilitySkillDef2.requiredStock = 1;
            utilitySkillDef2.shootDelay = 0.08f;
            utilitySkillDef2.stockToConsume = 1;
            utilitySkillDef2.skillDescriptionToken = "WYATT_UTILITY2_DESCRIPTION";
            utilitySkillDef2.skillName = "aaa";
            utilitySkillDef2.skillNameToken = "WYATT_UTILITY2_NAME";
            utilitySkillDef2.icon = AssetsCore.wyattUtilityAlt;
            utilitySkillDef2.keywordTokens = Array.Empty<string>();

            LanguageAPI.Add(utilitySkillDef.skillNameToken, "Rub and Scrub");
            LanguageAPI.Add(utilitySkillDef.skillDescriptionToken, "Dive downwards, <style=cIsDamage>dealing 100% damage</style>. Enemies directly below you are bounced on, <style=cIsUtility>reducing 3 seconds off the next cast</style> and <style=cIsDamage>taking 700% damage that is Sparkling</style>.");
            LanguageAPI.Add("KEYWORD_RUPTURE", "<style=cKeywordName>Sparkling</style><style=cSub>This damage deals up to <style=cIsDamage>3x</style> damage to low health enemies.</style>");


            LanguageAPI.Add(utilitySkillDef2.skillNameToken, "G22 WINCH");
            LanguageAPI.Add(utilitySkillDef2.skillDescriptionToken, "Fire a winch that deals <style=cIsDamage>500%</style> damage and <style=cIsUtility>pulls you</style> towards the target.");

            LoadoutAPI.AddSkillDef(utilitySkillDef);
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)
            };

            /*SkillFamily.Variant variant = new SkillFamily.Variant();

            variant.skillDef = utilitySkillDef2;
            variant.unlockableName = "";

            int prevLength = utilitySkillFamily.variants.Length;
            Array.Resize<SkillFamily.Variant>(ref utilitySkillFamily.variants, prevLength + 1);
            utilitySkillFamily.variants[prevLength] = variant;*/
        }

        private void CreateSpecial()
        {
            //LoadoutAPI.AddSkill(typeof(Drone));
            LoadoutAPI.AddSkill(typeof(DeployMaid));
            LoadoutAPI.AddSkill(typeof(RetrieveMaid));

            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(DeployMaid));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 3;
            specialSkillDef.beginSkillCooldownOnSkillEnd = true;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = true;
            specialSkillDef.interruptPriority = InterruptPriority.Any;
            specialSkillDef.isBullets = false;
            specialSkillDef.isCombatSkill = false;
            specialSkillDef.mustKeyPress = true;
            specialSkillDef.noSprint = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;
            specialSkillDef.shootDelay = 0.5f;
            specialSkillDef.stockToConsume = 1;
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
            specialSkillDef2.isBullets = false;
            specialSkillDef2.isCombatSkill = false;
            specialSkillDef2.mustKeyPress = true;
            specialSkillDef2.noSprint = false;
            specialSkillDef2.rechargeStock = 1;
            specialSkillDef2.requiredStock = 1;
            specialSkillDef2.shootDelay = 0.5f;
            specialSkillDef2.stockToConsume = 1;
            specialSkillDef2.skillDescriptionToken = "WYATT_SPECIAL2_DESCRIPTION";
            specialSkillDef2.skillName = "aaa";
            specialSkillDef2.skillNameToken = "WYATT_SPECIAL2_NAME";
            specialSkillDef2.icon = AssetsCore.wyattSpecial2;
            specialSkillDef2.keywordTokens = new string[] {
                 "KEYWORD_WEIGHTLESS"
            };

            retrievePrimary = specialSkillDef2;

            LanguageAPI.Add(specialSkillDef.skillNameToken, "G22 MAID");
            LanguageAPI.Add(specialSkillDef.skillDescriptionToken, "Deploy a floating MAID unit that generates an anti-gravity bubble that <style=cIsUtility>pulls enemies</style> and <style=cIsUtility>applies Weightless</style> to all enemies, <style=cIsUtility>while giving Survivors free movement</style>.");

            LanguageAPI.Add(specialSkillDef2.skillNameToken, "Retrival");
            LanguageAPI.Add(specialSkillDef2.skillDescriptionToken, "Throw a winch towards the deployed MAID unit, bringing her back.");


            LoadoutAPI.AddSkillDef(specialSkillDef);
            LoadoutAPI.AddSkillDef(specialSkillDef2);
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;

            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,
                unlockableName = "",
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
            LanguageAPI.Add("WYATT_BODY_NAME", "Custodian");
            LanguageAPI.Add("WYATT_BODY_SUBTITLE", "Lean, Mean, Cleaning Machine");

        }

        private void CreateSurvivorDef()
        {
            GameObject WYATTDisplay = this.wyattBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            string desc = "AYO HOL' UP <color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *SMACKS LIPS*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > SO WHAT YOU BE SAYIN' IS" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *CANCELS MOONFALL*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > HOL' UP" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *DIVERTS ALL RESOURCES TO STARSTORM 2*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > SO WHAT YOU BE SAYIN' IS....." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *NO CODERS ON MOONFALL*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > AYO" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *DEATH*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > FINNA REALLY" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *NEVER RELEASES*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > WE GONNA FINNA" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *NO WORK ON MOONFALL*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > SO WHATCHOO SAYIN' IS" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *FINISHES NOTHING*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > WE GONNA" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *ALMOST CANCELS MOD OVER GENO NOT GETTING INTO SMASH*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > FINNA" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *NAMEDROPS*" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > MAKE A MOD? SHIEEEEEEEEETTTTTTTTT" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > *DIES*" + Environment.NewLine + Environment.NewLine;

            LanguageAPI.Add("WYATT_DESCRIPTION", desc);
            LanguageAPI.Add("WYATT_OUTRO_FLAVOR", "...and so he left, a job well done.");

            SurvivorDef def = new SurvivorDef()
            {
                bodyPrefab = this.wyattBody,
                descriptionToken = "WYATT_DESCRIPTION",
                displayNameToken = "WYATT_BODY_NAME",
                displayPrefab = WYATTDisplay,
                name = "AAAAAAAAA",
                outroFlavorToken = "WYATT_OUTRO_FLAVOR",
                primaryColor = new Color(0.4862745098f, 0.94901960784f, 0.71764705882f),
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(def);
        }
        #endregion
    }
}
