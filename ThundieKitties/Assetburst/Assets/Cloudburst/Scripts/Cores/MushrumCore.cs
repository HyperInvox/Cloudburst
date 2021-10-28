using EntityStates;
using Cloudburst.Cores.States.MegaMushrum;

using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using static RoR2.CharacterAI.AISkillDriver;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using EntityStates.AI.Walker;
using R2API;

namespace Cloudburst.Cores
{
    //TODO:
    //Make an icon.

    internal sealed class MegaMushrum
    {
        public GameObject megaMushrum;
        public GameObject megaMushrumMaster;
        public GameObject projectile;
        public GameObject dotZone;

        public SkillLocator skillLocator;

        public MegaMushrum() => BuildMegaMushrums();
        public void BuildMegaMushrums()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            megaMushrum = Resources.Load<GameObject>("prefabs/characterbodies/MiniMushroomBody").InstantiateClone("MegaMushrumBody", true);
            megaMushrumMaster = Resources.Load<GameObject>("prefabs/charactermasters/MiniMushroomMaster").InstantiateClone("MegaMushrumMaster", true);
            projectile = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile").InstantiateClone("MegaSporeGrenadeProjectile", false);
            dotZone = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone").InstantiateClone("MegaSporeGrenadeProjectileDotZone", false);

            skillLocator = megaMushrum.GetComponent<SkillLocator>();

            CloudUtils.RegisterNewBody(megaMushrum);
            CloudUtils.RegisterNewMaster(megaMushrumMaster);

            megaMushrumMaster.GetComponent<CharacterMaster>().bodyPrefab = megaMushrum;

            BuildBody();
            BuildDirectorCard();
            SetupMegaMushrumSize();
            RebuildSkillDrivers();
            RebuildSkills();
        }


        private void RegisterHooks()
        {
            /*IL.EntityStates.AI.Walker.Combat.UpdateAI += (il) =>
            {
                ILCursor curs = new ILCursor(il);
                //go to where movement type is checked (applying movement vector)
                curs.GotoNext(x => x.MatchCall<Vector3>("Cross"));
                curs.Index += 2;
                curs.Emit(OpCodes.Ldarg_0);
                curs.Emit(OpCodes.Ldfld, typeof(EntityState).GetFieldCached("outer"));
                curs.Emit(OpCodes.Ldloc_1);
                curs.EmitDelegate<Func<EntityStateMachine, AISkillDriver.MovementType, AISkillDriver.MovementType>>((ESM, MoveType) =>
                {
                    if (ESM.GetComponent<CharacterMaster>().GetBody().HasBuff(fearDebuff))
                    {
                        return AISkillDriver.MovementType.FleeMoveTarget;
                    }
                    else
                        return MoveType;
                });
                curs.Emit(OpCodes.Stloc_1);
            };

            IL.EntityStates.AI.Walker.Combat.UpdateSkillButtonInputs += (il) =>
            {
                ILCursor curs = new ILCursor(il);
                curs.GotoNext(x => x.MatchLdfld<Combat>("currentSkillMeetsActivationConditions"));
                curs.Index += 1;
                curs.Emit(OpCodes.Ldarg_0);
                curs.Emit(OpCodes.Ldfld, typeof(EntityState).GetFieldCached("outer"));
                curs.EmitDelegate<Func<bool, EntityStateMachine, bool>>((cond, ESM) =>
                {
                    if (ESM.GetComponent<CharacterMaster>().GetBody())
                        if (ESM.GetComponent<CharacterMaster>().GetBody().HasBuff(fearDebuff))
                            return false;
                    return cond;
                });
            };*/
        }


