﻿using Cloudburst;
using Cloudburst.Content;

//using Cloudburst.Cores.ClayMan.Skills;
using EntityStates;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.CharacterAI.AISkillDriver;

namespace Cloudburst.Cores
{
    //TODO:
    //Nothing
    class ClayMenCore
    {
        public static ClayMenCore instance;

        private GameObject clayMan;
        private GameObject clayManMaster;


        private SkillLocator skillLocator;
        public ClayMenCore() => BuildClayMen();

        public void BuildClayMen()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());
            instance = this;
            
            clayMan = Resources.Load<GameObject>("prefabs/characterbodies/ClayBody");
            clayManMaster = Resources.Load<GameObject>("prefabs/charactermasters/ClaymanMaster");
            skillLocator = clayMan.GetComponent<SkillLocator>();

            BuildBody();
            BuildDirectorCard();
            RebuildSkillDrivers();
            RebuildSkills();
            FixHurtBox();
            FixInteractorandEquipment();

            //LogCore.LogM("Built Clay Men!");
        }

        private void BuildDirectorCard()
        {
            On.RoR2.CharacterSpawnCard.Awake += GlobalHooks.CharacterSpawnCard_Awake;
            CharacterSpawnCard characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            On.RoR2.CharacterSpawnCard.Awake -= GlobalHooks.CharacterSpawnCard_Awake;

            //DirectorAPI.DirectorCardHolder directorCardHolder = new DirectorAPI.DirectorCardHolder();
            DirectorCard directorCard = new DirectorCard();

            characterSpawnCard.directorCreditCost = 50;
            characterSpawnCard.forbiddenAsBoss = false;
            characterSpawnCard.name = "cscClaymanDude";
            //characterSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;
            characterSpawnCard.hullSize = HullClassification.Human;
            characterSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            characterSpawnCard.noElites = false;
            characterSpawnCard.prefab = clayManMaster;
            characterSpawnCard.occupyPosition = false;
            characterSpawnCard.sendOverNetwork = true;

            directorCard.allowAmbushSpawn = true;
            directorCard.forbiddenUnlockable = "";
            directorCard.minimumStageCompletions = 1;
            directorCard.preventOverhead = false;
            directorCard.requiredUnlockable = "";
            directorCard.selectionWeight = 1;
            directorCard.spawnCard = characterSpawnCard;
            directorCard.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            //directorCardHolder.Card = directorCard;
            //directorCardHolder.InteractableCategory = DirectorAPI.InteractableCategory.None;
            //directorCardHolder.MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters;
        }
        private void FixHurtBox()
        {
            //this is enigma's great spiral into insanity
            //welcome to the repo, son

            //what the fuck is with the russian women talking??

            //This is mdlClay gameobject
            GameObject mdlClayObject = clayMan.GetComponent<ModelLocator>().modelTransform.gameObject;
            //This is mdlClay
            Transform mdlClayTransform = clayMan.GetComponent<ModelLocator>().modelTransform;
            //Apparently mdlClay already has a HurtBoxGroup attached to it. That's nice and makes my job a hella lot easier.
            //APPARENTLY FUCK THAT
            //THERE WAS NO HURTBOXGROUP
            //THANKS HOPOO
            HurtBoxGroup hurtBoxGroup = mdlClayObject.AddComponent<HurtBoxGroup>();

            //unholy
            //ChildLocator locator = clayMan.GetComponentInChildren<ChildLocator>();

            //if (stomach)
            //{
            Transform armature = mdlClayTransform.Find("ClaymanArmature");
            if (armature)
            {
                Transform root = armature.Find("ROOT");
                LogCore.LogD("armature");

                if (root)
                {
                    LogCore.LogD("root");
                    //FUCK CSHARP//
                    Transform _base = root.Find("base");

                    if (_base)
                    {
                        LogCore.LogD("base");
                        Transform stomach = _base.Find("stomach");
                        if (stomach)
                        {
                            LogCore.LogD("stomach");
                            Transform chest = stomach.Find("chest");
                            if (chest)
                            {
                                LogCore.LogD("chest");

                                //AND HERE WE ARE
                                //THE MOMENT OF TRUTH

                                chest.gameObject.layer = LayerIndex.entityPrecise.intVal;

                                CapsuleCollider capsuleCollider = chest.AddComponent<CapsuleCollider>();

                                HurtBox hurtBox = chest.AddComponent<HurtBox>();
                                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                                hurtBox.healthComponent = clayMan.GetComponent<HealthComponent>();
                                hurtBox.hurtBoxGroup = hurtBoxGroup;
                                hurtBox.indexInGroup = 1;
                                hurtBox.isBullseye = true;

                                hurtBoxGroup.bullseyeCount = 1;
                                hurtBoxGroup.hurtBoxes = new HurtBox[] {
                                    hurtBox
                                };
                                hurtBoxGroup.mainHurtBox = hurtBox;

                                //PrefabAPI.RegisterNetworkPrefab(hurtBoxObject);
                            }
                        }
                    }
                }
            }
            mdlClayObject.gameObject.layer = LayerIndex.entityPrecise.intVal;

        }
        private void FixInteractorandEquipment()
        {
            Interactor interactor = clayMan.AddOrGetComponent<Interactor>();
            clayMan.AddOrGetComponent<InteractionDriver>().highlightInteractor = true;
            interactor.maxInteractionDistance = 4;

            //clayMan.AddOrGetComponent<TeamComponent>().teamIndex = TeamIndex.Monster;

            clayMan.AddComponent<EquipmentSlot>();
        }
        private void BuildBody()
        {
            CharacterBody characterBody = clayMan.GetComponent<CharacterBody>();
            if (characterBody)
            {
                R2API.LanguageAPI.Add("CLAYMAN_BODY_TOKEN", "Clay Man");
                characterBody.baseAcceleration = 80f;
                characterBody.baseArmor = 10; //Base armor this character has, set to 20 if this character is melee 
                characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
                characterBody.baseCrit = 1;  //Base crit, usually one
                characterBody.baseDamage = 12; //Base damage
                characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
                characterBody.baseJumpPower = 14; //Base jump power
                characterBody.baseMaxHealth = 110; //Base health, basically the health you have when you start a new run
                characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
                characterBody.baseMoveSpeed = 8; //Base move speed, this is usual 7
                characterBody.baseNameToken = "CLAYMAN_BODY_TOKEN"; //The base name token. 
                characterBody.subtitleNameToken = ""; //Set this to true if its a boss
                characterBody.baseRegen = 1.4f; //Base health regen.
                characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToGoo); ///Base body flags, should be self explanatory 
                //characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
                characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
                characterBody.hullClassification = HullClassification.Human; //The hull classification, usually used for AI
                characterBody.isChampion = false; //Set this to true if its A. a boss or B. a miniboss
                characterBody.levelArmor = 0; //Armor gained when leveling up. 
                characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
                characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
                characterBody.levelDamage = 2.5f; //Damage gained when leveling up. 
                characterBody.levelArmor = 3; //Armor gaix; //Health gained when leveling up. 
                characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
                characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
                characterBody.levelRegen = 0.6f; //Regen gained when leveling up. 
                                                 //characterBody.portraitIcon = portrait; //The portrait icon, shows up in multiplayer and the death UI
                                                 //characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); //The pod prefab this survivor spawns in. Options: Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod"); 
            }
        }
        private void RebuildSkillDrivers()
        {
            CloudUtils.DestroySkillDrivers(clayManMaster);

            AISkillDriver Swing = clayManMaster.AddComponent<AISkillDriver>();
            AISkillDriver Chase = clayManMaster.AddComponent<AISkillDriver>();
            AISkillDriver Leap = clayManMaster.AddComponent<AISkillDriver>();
            AISkillDriver anotherSkillDriver = clayManMaster.AddComponent<AISkillDriver>();
            AISkillDriver andAnotherSkillDriver = clayManMaster.AddComponent<AISkillDriver>();

            Swing.skillSlot = SkillSlot.Primary;
            Swing.requireSkillReady = false;
            Swing.requireEquipmentReady = false;
            Swing.moveTargetType = TargetType.CurrentEnemy;
            Swing.minUserHealthFraction = float.NegativeInfinity;
            Swing.maxUserHealthFraction = float.PositiveInfinity;
            Swing.minTargetHealthFraction = float.NegativeInfinity;
            Swing.maxTargetHealthFraction = float.PositiveInfinity;
            Swing.minDistance = 0;
            Swing.maxDistance = 2;
            Swing.selectionRequiresTargetLoS = false;
            Swing.activationRequiresTargetLoS = false;
            Swing.activationRequiresAimConfirmation = false;
            Swing.movementType = MovementType.ChaseMoveTarget;
            Swing.moveInputScale = 1;
            Swing.aimType = AimType.AtMoveTarget;
            Swing.ignoreNodeGraph = true;
            Swing.driverUpdateTimerOverride = -1;
            Swing.resetCurrentEnemyOnNextDriverSelection = false;
            Swing.noRepeat = false;
            Swing.shouldSprint = false;
            Swing.shouldFireEquipment = false;
            Swing.buttonPressType = ButtonPressType.Hold;

            Leap.skillSlot = SkillSlot.Secondary;
            Leap.requireSkillReady = false;
            Leap.requireEquipmentReady = false;
            Leap.moveTargetType = TargetType.CurrentEnemy;
            Leap.minUserHealthFraction = float.NegativeInfinity;
            Leap.maxUserHealthFraction = float.PositiveInfinity;
            Leap.minTargetHealthFraction = float.NegativeInfinity;
            Leap.maxTargetHealthFraction = float.PositiveInfinity;
            Leap.minDistance = 0;
            Leap.maxDistance = 30;
            Leap.selectionRequiresTargetLoS = false;
            Leap.activationRequiresTargetLoS = false;
            Leap.activationRequiresAimConfirmation = false;
            Leap.movementType = MovementType.ChaseMoveTarget;
            Leap.moveInputScale = 1;
            Leap.aimType = AimType.AtMoveTarget;
            Leap.ignoreNodeGraph = true;
            Leap.driverUpdateTimerOverride = -1;
            Leap.resetCurrentEnemyOnNextDriverSelection = false;
            Leap.noRepeat = false;
            Leap.shouldSprint = false;
            Leap.shouldFireEquipment = false;
            Leap.buttonPressType = ButtonPressType.Hold;

            Chase.skillSlot = SkillSlot.None;
            Chase.requireSkillReady = false;
            Chase.requireEquipmentReady = false;
            Chase.moveTargetType = TargetType.CurrentEnemy;
            Chase.minUserHealthFraction = float.NegativeInfinity;
            Chase.maxUserHealthFraction = float.PositiveInfinity;
            Chase.minTargetHealthFraction = float.NegativeInfinity;
            Chase.maxTargetHealthFraction = float.PositiveInfinity;
            Chase.minDistance = 10;
            Chase.maxDistance = 10;
            Chase.selectionRequiresTargetLoS = true;
            Chase.activationRequiresTargetLoS = false;
            Chase.activationRequiresAimConfirmation = false;
            Chase.movementType = MovementType.ChaseMoveTarget;
            Chase.moveInputScale = 1;
            Chase.aimType = AimType.AtMoveTarget;
            Chase.ignoreNodeGraph = true;
            Chase.driverUpdateTimerOverride = -1;
            Chase.resetCurrentEnemyOnNextDriverSelection = false;
            Chase.noRepeat = false;
            Chase.shouldSprint = false;
            Chase.shouldFireEquipment = false;
            Chase.buttonPressType = ButtonPressType.Hold;

            anotherSkillDriver.customName = "FollowNodeGraphToTarget";
            anotherSkillDriver.skillSlot = SkillSlot.None;
            anotherSkillDriver.requireSkillReady = false;
            anotherSkillDriver.requireEquipmentReady = false;
            anotherSkillDriver.moveTargetType = TargetType.CurrentEnemy;
            anotherSkillDriver.minUserHealthFraction = float.NegativeInfinity;
            anotherSkillDriver.maxUserHealthFraction = float.PositiveInfinity;
            anotherSkillDriver.minTargetHealthFraction = float.NegativeInfinity;
            anotherSkillDriver.maxTargetHealthFraction = float.PositiveInfinity;
            anotherSkillDriver.minDistance = 0;
            anotherSkillDriver.maxDistance = float.PositiveInfinity;
            anotherSkillDriver.selectionRequiresTargetLoS = false;
            anotherSkillDriver.activationRequiresTargetLoS = false;
            anotherSkillDriver.activationRequiresAimConfirmation = false;
            anotherSkillDriver.movementType = MovementType.ChaseMoveTarget;
            anotherSkillDriver.moveInputScale = 1;
            anotherSkillDriver.aimType = AimType.MoveDirection;
            anotherSkillDriver.ignoreNodeGraph = false;
            anotherSkillDriver.driverUpdateTimerOverride = -1;
            anotherSkillDriver.resetCurrentEnemyOnNextDriverSelection = false;
            anotherSkillDriver.noRepeat = false;
            anotherSkillDriver.shouldSprint = false;
            anotherSkillDriver.shouldFireEquipment = false;
            anotherSkillDriver.buttonPressType = ButtonPressType.Hold;

            andAnotherSkillDriver.customName = "ChaseOffNodegraph";
            andAnotherSkillDriver.skillSlot = SkillSlot.None;
            andAnotherSkillDriver.requireSkillReady = false;
            andAnotherSkillDriver.requireEquipmentReady = false;
            andAnotherSkillDriver.moveTargetType = TargetType.CurrentEnemy;
            andAnotherSkillDriver.minUserHealthFraction = float.NegativeInfinity;
            andAnotherSkillDriver.maxUserHealthFraction = float.PositiveInfinity;
            andAnotherSkillDriver.minTargetHealthFraction = float.NegativeInfinity;
            andAnotherSkillDriver.maxTargetHealthFraction = float.PositiveInfinity;
            andAnotherSkillDriver.minDistance = 0;
            andAnotherSkillDriver.maxDistance = 10;
            andAnotherSkillDriver.selectionRequiresTargetLoS = true;
            andAnotherSkillDriver.activationRequiresTargetLoS = false;
            andAnotherSkillDriver.activationRequiresAimConfirmation = false;
            andAnotherSkillDriver.movementType = MovementType.ChaseMoveTarget;
            andAnotherSkillDriver.moveInputScale = 1;
            andAnotherSkillDriver.aimType = AimType.AtMoveTarget;
            andAnotherSkillDriver.ignoreNodeGraph = true;
            andAnotherSkillDriver.driverUpdateTimerOverride = -1;
            andAnotherSkillDriver.resetCurrentEnemyOnNextDriverSelection = false;
            andAnotherSkillDriver.noRepeat = false;
            andAnotherSkillDriver.shouldSprint = false;
            andAnotherSkillDriver.shouldFireEquipment = false;
            andAnotherSkillDriver.buttonPressType = ButtonPressType.Hold;
        }
        private void RebuildSkills()
        {
            CloudUtils.DestroyGenericSkillComponents(clayMan);
            CreateSkillFamilies();
            CreatePrimary();
            CreateSecondary();
        }
        private void CreateSkillFamilies()
        {
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = clayMan.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = clayMan.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
        }
        private void CreatePrimary()
        {
            SkillDef primarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //primarySkillDef.activationState = new SerializableEntityStateType(typeof(SwipeForward));
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 1;
            primarySkillDef.baseRechargeInterval = 2;
            primarySkillDef.beginSkillCooldownOnSkillEnd = true;
            primarySkillDef.canceledFromSprinting = false;
            primarySkillDef.fullRestockOnAssign = true;
            primarySkillDef.interruptPriority = InterruptPriority.PrioritySkill;

            primarySkillDef.isCombatSkill = true;
            primarySkillDef.mustKeyPress = false;
            primarySkillDef.cancelSprintingOnActivation = false;
            primarySkillDef.rechargeStock = 1;
            primarySkillDef.requiredStock = 1;

            primarySkillDef.stockToConsume = 1;
            primarySkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAAAA";
            primarySkillDef.skillName = "aaa";
            primarySkillDef.skillNameToken = "aa";

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

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //secondarySkillDef.activationState = new SerializableEntityStateType(typeof(Leap));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 1;
            secondarySkillDef.baseRechargeInterval = 10f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = true;
            secondarySkillDef.interruptPriority = InterruptPriority.PrioritySkill;

            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = false;
            secondarySkillDef.cancelSprintingOnActivation = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;

            secondarySkillDef.stockToConsume = 1;
            secondarySkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAAAA";
            secondarySkillDef.skillName = "aaa";
            secondarySkillDef.skillNameToken = "aa";

            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(secondarySkillDef);
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondarySkillDef,

                viewableNode = new ViewablesCatalog.Node(secondarySkillDef.skillNameToken, false, null)

            };
        }
    }
}
