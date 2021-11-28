using Cloudburst.Cores.Components;

using EntityStates.BrotherMonster;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
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
//using static R2API.LanguageAPI;
using static RoR2.CharacterAI.AISkillDriver;

namespace Cloudburst.Cores
{
    class VoidCore
    {
        public VoidCore instance;

        internal class MithrixWeaponHooks : EntityStateHook {
            bool hasFired;
            public override void Hook() {
                base.Hook();

            }


            public override void Unhook()
            {
                base.Unhook();

            }

        }

        internal class EntityStateHook {
            public virtual void Hook()
            {

            }

            public virtual void Unhook()
            {

            }
        }
        /// <summary>
        /// https://cdn.discordapp.com/attachments/739717122229403740/804910319029321728/IMG_20210129_112137.jpg
        /// </summary>
        internal class VoidGolems : EnemyBuilder {

            protected override string resourceMasterPath => "prefabs/charactermasters/LunarGolemMaster";

            protected override string resourceBodyPath => "prefabs/characterbodies/LunarGolemBody";

            protected override string bodyName => "VoidGolem";

            protected override bool registerNetwork => true;

            public override int DirectorCost => 350;

            public override bool NoElites => false;

            public override bool ForbiddenAsBoss => false;

            public override HullClassification HullClassification => HullClassification.Golem;

            public override MapNodeGroup.GraphType GraphType => MapNodeGroup.GraphType.Ground;

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

                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
    
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

                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(def);
                Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(States.VoidGolems.EngageShell));

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
    
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateUtility(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyShield");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
    
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateSpecial(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/lunargolembody/LunarGolemBodyShield");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();

                CloudUtils.CopySkillDefSettings(origDef, def);

                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
    
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

        public MithrixWeaponHooks hooks = new MithrixWeaponHooks();

        public VoidCore() => FindGod();

        public static bool spawnInfection;
        public static bool triggeredTrueInfection;

        public static Material voidMat;

        bool hasFired = true;


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

            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += WeaponSlam_OnEnter;
            On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += HoldSkyLeap_OnEnter;
            On.EntityStates.BrotherMonster.WeaponSlam.FixedUpdate += WeaponSlam_FixedUpdate;

            //On.EntityStates.Missions.Arena.NullWard.Active.FixedUpdate += Active_FixedUpdate;
            CloudburstPlugin.start += CloudburstPlugin_start;



            golems = new VoidGolems();
            golems.Create();

            R2API.LanguageAPI.Add("TRIGGER_INFECTION", "A foul infection creeps into the exchange between time...");

            FindAssets();

            VoidGlass();
        }

        private void WeaponSlam_FixedUpdate(On.EntityStates.BrotherMonster.WeaponSlam.orig_FixedUpdate orig, EntityStates.BrotherMonster.WeaponSlam self)
        {
            if (triggeredTrueInfection)
            {
                if (self.isAuthority)
                {
                    bool hasDoneBlastAttack = self.hasDoneBlastAttack;
                    if (hasDoneBlastAttack)
                    {
                        //self.modelTransform;
                        if (this.hasFired == false)
                        {
                            hasFired = true;
                            float num = 360f / (float)ExitSkyLeap.waveProjectileCount;
                            Vector3 point = Vector3.ProjectOnPlane(self.inputBank.aimDirection, Vector3.up);
                            Vector3 footPosition = self.characterBody.footPosition;
                            for (int i = 0; i < ExitSkyLeap.waveProjectileCount; i++)
                            {
                                Vector3 forward = Quaternion.AngleAxis(num * (float)i, Vector3.up) * point;
                                if (self.isAuthority)
                                {
                                    ProjectileManager.instance.FireProjectile(ExitSkyLeap.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * 3, ExitSkyLeap.waveProjectileForce, self.characterBody.RollCrit(), DamageColorIndex.Default, null, -1f);
                                }
                            }
                        }
                    }
                }
            }
            orig(self);
        }

        private void WeaponSlam_OnEnter(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, EntityStates.BrotherMonster.WeaponSlam self)
        {
            orig(self);
            hasFired = false;
            if (triggeredTrueInfection)
            {
                self.weaponAttack.damageType = DamageType.Nullify;
            }
        }
        private void HoldSkyLeap_OnEnter(On.EntityStates.BrotherMonster.HoldSkyLeap.orig_OnEnter orig, EntityStates.BrotherMonster.HoldSkyLeap self)
        {
            orig(self);
            if (triggeredTrueInfection)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.searchOrigin = self.transform.position;
                bullseyeSearch.maxDistanceFilter = 500;
                bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(self.GetTeam());
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                bullseyeSearch.filterByLoS = true;
                bullseyeSearch.searchDirection = Vector3.up;
                bullseyeSearch.RefreshCandidates();
                bullseyeSearch.FilterOutGameObject(self.gameObject);
                var target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();

                if (target)
                {
                    self.characterMotor.Motor.SetPositionAndRotation(target.transform.position, Quaternion.identity, true);
                }
            }
        }

