using Cloudburst.Cores.Components;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using static R2API.LanguageAPI;
using static RoR2.CharacterAI.AISkillDriver;

namespace Cloudburst.Cores
{
    class VoidCore
    {
        public VoidCore instance;
        /// <summary>
        /// https://cdn.discordapp.com/attachments/739717122229403740/804910319029321728/IMG_20210129_112137.jpg
        /// </summary>
        internal class VoidGolems : EnemyCreator {

            protected override string resourceMasterPath => "prefabs/charactermasters/LunarGolemMaster";

            protected override string resourceBodyPath => "prefabs/characterbodies/LunarGolemBody";

            protected override string bodyName => "VoidGolem";

            protected override bool registerNetwork => true;

            public override void Create()
            {
                base.Create();
                CloudburstPlugin.start += CloudburstPlugin_StartVoid;
            }

            public override void OverrideMasterAI(CharacterMaster master)
            {
                base.OverrideMasterAI(master);
                AISkillDriver shell = master.AddComponent<AISkillDriver>();

                foreach (AISkillDriver driver in master.GetComponents<AISkillDriver>()) {
                    if (driver.customName == "StrafeAndShoot") {
                        driver.nextHighPriorityOverride = shell ;
                        //driver.skillSlot = SkillSlot.Secondary;
                    }
                }

                shell.customName = "Engage Shell Defenses";
                shell.skillSlot = SkillSlot.Secondary;
                shell.requireSkillReady = false;
                shell.requireEquipmentReady = false;
                shell.moveTargetType = TargetType.CurrentEnemy;
                shell.minUserHealthFraction = float.NegativeInfinity;
                shell.maxUserHealthFraction = float.PositiveInfinity;
                shell.minTargetHealthFraction = float.NegativeInfinity;
                shell.maxTargetHealthFraction = float.PositiveInfinity;
                shell.minDistance = 0;
                shell.maxDistance = 65;
                shell.selectionRequiresTargetLoS = false;
                shell.activationRequiresTargetLoS = false;
                shell.activationRequiresAimConfirmation = false;
                shell.movementType = MovementType.FleeMoveTarget;
                shell.moveInputScale = 1;
                shell.aimType = AimType.AtCurrentEnemy;
                shell.ignoreNodeGraph = true;
                shell.driverUpdateTimerOverride = -1;
                shell.resetCurrentEnemyOnNextDriverSelection = false;
                shell.noRepeat = false;
                shell.shouldSprint = false;
                shell.shouldFireEquipment = false;
                shell.buttonPressType = ButtonPressType.Hold;

            }

            private void CloudburstPlugin_StartVoid()
            {
                OverrideVisuals(enemyBody.GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>());
            }

            public override void OverrideVisuals(CharacterModel mdl)
            {
                base.OverrideVisuals(mdl);
                mdl.baseRendererInfos[0].defaultMaterial = voidMat;
            }

            public override void OverrideCharacterbody(CharacterBody body)
            {
                base.OverrideCharacterbody(body);

            }

            public override void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreatePrimary(skillLocator, skillFamily);

                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyTwinShot");

                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                LoadoutAPI.AddSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateSecondary(skillLocator, skillFamily);


                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyShield");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                def.activationState = new EntityStates.SerializableEntityStateType(typeof(States.VoidGolems.EngageShell));

                LoadoutAPI.AddSkillDef(def);
                LoadoutAPI.AddSkill(typeof(States.VoidGolems.EngageShell));

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateUtility(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyShield");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                LoadoutAPI.AddSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateSpecial(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyShield");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                LoadoutAPI.AddSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }
            /*public override void CreatePrimary(SkillLocator skillLocator)
            {
                base.CreatePrimary(skillLocator);

            }

            public override void CreateSecondary(SkillLocator skillLocator)
            {
                base.CreateSecondary(skillLocator); 
            }*/
        }

        public VoidCore() => FindGod();