        private void BuildDirectorCard()
        {
            /*On.RoR2.CharacterSpawnCard.Awake += GlobalHooks.CharacterSpawnCard_Awake;
            CharacterSpawnCard characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            On.RoR2.CharacterSpawnCard.Awake -= GlobalHooks.CharacterSpawnCard_Awake;

            DirectorAPI.DirectorCardHolder directorCardHolder = new DirectorAPI.DirectorCardHolder();
            DirectorCard directorCard = new DirectorCard();



            characterSpawnCard.directorCreditCost = 330;
            characterSpawnCard.forbiddenAsBoss = false;
            characterSpawnCard.name = "cscMegaMushrum";
            //characterSpawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;
            characterSpawnCard.hullSize = HullClassification.Golem;
            characterSpawnCard.loadout = new SerializableLoadout();
            characterSpawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            characterSpawnCard.noElites = false;
            characterSpawnCard.occupyPosition = false;
            characterSpawnCard.prefab = megaMushrumMaster;
            characterSpawnCard.sendOverNetwork = true;

            directorCard.allowAmbushSpawn = true;
            directorCard.forbiddenUnlockable = "";
            directorCard.minimumStageCompletions = 4;
            directorCard.preventOverhead = false;
            directorCard.requiredUnlockable = "";
            directorCard.selectionWeight = 1;
            directorCard.spawnCard = characterSpawnCard;
            directorCard.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            directorCardHolder.Card = directorCard; 
           // directorCardHolder.InteractableCategory = DirectorAPI.InteractableCategory.None;
        //    directorCardHolder.MonsterCategory = DirectorAPI.MonsterCategory.Champions;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (!list.Contains(directorCardHolder) && stage.stage == DirectorAPI.Stage.SkyMeadow)
                {
                    list.Add(directorCardHolder);
                }
            };*/
        }   

        private void BuildBody()
        {
            CharacterBody characterBody = megaMushrum.GetComponent<CharacterBody>();
            if (characterBody)
            {
                R2API.LanguageAPI.Add("MEGAMUSHRUM_BODY_NAME", "Mega Mushrum");
                characterBody.baseAcceleration = 70f;
                characterBody.baseArmor = 0; //Base armor this character has, set to 20 if this character is melee 
                characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
                characterBody.baseCrit = 0;  //Base crit, usually one
                characterBody.baseDamage = 40; //Base damage
                characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
                characterBody.baseJumpPower = 0; //Base jump power
                characterBody.baseMaxHealth = 2100; //Base health, basically the health you have when you start a new run
                characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
                characterBody.baseMoveSpeed = 7; //Base move speed, this is usual 7
                characterBody.baseNameToken = "MEGAMUSHRUM_BODY_NAME"; //The base name token. 
                //characterBody.subtitleNameToken = "Hidden summoner"; //Set this to true if its a boss
                characterBody.baseRegen = 0; //Base health regen.
                characterBody.bodyFlags = (CharacterBody.BodyFlags.None); ///Base body flags, should be self explanatory 
                characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
                characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
                characterBody.hullClassification = HullClassification.Golem; //The hull classification, usually used for AI
                characterBody.isChampion = true; //Set this to true if its A. A boss or B. A miniboss
                characterBody.levelArmor = 0; //Armor gained when leveling up. 
                characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
                characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
                characterBody.levelDamage = 8f; //Damage gained when leveling up. 
                characterBody.levelArmor = 0; //Armor gained when leveling up. 
                characterBody.levelJumpPower = 0; //Jump power gained when leveling up. 
                characterBody.levelMaxHealth = 630; //Health gained when leveling up. 
                characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
                characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
                characterBody.levelRegen = 0f; //Regen gained when leveling up. 
                                               //characterBody.portraitIcon = portrait; //The portrait icon, shows up in multiplayer and the death UI
                                               //characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); //The pod prefab this survivor spawns in. Options: Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod"); Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod"); 
                FixSetStateOnHurt();
            }
        }

        private void SetupMegaMushrumSize()
        {
            GameObject gameObject = megaMushrum.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
            gameObject.transform.localScale = gameObject.transform.localScale * 2;
            gameObject.transform.Translate(new Vector3(0, -2f, 0));
            megaMushrum.GetComponent<CharacterBody>().aimOriginTransform.Translate(new Vector3(0, -2f, 0));

            foreach (KinematicCharacterController.KinematicCharacterMotor behaviour in megaMushrum.GetComponentsInChildren<KinematicCharacterController.KinematicCharacterMotor>())
            {
                float currentY = behaviour.Capsule.center.y;

                behaviour.SetCapsuleDimensions(behaviour.Capsule.radius * 2, behaviour.Capsule.height * 2, -0f);

            }
        }