        private void CloudburstPlugin_start()
        {
            skyLeap = EntityStates.BrotherMonster.HoldSkyLeap.duration;
        }

        public void VoidGlass() {
            var mdl = Resources.Load<GameObject>("prefabs/characterbodies/BrotherGlassBody").GetComponent<CharacterBody>();
            R2API.LanguageAPI.Add("GLASS_BODY_NAME", "Shattering Clone");
            mdl.baseNameToken = "GLASS_BODY_NAME";
            mdl.gameObject.AddComponent<Glass>();

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
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_1", "Pray.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_2", "Beg.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_3", "Die.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_4", "Be slaughtered.");
            //Language.CCLanguageReload(new ConCommandArgs());

            R2API.LanguageAPI.Add("BROTHER_KILL_1", "Return to dirt.");
            R2API.LanguageAPI.Add("BROTHER_KILL_2", "Submit, vermin.");
            R2API.LanguageAPI.Add("BROTHER_KILL_3", "Die, vermin.");
            R2API.LanguageAPI.Add("BROTHER_KILL_4", "Die, weakling.");
            R2API.LanguageAPI.Add("BROTHER_KILL_5", "Become memories.");

            //Language.CCLanguageReload(new ConCommandArgs());


            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "Bleed.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_2", "Now is the time for fear.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_3", "Weak.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_4", "Frail - and soft.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_5", "You are nothing.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_6", "Mistake.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_7", "Scream, vermin.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_8", "Break beneath me.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "Slow.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_10", "Your body will shatter.");
            //Language.CCLanguageReload(new ConCommandArgs());

            R2API.LanguageAPI.Add("BROTHER_KILL_1", "Return to dirt.");
            R2API.LanguageAPI.Add("BROTHER_KILL_2", "Submit, vermin.");
            R2API.LanguageAPI.Add("BROTHER_KILL_3", "Die, vermin.");
            R2API.LanguageAPI.Add("BROTHER_KILL_4", "Die, weakling.");
            R2API.LanguageAPI.Add("BROTHER_KILL_5", "Become memories.");
            //Language.CCLanguageReload(new ConCommandArgs());

            R2API.LanguageAPI.Add("BROTHER_DEATH_1", "NO... NOT NOW...");
            R2API.LanguageAPI.Add("BROTHER_DEATH_2", "WHY... WHY NOW...?");
            R2API.LanguageAPI.Add("BROTHER_DEATH_3", "NO... NO...!");
            R2API.LanguageAPI.Add("BROTHER_DEATH_4", "BROTHER... HELP ME...!");
            R2API.LanguageAPI.Add("BROTHER_DEATH_5", "THIS PLANE GROWS DARK... BROTHER... I CANNOT SEE YOU... WHERE ARE YOU...?");
            R2API.LanguageAPI.Add("BROTHER_DEATH_6", "BROTHER... PERHAPS... WE WILL GET IT RIGHT... NEXT TIME...");

            //Language.CCLanguageReload(new ConCommandArgs());
        }