        public static bool spawnInfection;
        public static bool triggeredTrueInfection;

        public static Material voidMat;

        private GameObject infectionTrigger;
        private GameObject floatingPlatform;
        private GameObject floatingPlatform2;
        private float skyLeap;

        private VoidGolems golems = null;

        private void FindGod()
        {
            instance = this;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            ArenaMissionController.onBeatArena += ArenaMissionController_onBeatArena;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            //On.EntityStates.Missions.Arena.NullWard.Active.FixedUpdate += Active_FixedUpdate;
            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += WeaponSlam_OnEnter;
            On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += HoldSkyLeap_OnEnter;
            CloudburstPlugin.start += CloudburstPlugin_start;

            golems = new VoidGolems();
            golems.Create();

            LanguageAPI.Add("TRIGGER_INFECTION", "A foul infection creeps into the exchange between time...");

            FindAssets();

            VoidGlass();
        }

        private void HoldSkyLeap_OnEnter(On.EntityStates.BrotherMonster.HoldSkyLeap.orig_OnEnter orig, EntityStates.BrotherMonster.HoldSkyLeap self)
        {
            orig(self);
            if (triggeredTrueInfection)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.searchOrigin = self.transform.position;
                bullseyeSearch.searchDirection = Vector3.forward;
                bullseyeSearch.maxDistanceFilter = 2000;
                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(self.gameObject));
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                bullseyeSearch.RefreshCandidates();
                HurtBox target = bullseyeSearch.GetResults().FirstOrDefault();