        private void FixSetStateOnHurt()
        {
            SetStateOnHurt hurtState = megaMushrum.AddOrGetComponent<SetStateOnHurt>(); //AddOrGetComponent is because 

            hurtState.canBeFrozen = true; //If this gameobject can be frozen. Set to true if this is a survivor
            hurtState.canBeHitStunned = false; //If this gameobject can be stunned by being hit. Set to false if this is a survivor.
            hurtState.canBeStunned = false; //If this gameobject is able to be stunned. Set to false if this is a survivor
            hurtState.hitThreshold = 5f; //The hit threshold, set to 5 if this is a survivor.
        }


        private void RebuildSkillDrivers()
        {
            CloudUtils.DestroySkillDrivers(megaMushrumMaster);

            AISkillDriver SporeGrenade = megaMushrumMaster.AddComponent<AISkillDriver>();
            AISkillDriver SuicideBomb = megaMushrumMaster.AddComponent<AISkillDriver>();
            AISkillDriver Harvest = megaMushrumMaster.AddComponent<AISkillDriver>();
            AISkillDriver Flee = megaMushrumMaster.AddComponent<AISkillDriver>();
            AISkillDriver Path = megaMushrumMaster.AddComponent<AISkillDriver>();
            AISkillDriver StrafePath = megaMushrumMaster.AddComponent<AISkillDriver>();

            SporeGrenade.customName = "Spore Grenade";
            SporeGrenade.skillSlot = SkillSlot.Primary;
            SporeGrenade.requireSkillReady = true;
            SporeGrenade.requireEquipmentReady = false;
            SporeGrenade.moveTargetType = TargetType.CurrentEnemy;
            SporeGrenade.minUserHealthFraction = 0.5f;
            SporeGrenade.maxUserHealthFraction = float.PositiveInfinity;
            SporeGrenade.minTargetHealthFraction = float.NegativeInfinity;
            SporeGrenade.maxTargetHealthFraction = float.PositiveInfinity;
            SporeGrenade.minDistance = 0;
            SporeGrenade.maxDistance = 60;
            SporeGrenade.selectionRequiresTargetLoS = false;
            SporeGrenade.activationRequiresTargetLoS = true;
            SporeGrenade.activationRequiresAimConfirmation = true;
            SporeGrenade.movementType = MovementType.StrafeMovetarget;
            SporeGrenade.moveInputScale = 1;
            SporeGrenade.aimType = AimType.AtMoveTarget;
            SporeGrenade.ignoreNodeGraph = false;
            SporeGrenade.driverUpdateTimerOverride = -1;
            SporeGrenade.resetCurrentEnemyOnNextDriverSelection = false;
            SporeGrenade.noRepeat = false;
            SporeGrenade.shouldSprint = false;
            SporeGrenade.shouldFireEquipment = false;
            SporeGrenade.shouldTapButton = false;


            SuicideBomb.customName = "Suicide Bomb";
            SuicideBomb.skillSlot = SkillSlot.Secondary;
            //SpiritPull.requiredSkill =
            SuicideBomb.requireSkillReady = true;
            SuicideBomb.requireEquipmentReady = false;
            SuicideBomb.moveTargetType = TargetType.CurrentEnemy;
            SuicideBomb.minUserHealthFraction = float.NegativeInfinity;
            SuicideBomb.maxUserHealthFraction = float.PositiveInfinity;
            SuicideBomb.minTargetHealthFraction = float.NegativeInfinity;
            SuicideBomb.maxTargetHealthFraction = float.PositiveInfinity;
            SuicideBomb.minDistance = 1;
            SuicideBomb.maxDistance = 80;
            SuicideBomb.selectionRequiresTargetLoS = false;
            SuicideBomb.activationRequiresTargetLoS = false;
            SuicideBomb.activationRequiresAimConfirmation = false;
            SuicideBomb.movementType = MovementType.StrafeMovetarget;
            SuicideBomb.moveInputScale = 1;
            SuicideBomb.aimType = AimType.AtCurrentEnemy;
            SuicideBomb.ignoreNodeGraph = true;
            SuicideBomb.driverUpdateTimerOverride = -1;
            SuicideBomb.resetCurrentEnemyOnNextDriverSelection = false;
            SuicideBomb.noRepeat = false;
            SuicideBomb.shouldSprint = false;
            SuicideBomb.shouldFireEquipment = false;
            SuicideBomb.shouldTapButton = false;

            Harvest.customName = "Harvest";
            Harvest.skillSlot = SkillSlot.Utility;
            //Offspring.requiredSkill =
            Harvest.requireSkillReady = true;
            Harvest.requireEquipmentReady = false;
            Harvest.moveTargetType = TargetType.CurrentEnemy;
            Harvest.minUserHealthFraction = float.NegativeInfinity;
            Harvest.maxUserHealthFraction = 0.5f;
            Harvest.minTargetHealthFraction = float.NegativeInfinity;
            Harvest.maxTargetHealthFraction = float.PositiveInfinity;
            Harvest.minDistance = 0;
            Harvest.maxDistance = float.PositiveInfinity;
            Harvest.selectionRequiresTargetLoS = false;
            Harvest.activationRequiresTargetLoS = false;
            Harvest.activationRequiresAimConfirmation = false;
            Harvest.movementType = MovementType.Stop;
            Harvest.moveInputScale = 1;
            Harvest.aimType = AimType.None;
            Harvest.ignoreNodeGraph = false;
            Harvest.driverUpdateTimerOverride = 4;
            Harvest.resetCurrentEnemyOnNextDriverSelection = false;
            Harvest.noRepeat = false;
            Harvest.shouldSprint = false;
            Harvest.shouldFireEquipment = false;
            Harvest.shouldTapButton = false;

            Flee.customName = "Flee";
            Flee.skillSlot = SkillSlot.None;
            //Flee.requiredSkill =
            Flee.requireSkillReady = false;
            Flee.requireEquipmentReady = false;
            Flee.moveTargetType = TargetType.CurrentEnemy;
            Flee.minUserHealthFraction = float.NegativeInfinity;
            Flee.maxUserHealthFraction = 0.5f;
            Flee.minTargetHealthFraction = float.NegativeInfinity;
            Flee.maxTargetHealthFraction = float.PositiveInfinity;
            Flee.minDistance = 0;
            Flee.maxDistance = 60;
            Flee.selectionRequiresTargetLoS = false;
            Flee.activationRequiresTargetLoS = false;
            Flee.activationRequiresAimConfirmation = false;
            Flee.movementType = MovementType.FleeMoveTarget;
            Flee.moveInputScale = 1;
            Flee.aimType = AimType.AtMoveTarget;
            Flee.ignoreNodeGraph = false;
            Flee.driverUpdateTimerOverride = 3;
            Flee.resetCurrentEnemyOnNextDriverSelection = false;
            Flee.noRepeat = false;
            Flee.shouldSprint = true;
            Flee.shouldFireEquipment = false;
            Flee.shouldTapButton = false;

            Path.customName = "Path";
            Path.skillSlot = SkillSlot.None;
            //Path.requiredSkill =
            Path.requireSkillReady = false;
            Path.requireEquipmentReady = false;
            Path.moveTargetType = TargetType.CurrentEnemy;
            Path.minUserHealthFraction = float.NegativeInfinity;
            Path.maxUserHealthFraction = float.PositiveInfinity;
            Path.minTargetHealthFraction = float.NegativeInfinity;
            Path.maxTargetHealthFraction = float.PositiveInfinity;
            Path.minDistance = 0;
            Path.maxDistance = float.PositiveInfinity;
            Path.selectionRequiresTargetLoS = false;
            Path.activationRequiresTargetLoS = false;
            Path.activationRequiresAimConfirmation = false;
            Path.movementType = MovementType.ChaseMoveTarget;
            Path.moveInputScale = 1;
            Path.aimType = AimType.AtMoveTarget;
            Path.ignoreNodeGraph = true;
            Path.driverUpdateTimerOverride = -1;
            Path.resetCurrentEnemyOnNextDriverSelection = false;
            Path.noRepeat = false;
            Path.shouldSprint = false;
            Path.shouldFireEquipment = false;
            Path.shouldTapButton = false;

            StrafePath.customName = "Strafe Path";
            StrafePath.skillSlot = SkillSlot.None;
            //Path.requiredSkill =
            StrafePath.requireSkillReady = false;
            StrafePath.requireEquipmentReady = false;
            StrafePath.moveTargetType = TargetType.CurrentEnemy;
            StrafePath.minUserHealthFraction = float.NegativeInfinity;
            StrafePath.maxUserHealthFraction = float.PositiveInfinity;
            StrafePath.minTargetHealthFraction = float.NegativeInfinity;
            StrafePath.maxTargetHealthFraction = float.PositiveInfinity;
            StrafePath.minDistance = 0;
            StrafePath.maxDistance = 30;
            StrafePath.selectionRequiresTargetLoS = false;
            StrafePath.activationRequiresTargetLoS = false;
            StrafePath.activationRequiresAimConfirmation = false;
            StrafePath.movementType = MovementType.StrafeMovetarget;
            StrafePath.moveInputScale = 1;
            StrafePath.aimType = AimType.AtMoveTarget;
            StrafePath.ignoreNodeGraph = false;
            StrafePath.driverUpdateTimerOverride = -1;
            StrafePath.resetCurrentEnemyOnNextDriverSelection = false;
            StrafePath.noRepeat = false;
            StrafePath.shouldSprint = false;
            StrafePath.shouldFireEquipment = false;
            StrafePath.shouldTapButton = false;
        }