        private void CreateInfection(GameObject re)
        {
            GameObject obj = re;
            voidMat = obj.GetComponent<Renderer>().material;
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

                        CapsuleCollider clayCollider = obj.AddComponent<CapsuleCollider>();
            clayCollider.center -= new Vector3(0, 0.6f, 0);
            clayCollider.height *= 0.25f;
            clayCollider.radius *= 1.16f;
            hb.isBullseye = true;
            hb.gameObject.layer = LayerIndex.entityPrecise.intVal;
            hb.healthComponent = health;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            HurtBox[] clayHurtBoxArray = new HurtBox[]
            {
                hb
            };

            hurtbox.bullseyeCount = 1;
            hurtbox.hurtBoxes = clayHurtBoxArray;
            hurtbox.mainHurtBox = hb;

            /*hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

            hb.healthComponent = health;
            hb.isBullseye = true;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            hurtbox.hurtBoxes = new HurtBox[] { hb };
            hurtbox.mainHurtBox = hb;
            hurtbox.bullseyeCount = 1;*/

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


            PrefabAPI.RegisterNetworkPrefab(obj);
            CloudUtils.RegisterNewBody(obj);

            this.infectionTrigger = obj;
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

            /*hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

            hb.healthComponent = health;
            hb.isBullseye = true;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            hurtbox.hurtBoxes = new HurtBox[] { hb };
            hurtbox.mainHurtBox = hb;
            hurtbox.bullseyeCount = 1;*/

            //HurtBoxGroup clayHurtBoxGroup = obj.AddComponent<HurtBoxGroup>();
            CapsuleCollider clayCollider = obj.AddComponent<CapsuleCollider>();
            clayCollider.center -= new Vector3(0, 0.6f, 0);
            clayCollider.height *= 0.25f;
            clayCollider.radius *= 1.16f;
            hb.isBullseye = true;
            hb.gameObject.layer = LayerIndex.entityPrecise.intVal;
            hb.healthComponent = health;
            hb.damageModifier = HurtBox.DamageModifier.Normal;
            hb.hurtBoxGroup = hurtbox;
            hb.indexInGroup = 0;

            HurtBox[] clayHurtBoxArray = new HurtBox[]
            {
                hb
            };

            hurtbox.bullseyeCount = 1;
            hurtbox.hurtBoxes = clayHurtBoxArray;
            hurtbox.mainHurtBox = hb;

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
            CloudUtils.RegisterNewBody(obj);

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
                    switch (PhaseCounter.instance.phase)
                    {
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
            if (self)
            {
                //                PostProcessProfile[] source = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
                //                PostProcessProfile profile = (from p in source
                //                                 where p.name == "ppSceneEclipseStandard"//"ppLocalUnderwater"
                //                                 select p).FirstOrDefault<PostProcessProfile>();
                //              CloudUtils.AlterCurrentPostProcessing(profile);

                SpawnInfectionBazaar();
                SpawnInfectionMoon();
                if (triggeredTrueInfection)
                {
                    self.monsterCredit += (self.monsterCredit / 2);

                    PassivelyTriggerVoidEffect();
                }
            }
            orig(self);
        }

        private void PassivelyTriggerVoidEffect() {
            PostProcessProfile[] source = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
            PostProcessProfile profile = (from p in source
                                          where p.name == "ppSceneArenaSick"//"ppLocalUnderwater"
                                          select p).FirstOrDefault<PostProcessProfile>();
            CloudUtils.PostProcessingOverlay(profile);
            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("moon2"))
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
            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("bazaar"))
            {
                LogCore.LogI("Is bazaar");
                if (NetworkServer.active)
                {
                    LogCore.LogI("Server active");

                    if (!triggeredTrueInfection && spawnInfection)
                    {
                        LogCore.LogI("Can spawn");
                        var o2bj = GameObject.Find("LunarTable");
                        GameObject obj = GameObject.Instantiate<GameObject>(infectionTrigger, o2bj.transform.position, Quaternion.identity);
                        obj.transform.localScale = new Vector3(20, 20, 20);
                        InfestationTrigger.OnKilled += InfestationTrigger_OnKilled;
                        NetworkServer.Spawn(obj);
                    }
                }
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
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_1", "Become one.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_2", "Embrace.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_3", "Fracture.");
            R2API.LanguageAPI.Add("BROTHER_SPAWN_PHASE1_4", "Void.");

            if (Util.CheckRoll(50))
            {
                R2API.LanguageAPI.Add("BROTHER_KILL_1", "BE LOCKED AWAY.");
                R2API.LanguageAPI.Add("BROTHER_KILL_2", "AND STAY DOWN.");
                R2API.LanguageAPI.Add("BROTHER_KILL_3", "KNEEL BEFORE US.");
                R2API.LanguageAPI.Add("BROTHER_KILL_4", "PATHETIC.");
                R2API.LanguageAPI.Add("BROTHER_KILL_5", "YIELD.");
            }
            else
            {
                R2API.LanguageAPI.Add("BROTHER_KILL_1", "OH WHEN WILL YOU COME BACK.");
                R2API.LanguageAPI.Add("BROTHER_KILL_2", "FIRST TIME?");
                R2API.LanguageAPI.Add("BROTHER_KILL_3", "NO ONE WILL HEAR YOU");
                R2API.LanguageAPI.Add("BROTHER_KILL_4", "ENDLESS BAUBLES CANNOT SAVE YOU");
                R2API.LanguageAPI.Add("BROTHER_KILL_5", "THERE IS NO END FOR YOU.");
            }
            if (Util.CheckRoll(0.1f))
            {
                R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "HOT DAMN YOU PIECE OF SHIT, JUST WHEN WILL YOU GIVE UP, JUST WHEN WILL YOU QUIT? YOUR PARENTS DON'T LOVE YOU AND FOR WHAT IT'S WORTH THERE'S REASON TO SUGGEST YOU WERE DROPPED AT BIRTH.");

            }
            else {
                R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_1", "SUBMIT.");

            }
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_2", "CONCEDE YOUR EXISTENCE.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_3", "MAKE PEACE");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_4", "STOP RESISTING.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_5", "BE DETAINED.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_6", "ADMIT.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_7", "NO ONE WILL HEAR YOU SCREAM.");
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_8", "CONFESS, VERMIN.");
            if (Util.CheckRoll(0.1f))
            {
                R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "Heh, nice cock, vermin.");
            }
            else
            {
                R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_9", "PLANET PEST.");
            }
            R2API.LanguageAPI.Add("BROTHER_DAMAGEDEALT_10", "WHEN WILL YOU MAKE YOUR PEACE?");

            R2API.LanguageAPI.Add("BROTHER_BODY_NAME", "Nilthnix");
            R2API.LanguageAPI.Add("BROTHER_BODY_SUBTITLE", "Slave to Eternity.");

            R2API.LanguageAPI.Add("BROTHER_DEATH_1", "FUTILITY...");
            R2API.LanguageAPI.Add("BROTHER_DEATH_2", "WE ARE NUMBERLESS...");
            R2API.LanguageAPI.Add("BROTHER_DEATH_3", "THIS MATTERS NOT...!");
            R2API.LanguageAPI.Add("BROTHER_DEATH_4", "YOU SHALL BE PURSUED, MY DEATH MEANS NOTH—");
            R2API.LanguageAPI.Add("BROTHER_DEATH_5", "EXECUTE THEM, EXECUTE TH—");
            R2API.LanguageAPI.Add("BROTHER_DEATH_6", "WE ARE A UNIVERSAL CONSTANT... YOU CANNOT KILL U-");
            //Language.CCLanguageReload(new ConCommandArgs());
        }

        [ConCommand(commandName = "trigger_infection", flags = ConVarFlags.ExecuteOnServer, helpText = "Enables the void infection bubble to spawn.")]
        private static void TriggerInfection(ConCommandArgs arg)
        {
            spawnInfection = true;
        }

        [ConCommand(commandName = "ttrigger_infection", flags = ConVarFlags.ExecuteOnServer, helpText = "Triggers the infection, bypassing the infection bubble entirely.")]
        private static void TTriggerInfection(ConCommandArgs arg)
        {
            triggeredTrueInfection = true;
            VoidCore.MithrixReplacementLines();
        }

        private void ArenaMissionController_onBeatArena()
        {
            spawnInfection = true;
            LogCore.LogI("triggered arena");
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
            {
                baseToken = "TRIGGER_INFECTION",
            });
        }
    }
}