                self.characterMotor.Motor.SetPositionAndRotation(target.transform.position, Quaternion.identity, true);
            }
        }

        private void CloudburstPlugin_start()
        {
            skyLeap = EntityStates.BrotherMonster.HoldSkyLeap.duration;
        }

        public void VoidGlass() {
            var mdl = Resources.Load<GameObject>("prefabs/characterbodies/BrotherGlassBody").GetComponent<CharacterBody>();
            Add("GLASS_BODY_NAME", "Shattered Clone");
            mdl.baseNameToken = "GLASS_BODY_NAME";

        }

        private void WeaponSlam_OnEnter(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, EntityStates.BrotherMonster.WeaponSlam self)
        {
            orig(self);
            if (triggeredTrueInfection)
            {
                self.weaponAttack.damageType = DamageType.Nullify;
            }
        }

        private void Run_onRunStartGlobal(Run obj)
        {

            if (triggeredTrueInfection)
            {
                UndoMithrixReplacementLines();
                EntityStates.BrotherMonster.HoldSkyLeap.duration = skyLeap;
            }

            spawnInfection = false;
            triggeredTrueInfection = false;
        }

        private void UndoMithrixReplacementLines()
        {
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_1", "Pray.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_2", "Beg.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_3", "Die.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_4", "Be slaughtered.");

            LanguageAPI.Add("BROTHER_KILL_1", "Return to dirt.");
            LanguageAPI.Add("BROTHER_KILL_2", "Submit, vermin.");
            LanguageAPI.Add("BROTHER_KILL_3", "Die, vermin.");
            LanguageAPI.Add("BROTHER_KILL_4", "Die, weakling.");
            LanguageAPI.Add("BROTHER_KILL_5", "Become memories.");



            LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "Bleed.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_2", "Now is the time for fear.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_3", "Weak.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_4", "Frail - and soft.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_5", "You are nothing.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_6", "Mistake.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_7", "Scream, vermin.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_8", "Break beneath me.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "Slow.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_10", "Your body will shatter.");

            LanguageAPI.Add("BROTHER_KILL_1", "Return to dirt.");
            LanguageAPI.Add("BROTHER_KILL_2", "Submit, vermin.");
            LanguageAPI.Add("BROTHER_KILL_3", "Die, vermin.");
            LanguageAPI.Add("BROTHER_KILL_4", "Die, weakling.");
            LanguageAPI.Add("BROTHER_KILL_5", "Become memories.");

            LanguageAPI.Add("BROTHER_DEATH_1", "NO... NOT NOW...");
            LanguageAPI.Add("BROTHER_DEATH_2", "WHY... WHY NOW...?");
            LanguageAPI.Add("BROTHER_DEATH_3", "NO... NO...!");
            LanguageAPI.Add("BROTHER_DEATH_4", "BROTHER... HELP ME...!");
            LanguageAPI.Add("BROTHER_DEATH_5", "THIS PLANE GROWS DARK... BROTHER... I CANNOT SEE YOU... WHERE ARE YOU...?");
            LanguageAPI.Add("BROTHER_DEATH_6", "BROTHER... PERHAPS... WE WILL GET IT RIGHT... NEXT TIME...");

            Language.CCLanguageReload(new ConCommandArgs());
        }

        private void CreateInfection(GameObject re)
        {
            GameObject obj = re;

            var identity = obj.AddComponent<NetworkIdentity>();
            var sate = obj.AddComponent<NetworkTransform>();

            var filter = obj.AddComponent<TeamFilter>();
            var rigid = obj.AddComponent<Rigidbody>();
            var locator = obj.AddComponent<SkillLocator>();
            var state = obj.AddComponent<EntityStateMachine>();
            var network = obj.AddComponent<NetworkStateMachine>();
            var hb = obj.AddComponent<HurtBox>();
            var hurtbox = obj.AddComponent<HurtBoxGroup>();
            var characterBody = obj.AddComponent<CharacterBody>();
            var health = obj.AddComponent<HealthComponent>();
            var mdlLoc = obj.AddComponent<ModelLocator>();

            state.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            state.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

            network.stateMachines = new EntityStateMachine[] { state
                };

            rigid.useGravity = false;

            hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

            hb.healthComponent = health;
            hb.isBullseye = true;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            hurtbox.hurtBoxes = new HurtBox[] { hb };
            hurtbox.mainHurtBox = hb;
            hurtbox.bullseyeCount = 1;

            filter.teamIndex = TeamIndex.Monster;

            characterBody.baseAcceleration = 0;
            characterBody.baseArmor = 0; //Base armor this character has, set to 20 if this character is melee 
            characterBody.baseAttackSpeed = 0; //Base attack speed, usually 1
            characterBody.baseCrit = 0;  //Base crit, usually one
            characterBody.baseDamage = 0; //Base damage
            characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
            characterBody.baseJumpPower = 0; //Base jump power
            characterBody.baseMaxHealth = 500; //Base health, basically the health you have when you start a new run
            characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
            characterBody.baseMoveSpeed = 0; //Base move speed, this is usual 7
            characterBody.baseNameToken = ""; //The base name token. 
            characterBody.subtitleNameToken = ""; //Set this if its a boss
            characterBody.baseRegen = 0f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Masterless); ///Base body flags, should be self explanatory 
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
            characterBody.levelMaxHealth = 100; //Health gained when leveling up. 
            characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
            characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
            characterBody.levelRegen = 0; //Regen gained when leveling up. 

            health.health = 1000;
            health.globalDeathEventChanceCoefficient = 1;
            health.body = characterBody;

            mdlLoc.modelTransform = obj.transform;
            mdlLoc.dontReleaseModelOnDeath = true;
            mdlLoc.dontDetatchFromParent = true;
            mdlLoc.noCorpse = true;

            obj.AddComponent<InfestationTrigger>();

            voidMat = obj.GetComponent<Renderer>().material;

            PrefabAPI.RegisterNetworkPrefab(obj);

            infectionTrigger = obj;
        }

        private void CreateInfectionBubble(GameObject re)
        {
            GameObject obj = re;

            var identity = obj.AddComponent<NetworkIdentity>();
            var sate = obj.AddComponent<NetworkTransform>();

            var projectileController = obj.AddComponent<ProjectileController>();
            var projectileNetwork = obj.AddComponent<ProjectileNetworkTransform>();

            var filter = obj.AddComponent<TeamFilter>();
            var lol = obj.AddComponent<AssignTeamFilterToTeamComponent>();
            var rigid = obj.AddComponent<Rigidbody>();
            var locator = obj.AddComponent<SkillLocator>();
            var state = obj.AddComponent<EntityStateMachine>();
            var network = obj.AddComponent<NetworkStateMachine>();
            var hb = obj.AddComponent<HurtBox>();
            var hurtbox = obj.AddComponent<HurtBoxGroup>();
            var characterBody = obj.AddComponent<CharacterBody>();
            var health = obj.AddComponent<HealthComponent>();
            var mdlLoc = obj.AddComponent<ModelLocator>();

            state.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            state.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

            network.stateMachines = new EntityStateMachine[] { state
                };

            rigid.useGravity = false;

            hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

            hb.healthComponent = health;
            hb.isBullseye = true;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            hurtbox.hurtBoxes = new HurtBox[] { hb };
            hurtbox.mainHurtBox = hb;
            hurtbox.bullseyeCount = 1;

            filter.teamIndex = TeamIndex.Monster;

            characterBody.baseAcceleration = 0;
            characterBody.baseArmor = 0; //Base armor this character has, set to 20 if this character is melee 
            characterBody.baseAttackSpeed = 0; //Base attack speed, usually 1
            characterBody.baseCrit = 0;  //Base crit, usually one
            characterBody.baseDamage = 0; //Base damage
            characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
            characterBody.baseJumpPower = 0; //Base jump power
            characterBody.baseMaxHealth = 500; //Base health, basically the health you have when you start a new run
            characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
            characterBody.baseMoveSpeed = 0; //Base move speed, this is usual 7
            characterBody.baseNameToken = ""; //The base name token. 
            characterBody.subtitleNameToken = ""; //Set this if its a boss
            characterBody.baseRegen = 0f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Masterless); ///Base body flags, should be self explanatory 
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
            characterBody.levelMaxHealth = 100; //Health gained when leveling up. 
            characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
            characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
            characterBody.levelRegen = 0; //Regen gained when leveling up. 

            health.health = 1000;
            health.globalDeathEventChanceCoefficient = 1;
            health.body = characterBody;

            mdlLoc.modelTransform = obj.transform;
            mdlLoc.dontReleaseModelOnDeath = true;
            mdlLoc.dontDetatchFromParent = true;
            mdlLoc.noCorpse = true;

            obj.AddComponent<InfestationTrigger>();

            voidMat = obj.GetComponent<Renderer>().material;

            PrefabAPI.RegisterNetworkPrefab(obj);

            infectionTrigger = obj;
        }

        private void FindAssets()
        {
            var asyncStageLoad = SceneManager.LoadSceneAsync("arena", LoadSceneMode.Additive);
            asyncStageLoad.allowSceneActivation = false;
            GameObject yo = null;
            asyncStageLoad.completed += ___ => {
                var scene = SceneManager.GetSceneByName("arena"); ;
                var root = scene.GetRootGameObjects();

                for (int i = 0; i < root.Length; i++)
                {
                    if (root[i].name == "HOLDER: Ruins and Gameplay Zones")
                    {
                        var sphere = root[i].transform.Find("ShrineCombat/Base/Sphere/");
                        var platform = root[i].transform.Find("WPPlatform2");
                        var plat = root[i].transform.Find("ArenaEnemySpawnZone");

                        platform.gameObject.SetActive(true);
                        plat.gameObject.SetActive(true);

                        foreach (Transform form in platform) {
                            form.gameObject.SetActive(true);
                        }

                        floatingPlatform2 = plat.gameObject.InstantiateClone("VoidPlatform2", false);

                        floatingPlatform = platform.gameObject.InstantiateClone("VoidPlatform", false);

                        if (sphere)
                        {
                            //Do stuff
                            yo = sphere.gameObject.InstantiateClone("TrueInfectionTrigger", false);
                            LogCore.LogI(sphere.gameObject);
                        }


                        // LogCore.LogW("FOUND");
                        /*foreach (Transform form in root[i].transform)
                        {
                            if (form.name == "ShrineCombat")
                            {
                                //LogCore.LogE("FOUND1");
                                foreach (Transform bruh in form)
                                {
                                    if (bruh.name == "Base")
                                    {
                                        //LogCore.LogF("FOUND2");
                                        foreach (Transform lmao in bruh)
                                        {
                                            if (lmao.name == "Sphere")
                                            {
                                                // LogCore.LogF("FOUND3");
                                                //sphere = lmao.gameObject;
                                                yo = lmao.gameObject.InstantiateClone("TrueInfectionTrigger", false);
                                                LogCore.LogI(lmao.gameObject);
                                                //LogCore.LogI(sphere);
                                            }
                                        }
                                    }
                                }

                            }
                        }*/
                    }
                }

                CreateInfection(yo);


                SceneManager.UnloadSceneAsync("arena");
            };


        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (self && self.baseNameToken == "LUNARWISP_BODY_NAME" || self.baseNameToken == "LUNARGOLEM_BODY_NAME" || self.baseNameToken == "BROTHER_BODY_NAME" && triggeredTrueInfection)
            {
                LogCore.LogI(triggeredTrueInfection);

                var mdl = CloudUtils.GetCharacterModelFromCharacterBody(self);
                if (mdl && triggeredTrueInfection)
                {
                    LogCore.LogI(triggeredTrueInfection);

                    var setstateonhurt = self.GetComponent<SetStateOnHurt>();

                    if (setstateonhurt) {
                        setstateonhurt.canBeFrozen = false;
                        setstateonhurt.canBeHitStunned = false;
                        setstateonhurt.canBeStunned = false;
                    }

                    DetermineVoidInfestation(mdl, self.baseNameToken);

                    if (self.baseNameToken != "BROTHER_BODY_NAME")
                    {
                        foreach (GenericSkill i in self.skillLocator.allSkills) {
                            i.maxStock += 2;
                        }
                        self.baseMoveSpeed += 2;
                        self.baseAttackSpeed += 2;
                        self.AddComponent<EmitVoid>();
                    }
                    else {
                        self.AddComponent<EmitVoidBrother>();
                    }

                }
            }
        }

        private void DetermineVoidInfestation(CharacterModel mdl, string nameToken) {
            mdl.baseRendererInfos[0].defaultMaterial = voidMat;

            if (nameToken == "BROTHER_BODY_NAME")
            {
                if (PhaseCounter.instance)  
                {
                    switch (PhaseCounter.instance.phase) {
                        case 1:

                            mdl.baseRendererInfos[1].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[2].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[4].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[7].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[8].defaultMaterial = voidMat;
                            break;
                        case 2:
                            break;
                        case 3:
                            mdl.baseRendererInfos[1].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[2].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[4].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[5].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[7].defaultMaterial = voidMat;
                            mdl.baseRendererInfos[8].defaultMaterial = voidMat;
                            break;
                        case 4:
                            for (int i = 0; i < mdl.baseRendererInfos.Length; i++)
                            {
                                mdl.baseRendererInfos[i].defaultMaterial = voidMat;
                            }
                            break;
                    }
                }
            }

            if (nameToken == "LUNARWISP_BODY_NAME") {
                mdl.baseRendererInfos[0].defaultMaterial = voidMat;
            }

        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (self)
            {
                SpawnInfectionBazaar();
                SpawnInfectionMoon();
                if (triggeredTrueInfection)
                {
                    self.monsterCredit += (self.monsterCredit / 2);

                    PassivelyTriggerVoidEffect();
                }
            }
        }

        private void PassivelyTriggerVoidEffect() {
            PostProcessProfile[] source = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
            PostProcessProfile profile = (from p in source
                                          where p.name == "ppSceneArenaSick"//"ppLocalUnderwater"
                                          select p).FirstOrDefault<PostProcessProfile>();
            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("moon"))
            {
                CloudUtils.PostProcessingOverlay(profile);
            }
            else
            {
                CloudUtils.AlterCurrentPostProcessing(profile);
            }

            /*PostProcessVolume[] volume = GameObject.FindObjectsOfType<PostProcessVolume>();//vCloudUtils.FindCurrentPostProcessing();

            for (int i = 0; i < volume.Length; i++)
            {
                volume[i].profile = profile;
                volume[i].weight = 0.85f;   
            }*/
        }

        private void SpawnInfectionMoon()
        {
            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("moon") && NetworkServer.active && triggeredTrueInfection) {
                SceneInfo info = SceneInfo.instance;
                var profile = (from p in CommonAssets.profiles
                               where p.name == "ppSceneArenaSick"
                               select p).FirstOrDefault<PostProcessProfile>();

                var objs = GameObject.FindObjectsOfType<Light>();
                for (int i = 0; i < objs.Length; i++)
                {
                    objs[i].intensity = 0.1f;
                }

                var spawner = SceneInfo.instance.transform.Find("BrotherMissionController/BrotherEncounter, Phase 2");
                var combatEncounter =  spawner.GetComponent<ScriptedCombatEncounter>();

                combatEncounter.spawns[3].spawnCard = golems.characterSpawnCard;
                combatEncounter.spawns[4].spawnCard = golems.characterSpawnCard;
                combatEncounter.spawns[5].spawnCard = golems.characterSpawnCard;

                /*foreach (Transform form in spawner.transform) {
                    CloudburstPlugin.Destroy(form.gameObject);
                }

                
                var a = CloudburstPlugin.Instantiate<GameObject>(monsterMaster);
                a.GetComponent<CharacterMaster>().teamIndex = TeamIndex.Monster;
                a.transform.position = new Vector3(1447.18f, 494.8316f, 776.7117f);
                a.transform.SetParent(spawner.transform);*/


                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform, new Vector3(1146, 857.3f, 185.6f), Quaternion.identity).AddComponent<Spinner>();
                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform, new Vector3(315, 600, -138), Quaternion.identity).AddComponent<Spinner>();
                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform, new Vector3(-136, 240, -333), Quaternion.identity).AddComponent<Spinner>();

                //sky
                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform, new Vector3(-123.2f, 776.4f, -35.2f), Quaternion.identity).AddComponent<Spinner>();
                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform2, new Vector3(-134.4f, 651.3f, 100f), Quaternion.identity).AddComponent<Spinner>();
                CloudburstPlugin.Instantiate<GameObject>(floatingPlatform2, new Vector3(-200, 787.9f, 25f), Quaternion.identity).AddComponent<Spinner>();
            }
        }

            private void SpawnInfectionBazaar()
        {
            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("bazaar") && NetworkServer.active && !triggeredTrueInfection && spawnInfection)
            {
                var o2bj = GameObject.Find("LunarTable");
                GameObject obj = GameObject.Instantiate<GameObject>(infectionTrigger, o2bj.transform.position, Quaternion.identity);
                obj.transform.localScale = new Vector3(20, 20, 20);
                InfestationTrigger.OnKilled += InfestationTrigger_OnKilled;
                NetworkServer.Spawn(obj);
            }
        }

        private void InfestationTrigger_OnKilled(Vector3 obj)
        {
            triggeredTrueInfection = true;
            Wave wave = new Wave()
            {
                amplitude = 0.3f,
                cycleOffset = 0,
                frequency = 100,
            };
            var death = ShakeEmitter.CreateSimpleShakeEmitter(obj, wave, 5, 1000, true);
            death.StartShake();


            MithrixReplacementLines();





            EntityStates.BrotherMonster.HoldSkyLeap.duration /= 4;
        }

        public static void MithrixReplacementLines()
        {
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_1", "Become one.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_2", "Embrace.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_3", "Fracture.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_4", "Void.");

            if (Util.CheckRoll(50))
            {
                LanguageAPI.Add("BROTHER_KILL_1", "BE LOCKED AWAY.");
                LanguageAPI.Add("BROTHER_KILL_2", "AND STAY DOWN.");
                LanguageAPI.Add("BROTHER_KILL_3", "KNEEL BEFORE US.");
                LanguageAPI.Add("BROTHER_KILL_4", "PATHETIC.");
                LanguageAPI.Add("BROTHER_KILL_5", "YIELD.");
            }
            else
            {
                LanguageAPI.Add("BROTHER_KILL_1", "OH WHEN WILL YOU COME BACK.");
                LanguageAPI.Add("BROTHER_KILL_2", "FIRST TIME?");
                LanguageAPI.Add("BROTHER_KILL_3", "NO ONE WILL HEAR YOU");
                LanguageAPI.Add("BROTHER_KILL_4", "ENDLESS BAUBLES CANNOT SAVE YOU");
                LanguageAPI.Add("BROTHER_KILL_5", "THERE IS NO END FOR YOU.");
            }
            if (Util.CheckRoll(0.1f))
            {
                LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "HOT DAMN YOU PIECE OF SHIT, JUST WHEN WILL YOU GIVE UP, JUST WHEN WILL YOU QUIT? YOUR PARENTS DON'T LOVE YOU AND FOR WHAT IT'S WORTH THERE'S REASON TO SUGGEST YOU WERE DROPPED AT BIRTH.");

            }
            else {
                LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "SUBMIT.");

            }
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_2", "CONCEDE YOUR EXISTENCE.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_3", "MAKE PEACE");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_4", "STOP RESISTING.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_5", "BE DETAINED.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_6", "ADMIT.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_7", "NO ONE WILL HEAR YOU SCREAM.");
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_8", "CONFESS, VERMIN.");
            if (Util.CheckRoll(0.1f))
            {
                LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "Heh, nice cock, vermin.");
            }
            else
            {
                LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "PLANET PEST.");
            }
            LanguageAPI.Add("BROTHER_DAMAGEDEALT_10", "WHEN WILL YOU MAKE YOUR PEACE?");

            Add("BROTHER_BODY_NAME", "Nilthnix");
            Add("BROTHER_BODY_SUBTITLE", "Slave to Eternity.");

            LanguageAPI.Add("BROTHER_DEATH_1", "FUTILITY...");
            LanguageAPI.Add("BROTHER_DEATH_2", "WE ARE NUMBERLESS...");
            LanguageAPI.Add("BROTHER_DEATH_3", "THIS MATTERS NOT...!");
            LanguageAPI.Add("BROTHER_DEATH_4", "YOU SHALL BE PURSUED, MY DEATH MEANS NOTH—");
            LanguageAPI.Add("BROTHER_DEATH_5", "EXECUTE THEM, EXECUTE TH—");
            LanguageAPI.Add("BROTHER_DEATH_6", "WE ARE A UNIVERSAL CONSTANT... YOU CANNOT KILL U-");
            Language.CCLanguageReload(new ConCommandArgs());
        }

        [ConCommand(commandName = "trigger_infection", flags = ConVarFlags.ExecuteOnServer, helpText = "Enables the void infection bubble to spawn.")]
        private static void TriggerInfection(ConCommandArgs arg)
        {
            spawnInfection = true;
        }

        [ConCommand(commandName = "ttrigger_infection", flags = ConVarFlags.ExecuteOnServer, helpText = "Triggers the true infection. God help you.")]
        private static void TTriggerInfection(ConCommandArgs arg)
        {
            triggeredTrueInfection = true;
            VoidCore.MithrixReplacementLines();
        }

        private void ArenaMissionController_onBeatArena()
        {
            spawnInfection = true;
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
            {
                baseToken = "TRIGGER_INFECTION",
            });
        }
    }
}