        private void RebuildSkills()
        {
            CloudUtils.CreateEmptySkills(megaMushrum);
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();
        }

        private void CreatePrimary()
        {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(SporeGrenades));

            SkillDef primarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            primarySkillDef.activationState = new SerializableEntityStateType(typeof(SporeGrenades));
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 1;
            primarySkillDef.baseRechargeInterval = 10f;
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
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Smashin));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Planting));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(Planting));
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

        private void CreateUtility()
        {
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Implant));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Planted));
            Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(Unplant));

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(Implant));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 3f;
            utilitySkillDef.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef.canceledFromSprinting = true;
            utilitySkillDef.fullRestockOnAssign = true;
            utilitySkillDef.interruptPriority = InterruptPriority.PrioritySkill;

            utilitySkillDef.isCombatSkill = false;
            utilitySkillDef.mustKeyPress = false;
            utilitySkillDef.cancelSprintingOnActivation = false;
            utilitySkillDef.rechargeStock = 1;
            utilitySkillDef.requiredStock = 1;

            utilitySkillDef.stockToConsume = 1;
            utilitySkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            utilitySkillDef.skillName = "AAAAAAAAAAAAAA";
            utilitySkillDef.skillNameToken = "AAAAAAAAAAAA";

            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(utilitySkillDef);
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,

                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)

            };
        }
        private void CreateSpecial()
        {


            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(SporeGrenades));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 12f;
            specialSkillDef.beginSkillCooldownOnSkillEnd = false;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = false;
            specialSkillDef.interruptPriority = InterruptPriority.Any;

            specialSkillDef.isCombatSkill = true;
            specialSkillDef.mustKeyPress = false;
            specialSkillDef.cancelSprintingOnActivation = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;

            specialSkillDef.stockToConsume = 1;
            specialSkillDef.skillDescriptionToken = "AAAAAAAAAAAAAAAAAAAA";
            specialSkillDef.skillName = "AAAAAAAAAAAAA";
            specialSkillDef.skillNameToken = "AAAAAAAAAAAAAA";

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