using Cloudburst.Cores.Components;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Cloudburst.Cores
{
    class VoidCore
    {

        public VoidCore() => FindGod();

        public static bool spawnInfection;
        public static bool triggeredTrueInfection;

        public static Material voidMat;
        private GameObject infectionTrigger;

        private void FindGod()
        {
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            ArenaMissionController.onBeatArena += ArenaMissionController_onBeatArena;
            Run.onRunStartGlobal += Run_onRunStartGlobal;

            FindSphere();

        }

        private void Run_onRunStartGlobal(Run obj)
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
            spawnInfection = false;
            triggeredTrueInfection = false;
        }

        private void CreateInfection(GameObject re) {
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

        private void FindSphere()
        {
            var asyncStageLoad = SceneManager.LoadSceneAsync("arena", LoadSceneMode.Additive);
            asyncStageLoad.allowSceneActivation = false;
            GameObject yo = null;
            asyncStageLoad.completed += ___ => {
                var scene = SceneManager.GetSceneByName("arena"); ;
                var root = scene.GetRootGameObjects();

                List<Transform> michealPense = new List<Transform>();
                for (int i = 0; i < root.Length; i++)
                {
                    if (root[i].name == "HOLDER: Ruins and Gameplay Zones")
                    {
                        // LogCore.LogW("FOUND");
                        foreach (Transform form in root[i].transform)
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
                                                yo = lmao.gameObject.InstantiateClone("CoolAssSphere", false);
                                                LogCore.LogI(lmao.gameObject);
                                                //LogCore.LogI(sphere);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                CreateInfection(yo);


                LogCore.LogF("Micheal Pense is " + michealPense.Count + "ft long!");

                SceneManager.UnloadSceneAsync("arena");
            };


        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (triggeredTrueInfection && self && self.baseNameToken == "LUNARWISP_BODY_NAME" || self.baseNameToken == "LUNARGOLEM_BODY_NAME" || self.baseNameToken == "BROTHER_BODY_NAME")
            {
                var mdl = CloudUtils.GetCharacterModelFromCharacterBody(self);
                if (mdl)
                {
                    var setstateonhurt = self.GetComponent<SetStateOnHurt>();

                    if (setstateonhurt) {
                        setstateonhurt.canBeFrozen = false;
                        setstateonhurt.canBeHitStunned = false;
                        setstateonhurt.canBeStunned = false;
                    }

                    DetermineVoidInfestation(mdl, self.baseNameToken);

                    if (self.baseNameToken != "BROTHER_BODY_NAME")
                    {
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
                            for (int i = 0; i < mdl.baseRendererInfos.Length; i++) {
                                mdl.baseRendererInfos[i].defaultMaterial = voidMat;
                            }
                            break;



                } }
            }

            if (nameToken == "LUNARWISP_BODY_NAME") {
                for (int i = 0; i < mdl.baseRendererInfos.Length; i++) {
                    mdl.baseRendererInfos[i].defaultMaterial = voidMat;
                }
            }

        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (self && SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName("bazaar") && NetworkServer.active && !triggeredTrueInfection && spawnInfection)
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

            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_1", "Become one.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_2", "Embrace.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_3", "Fracture.");
            LanguageAPI.Add("BROTHER_SPAWN_PHASE1_4", "Void.");

            LanguageAPI.Add("BROTHER_KILL_1", "Become void.");
            LanguageAPI.Add("BROTHER_KILL_2", "Submit to them.");
            LanguageAPI.Add("BROTHER_KILL_3", "Ascend from time.");
            LanguageAPI.Add("BROTHER_KILL_4", "Devolve into us.");
            LanguageAPI.Add("BROTHER_KILL_5", "They will remember.");

        }

 

        private void ArenaMissionController_onBeatArena()
        {
            spawnInfection = true;
        }
    }
}
