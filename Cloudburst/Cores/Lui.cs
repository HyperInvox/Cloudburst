using System;
using System.Collections.Generic;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.Components.Wyatt;
using Cloudburst.Cores.HAND.Components;
using Cloudburst.Cores.HAND.Skills;
using Cloudburst.Cores.Skills;
using Cloudburst.Cores.States.Bombardier;
using Cloudburst.Cores.States.Wyatt;

using EntityStates;

using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace Cloudburst.Cores
{
    class Lui : SurvivorCreator<Lui>
    {
        public override string SurvivorOutro => "...human life is so precious guys, there's only like 500 of us left.";

        public override string SurvivorLore => @"";

        public override string SurvivorName => "The Survivor";

        public override string SurvivorDescription => "With so many of us left, I wonder if we'll be able to win the war.";
            public override string SurvivorInternalName => "TheSurvivor";

        public override string BodyName => "Survivor";

        public override string UnlockableString => "";

        public override UnlockableDef MasteryUnlockString => null;

        public override GameObject survivorDisplay => AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdLui");//("mdLui");

        public override GameObject survivorMdl => AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdLui");//("mdLui");

        public override Color survivorDefColor => new Color(0.4862745098f, 0.94901960784f, 0.71764705882f);

        public override Sprite defaultSkinColor => Cloudburst.Content.ContentHandler.Loadouts.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));

        public override Sprite masterySkinColor => Cloudburst.Content.ContentHandler.Loadouts.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));

        public override string SurvivorSubtitle => "Lean, Mean, Cleaning Machines";

        public override UnlockableDef unlockableDef => throw new NotImplementedException();

        public override float desiredSortPosition => throw new NotImplementedException();

        public override void Hooks()
        {
            base.Hooks();
        }

        public override void GenerateUmbra()
        {
            base.GenerateUmbra();
            //umbraMaster = Resources.Load<GameObject>("prefabs/charactermasters/LoaderMonsterMaster").InstantiateClone(BodyName + "MonsterMaster", true);
            //CloudUtils.RegisterNewMaster(umbraMaster);

            //umbraMaster.GetComponent<CharacterMaster>().bodyPrefab = survivorBody;
        }
       


        public override Material GetMasteryMat()
        {
            return Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;
        }


        public override void GenerateEquipmentDisplays(List<ItemDisplayRuleSet.KeyAssetRuleGroup> obj)
        {
            base.GenerateEquipmentDisplays(obj);
        }

        public override void GenerateItemDisplays(List<ItemDisplayRuleSet.KeyAssetRuleGroup> obj)
        {
            base.GenerateItemDisplays(obj);
        }

        public override void GenerateRenderInfos(List<CharacterModel.RendererInfo> arg1, Transform arg2)
        {
            base.GenerateRenderInfos(arg1, arg2);
            var actualmdl = arg2.Find("TheSurvivor");
            var mat = actualmdl.GetComponentInChildren<SkinnedMeshRenderer>();
            arg1.Add(new CharacterModel.RendererInfo
            {
                defaultMaterial = mat.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = mat,
            });

            var cmdl = arg2.Find("oorm");
            var cmat = cmdl.GetComponentInChildren<SkinnedMeshRenderer>();
            arg1.Add(new CharacterModel.RendererInfo
            {
                defaultMaterial = cmat.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = cmat,
            });
            /*var actualmdl = arg2.Find("Bombardier");
            var mat = actualmdl.GetComponentInChildren<SkinnedMeshRenderer>();
            arg1.Add(new CharacterModel.RendererInfo
            {
                defaultMaterial = mat.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = mat,
            });*/
        }

        public override void CreateMainState(EntityStateMachine machine)
        {
            base.CreateMainState(machine);
        }

        public override void SetupCharacterBody(CharacterBody characterBody)
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
            characterBody.baseNameToken = SurvivorInternalName + "_BODY_NAME"; //The base name token. 
            characterBody.subtitleNameToken = SurvivorInternalName + "_BODY_SUBTITLE"; //Set this if its a boss
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
            characterBody.portraitIcon = AssetsCore.mainAssetBundle.LoadAsset<Texture>("BombardierPortrait"); //The portrait icon, shows up in multiplayer and the death UI
            characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/SurvivorPod");
        }

        public override void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
        {

            SteppedSkillDef primarySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();

            primarySkillDef.activationState = new SerializableEntityStateType(typeof(Uninitialized));
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
            primarySkillDef.skillDescriptionToken = "WYATT_PRIMARY_DESCRIPTION";
            primarySkillDef.skillName = "WYATT_PRIMARY_NAME";
            primarySkillDef.skillNameToken = "WYATT_PRIMARY_NAME";
            primarySkillDef.icon = AssetsCore.wyattPrimary;
            primarySkillDef.keywordTokens = new string[] {
                 "KEYWORD_AGILE",
                 "KEYWORD_WEIGHTLESS",
                 "KEYWORD_SPIKED",
            };

            R2API.LanguageAPI.Add(primarySkillDef.skillNameToken, "G22 Grav-Broom");
            R2API.LanguageAPI.Add(primarySkillDef.skillDescriptionToken, "<style=cIsUtility>Agile</style>. Swing in front for X% damage. [NOT IMPLEMENTED] Every 4th hit <style=cIsDamage>Spikes</style>.");
            //R2API.LanguageAPI.Add(primarySkillDef.keywordTokens[1], "<style=cKeywordName>Weightless</style><style=cSub>Slows and removes gravity from target.</style>");
            R2API.LanguageAPI.Add(primarySkillDef.keywordTokens[2], "<style=cKeywordName>Spikes</style><style=cSub>Knocks an enemy directly toward the ground at dangerous speeds.</style>");

            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(primarySkillDef);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primarySkillDef,
                viewableNode = new ViewablesCatalog.Node(primarySkillDef.skillNameToken, false, null)

            };
        }
        public override void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
        {

            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut));
            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut2));
            //Cloudburst.Content.ContentHandler.Loadouts.RegisterEntityState(typeof(TrashOut3));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(Uninitialized));
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
        public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
        {

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(Uninitialized));
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
            utilitySkillDef2.isCombatSkill = true;
            utilitySkillDef2.mustKeyPress = false;
            utilitySkillDef2.cancelSprintingOnActivation = false;
            utilitySkillDef2.rechargeStock = 1;
            utilitySkillDef2.requiredStock = 1;
            utilitySkillDef2.stockToConsume = 1;
            utilitySkillDef2.skillDescriptionToken = "WYATT_UTILITY2_DESCRIPTION";
            utilitySkillDef2.skillName = "aaa";
            utilitySkillDef2.skillNameToken = "WYATT_UTILITY2_NAME";
            utilitySkillDef2.icon = AssetsCore.wyattUtilityAlt;
            utilitySkillDef2.keywordTokens = Array.Empty<string>();

            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,
                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)
            };
        }
        public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            specialSkillDef.activationState = new SerializableEntityStateType(typeof(Uninitialized));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 0;
            specialSkillDef.beginSkillCooldownOnSkillEnd = true;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = true;
            specialSkillDef.interruptPriority = InterruptPriority.PrioritySkill;
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

            Cloudburst.Content.ContentHandler.Loadouts.RegisterSkillDef(specialSkillDef);

            skillLocator.special.skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,
                viewableNode = new ViewablesCatalog.Node(specialSkillDef.skillNameToken, false, null)
            };
        }

        protected override void Initialization()
        {
            SfxLocator sfxLocator = survivorBody.GetComponent<SfxLocator>();

            //sfx
            sfxLocator.fallDamageSound = "Play_MULT_shift_hit";
            sfxLocator.landingSound = "play_char_land";

        }
    }
}
