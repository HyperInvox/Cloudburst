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
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace Cloudburst.Cores
{
    class Custodian : SurvivorCreator<Custodian>
    {
        public override string SurvivorOutro => "...and so they left, a job well done";

        public override string SurvivorLore => @"Can't stop now. Can't stop now. Every step I take is a step I can't take back. Come hell or high water I will find it.

It's all a rhythm, just a rhythm. Every time I step out of line is a punishment. I will obey the groove. Nothing can stop me now.

Every scar is worth it. I can feel it, I am coming closer to it. I will have it, and it will be mine.

No matter the blood, it's worth it. I will find what I want, and I will come home.

No matter how many I slaughter, it will be mine... It can't hide from me from me forever.

It's here, I can feel it. This security chest, it has it. I crack it open, and I find it.

It's... finally mine. I hold it in my bruised hands. Has it really been years? 

She'll love this, I know.

";

        public override string SurvivorName => "Custodian";

        public override string SurvivorDescription => "The Custodian is a master of janitorial warfare who uses his MAID to control the battle field<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
        + "< ! > Send enemies upwards with the MAID, and spike them downwads with Trash Out for major damage." + Environment.NewLine + Environment.NewLine
      + "< ! > The MAID slows projectiles within her radius, use this to your advantage in combat!" + Environment.NewLine + Environment.NewLine
+ "< ! > Direct strikes with Rub and Scrub help you stay in the air longer -- use this to avoid crowds!" + Environment.NewLine + Environment.NewLine
        + "< ! > The key to success is realizing that staying away from the ground helps you stay alive longer." + Environment.NewLine + Environment.NewLine;

        public override string SurvivorInternalName => "WYATT";

        public override string BodyName => "Custodian";

        public override string UnlockableString => "";

        public override string MasteryUnlockString => "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_REWARD_ID";

        public override GameObject survivorDisplay => AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlWyattCSS");

        public override GameObject survivorMdl => AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlWyatt");

        public override Color survivorDefColor => new Color(0.4862745098f, 0.94901960784f, 0.71764705882f);

        public override Sprite defaultSkinColor => LoadoutAPI.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));

        public override Sprite masterySkinColor => LoadoutAPI.CreateSkinIcon(CloudUtils.HexToColor("00A86B"), CloudUtils.HexToColor("E56717"), CloudUtils.HexToColor("D9DDDC"), CloudUtils.HexToColor("43464B"));

        public override string SurvivorSubtitle => "Lean, Mean, Cleaning Machines";

        public override void Hooks()
        {
            base.Hooks();
            On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += SlowDownProjectiles_OnTriggerEnter;
        }

        public override void GenerateUmbra()
        {
            base.GenerateUmbra();
            umbraMaster = Resources.Load<GameObject>("prefabs/charactermasters/LoaderMonsterMaster").InstantiateClone(BodyName + "MonsterMaster", true);
            CloudUtils.RegisterNewMaster(umbraMaster);

            umbraMaster.GetComponent<CharacterMaster>().bodyPrefab = survivorBody;
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
                if (con.teamFilter && con.owner && con.owner.GetComponent<CharacterBody>() && con.owner.GetComponent<CharacterBody>().teamComponent.teamIndex == con.teamFilter.teamIndex)
                {
                    shouldOrigSelf = false;
                }
            }

            if (shouldOrigSelf)
            {
                EffectManager.SpawnEffect(EffectCore.maidCleanseEffect, new EffectData()
                {
                    origin = other.transform.position,
                    scale = 10,
                    rotation = Quaternion.identity,
                }, false);
                orig(self, other);
            }
        }


        public override Material GetMasteryMat()
        {
            return Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;
        }

        public static SkillDef retrievePrimary;
        public static SkillDef throwPrimary;

        public override void GenerateEquipmentDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
        {
            base.GenerateEquipmentDisplays(obj);
            /*childName = "UpperBody",
localPos = new Vector3(0.0046F, 0.0014F, -0.0006F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Jetpack", "DisplayBugWings", "UpperBody", new Vector3(0.0046F, 0.0014F, -0.0006F), new Vector3(0, 270, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "UpperBody",
localPos = new Vector3(0F, 0.0187F, 0.0101F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("GoldGat", "DisplayGoldGat", "UpperBody", new Vector3(0F, 0.0187F, 0.0101F), new Vector3(0, 0, 0), new Vector3(0.005F, 0.005F, 0.005F)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("BFG", "DisplayBFG", "UpperBody", new Vector3(0, 0.012f, -0.006f), new Vector3(15, 270, 25), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.006F, 0.0058F, -0.0014F),
localAngles = new Vector3(357.2348F, 86.3632F, 7.6717F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("QuestVolatileBattery", "DisplayBatteryArray", "UpperBody", new Vector3(0.006F, 0.0058F, -0.0014F), new Vector3(357.2348F, 86.3632F, 7.6717F), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0063F, 0.0102F, -0.0009F),
localAngles = new Vector3(61.5214F, 78.6229F, 348.4264F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("CommandMissile", "DisplayMissileRack", "UpperBody", new Vector3(0.0063F, 0.0102F, -0.0009F), new Vector3(61.5214F, 78.6229F, 348.4264F), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "LowerBody",
localPos = new Vector3(-0.0042F, -0.0166F, -0.0097F),
localAngles = new Vector3(3.2258F, 25.4977F, 22.0889F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Fruit", "DisplayFruit", "LowerBody", new Vector3(-0.0042F, -0.0166F, -0.0097F), new Vector3(3.2258F, 25.4977F, 22.0889F), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "Head",
localPos = new Vector3(0.0098F, 0.0012F, -0.0002F),
localAngles = new Vector3(9.6844F, 91.5572F, 181.5758F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("AffixWhite", "DisplayEliteIceCrown", "Head", new Vector3(0.0098F, 0.0012F, -0.0002F), new Vector3(9.6844F, 91.5572F, 181.5758F), new Vector3(0.001f, 0.001f, 0.001f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("AffixPoison", "DisplayEliteUrchinCrown", "Head", new Vector3(0.0098F, 0.0012F, -0.0002F),new Vector3(9.6844F, 91.5572F, 181.5758F), new Vector3(0.001f, 0.001f, 0.001f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("AffixHaunted", "DisplayEliteStealthCrown", "Head", new Vector3(0.0098F, 0.0012F, -0.0002F), new Vector3(9.6844F, 91.5572F, 181.5758F), new Vector3(0.001f, 0.001f, 0.001f)));
            /*childName = "Head",
localPos = new Vector3(0.0043F, 0.0071F, 0F),
localAngles = new Vector3(84.2833F, 103.373F, 12.1276F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("CritOnUse", "DisplayNeuralImplant", "Head", new Vector3(0.0043F, 0.0071F, 0F), new Vector3(84.2833F, 103.373F, 12.1276F), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("DroneBackup", "DisplayRadio", "Pelvis", new Vector3(0, 0, 0.008f), new Vector3(0, 0, 180), new Vector3(0.01f, 0.01f, 0.01f)));
            //obj.Add(CloudUtils.CreateGenericDisplayRule("Lightning", ItemDisplays.capacitorPrefab, "ClavicleL", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.04f, 0.04f, 0.04f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("BurnNearby", "DisplayPotion", "Pelvis", new Vector3(0, 0, 0.008f), new Vector3(0, 0, 180), new Vector3(0.002f, 0.002f, 0.002f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("CrippleWard", "DisplayEffigy", "Pelvis", new Vector3(0, 0.008f, 0.009f), new Vector3(0, 180, 180), new Vector3(0.02f, 0.02f, 0.02f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("GainArmor", "DisplayElephantFigure", "UpperLeg.R", new Vector3(0.004f, 0.012f, 0), new Vector3(90, 90, 0), new Vector3(0.02f, 0.02f, 0.02f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Recycle", "DisplayRecycler", "UpperBody", new Vector3(0.012f, 0.012f, 0), new Vector3(0, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("FireBallDash", "DisplayEgg", "Pelvis", new Vector3(0, 0, 0.008f), new Vector3(90, 0, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Cleanse", "DisplayWaterPack", "UpperBody", new Vector3(0.012f, 0.01f, 0), new Vector3(315, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Tonic", "DisplayTonic", "Pelvis", new Vector3(0, 0, 0.008f), new Vector3(0, 0, 180), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Gateway", "DisplayVase", "Pelvis", new Vector3(0, 0, 0.009f), new Vector3(0, 0, 180), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Scanner", "DisplayScanner", "Pelvis", new Vector3(0, 0.005f, 0.008f), new Vector3(90, 270, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("DeathProjectile", "DisplayDeathProjectile", "Pelvis", new Vector3(-0.0012f, 0.005f, 0), new Vector3(0, 270, 180), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "Head",
localPos = new Vector3(0.0034F, 0.0079F, 0.002F),
localAngles = new Vector3(45F, 180F, 0F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("LifestealOnHit", "DisplayLifestealOnHit", "Head", new Vector3(0.0034F, 0.0079F, 0.002F), new Vector3(45F, 180F, 0F), new Vector3(0.005f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("TeamWarCry", "DisplayTeamWarCry", "Pelvis", new Vector3(0.01f, 0.003f, 0), new Vector3(10, 90, 0), new Vector3(0.0035f, 0.0035f, 0.0035f)));

            obj.Add(CloudUtils.CreateFollowerDisplayRule("Saw", "DisplaySawmerang", new Vector3(0.06f, 0.02f, 0), new Vector3(90, 0, 0), new Vector3(0.25f, 0.25f, 0.25f)));
            obj.Add(CloudUtils.CreateFollowerDisplayRule("Meteor", "DisplayMeteor", new Vector3(0.05f, 0.02f, 0), new Vector3(90, 0, 0), new Vector3(1, 1, 1)));
            obj.Add(CloudUtils.CreateFollowerDisplayRule("Blackhole", "DisplayGravCube", new Vector3(0.05f, 0.02f, 0), new Vector3(90, 0, 0), new Vector3(1, 1, 1)));

        }

        public override void GenerateItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
        {
            base.GenerateItemDisplays(obj);

            /*childName = "Head",
localPos = new Vector3(0.0055F, 0.0048F, -0.0001F),
localAngles = new Vector3(272.0249F, 67.564F, 203.6608F),
localScale = new Vector3(0.01F, 0.01F, 0.0105F)*/

            /*done*/obj.Add(CloudUtils.CreateGenericDisplayRule("CritGlasses", "DisplayGlasses", "Head", new Vector3(0.0055F, 0.0048F, -0.0001F), new Vector3(272.0249F, 67.564F, 203.6608F), new Vector3(0.01F, 0.01F, 0.0105F)));
            /*done*/ obj.Add(CloudUtils.CreateGenericDisplayRule("Syringe", "DisplaySyringeCluster", "LowerBody", new Vector3(-0.0001F, 0.0007F, 0.0052F), new Vector3(65.2608F, 238.0223F, 263.2361F), new Vector3(0.005F, 0.005F, 0.005F)));
            /*childName = "Broom3",
 localPos = new Vector3(0.0003F, -0.0182F, 0.0001F),
 localAngles = new Vector3(0F, 0F, 0F),
 localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("NearbyDamageBonus", "DisplayDiamond", "Broom3", new Vector3(0.0003F, -0.0182F, 0.0001F), new Vector3(0, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "Broom3",
localPos = new Vector3(0.0023F, 0.0571F, 0.0006F),
localAngles = new Vector3(274.0766F, 66.9896F, 295.4782F),
localScale = new Vector3(0.0175F, 0.0175F, 0.0175F)*/
            /*odd*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("ArmorReductionOnHit", "DisplayWarhammer", "Broom3", new Vector3(0.0023F, 0.0571F, 0.0006F), new Vector3(274.0766F, 66.9896F, 295.4782F), new Vector3(0.0175F, 0.0175F, 0.0175F)));
            /*childName = "Broom3",
localPos = new Vector3(0.0009F, 0F, 0.0012F),
localAngles = new Vector3(70F, 0F, 180F),
localScale = new Vector3(0.003F, 0.003F, 0.003F)*/
            /*weird*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("SecondarySkillMagazine", "DisplayDoubleMag", "Broom3", new Vector3(0.0009F, 0F, 0.0012F), new Vector3(70, 0, 180), new Vector3(0.003F, 0.003F, 0.003F)));
            /*childName = "Zipper",
localPos = new Vector3(-0.0002F, 0.002F, 0F),
localAngles = new Vector3(351.1161F, 94.042F, 174.5225F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Bear", "DisplayBear", "Zipper", new Vector3(-0.0002F, 0.002F, 0F), new Vector3(351.1161F, 94.042F, 174.5225F), new Vector3(0.01F, 0.01F, 0.01F)));
            /*childName = "Pelvis",
localPos = new Vector3(0.0005F, 0.0032F, 0.0086F),
localAngles = new Vector3(18.9127F, 281.0396F, 184.2924F),
localScale = new Vector3(0.0175F, 0.0175F, 0.0175F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("SprintOutOfCombat", "DisplayWhip", "Pelvis", new Vector3(0.0005F, 0.0032F, 0.0086F), new Vector3(18.9127F, 281.0396F, 184.2924F), new Vector3(0.0175F, 0.0175F, 0.0175F)));
            /*childName = "Pelvis",
localPos = new Vector3(-0.0002F, 0.0024F, -0.0076F),
localAngles = new Vector3(77.981F, 226.2504F, 225.5097F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("PersonalShield", "DisplayShieldGenerator", "Pelvis", new Vector3(-0.0002F, 0.0024F, -0.0076F), new Vector3(77.981F, 226.2504F, 225.5097F), new Vector3(0.005F, 0.005F, 0.005F)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0035F, 0.0093F, -0.0117F),
localAngles = new Vector3(301.4073F, 197.9899F, 238.5406F),
localScale = new Vector3(0.0075F, 0.0075F, 0.0075F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("RegenOnKill", "DisplaySteakCurved", "UpperBody", new Vector3(-0.0035F, 0.0093F, -0.0117F), new Vector3(301.4073F, 197.9899F, 238.5406F), new Vector3(0.0075F, 0.0075F, 0.0075F)));
            /*childName = "Pelvis",
localPos = new Vector3(-0.0014F, 0.0042F, 0.0004F),
localAngles = new Vector3(354.92F, 99.0354F, 0.9559F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("FireballsOnHit", "DisplayFireballsOnHit", "Pelvis", new Vector3(-0.0014F, 0.0042F, 0.0004F), new Vector3(354.92F, 99.0354F, 0.9559F), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "LowerLeg.L",
localPos = new Vector3(-0.0019F, 0.0119F, 0.0005F),
localAngles = new Vector3(76.5328F, 91.3411F, 1.3874F),
localScale = new Vector3(0.006F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Hoof", "DisplayHoof", "LowerLeg.L", new Vector3(-0.0019F, 0.0119F, 0.0005F), new Vector3(76.5328F, 91.3411F, 1.3874F), new Vector3(0.006F, 0.005F, 0.005F)));
            //done
            obj.Add(CloudUtils.CreateGenericDisplayRule("WardOnLevel", "DisplayWarbanner", "UpperBody", new Vector3(0.0049F, 0.0054F, 0.0008F), new Vector3(0F, 90F, 90F), new Vector3(0.0175f, 0.0175f, 0.0175f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0049F, 0.0054F, 0.0008F),
localAngles = new Vector3(0F, 90F, 90F),
localScale = new Vector3(0.0175F, 0.0175F, 0.0175F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("BarrierOnOverHeal", "DisplayAegis", "LowerArm.R", new Vector3(-0.002f, -0.005f, 0), new Vector3(90, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("WarCryOnMultiKill", "DisplayPauldron", "UpperBody", new Vector3(0, 0.008f, -0.008f), new Vector3(60, 180, 0), new Vector3(0.03f, 0.03f, 0.03f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("SprintArmor", "DisplayBuckler", "LowerArm.L", new Vector3(0.002f, 0.005f, 0), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "GyroRing",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.02F, 0.0175F, 0.0175F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("IceRing", "DisplayIceRing", "GyroRing", new Vector3(0, 0, 0), new Vector3(270, 90, 0), new Vector3(0.02f, 0.0175f, 0.0175f)));
            /*childName = "GyroBall",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.02F, 0.0175F, 0.0175F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("FireRing", "DisplayFireRing", "GyroBall", new Vector3(0, 0, 0), new Vector3(270, 90, 0), new Vector3(0.02f, 0.0175f, 0.0175f)));
            /*childName = "Broom3",
localPos = new Vector3(0F, -0.0514F, 0.0004F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.003F, 0.004F, 0.003F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Behemoth", "DisplayBehemoth", "Broom3", new Vector3(0F, -0.0514F, 0.0004F), new Vector3(0, 0, 0), new Vector3(0.003F, 0.004F, 0.003F)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0016F, 0.0146F, 0.0144F),
localAngles = new Vector3(4.3116F, 270.9612F, 339.7851F),
localScale = new Vector3(0.002F, 0.002F, 0.002F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Missile", "DisplayMissileLauncher", "UpperBody", new Vector3(-0.0016F, 0.0146F, 0.0144F), new Vector3(4.3116F, 270.9612F, 339.7851F), new Vector3(0.002F, 0.002F, 0.002F)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0003F, 0.0076F, 0.0092F),
localAngles = new Vector3(320.7576F, 68.9507F, 138.2797F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Dagger", "DisplayDagger", "UpperBody", new Vector3(0.0003F, 0.0076F, 0.0092F), new Vector3(320.7576F, 68.9507F, 138.2797F), new Vector3(0.03F, 0.03F, 0.03F)));
            /*childName = "Broom3",
localPos = new Vector3(0.0021F, 0.0167F, -0.0024F),
localAngles = new Vector3(2.0048F, 359.2651F, 178.9463F),
localScale = new Vector3(0.09F, 0.09F, 0.09F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("ChainLightning", "DisplayUkulele", "Broom3", new Vector3(0.0021F, 0.0167F, -0.0024F), new Vector3(2.0048F, 359.2651F, 178.9463F), new Vector3(0.09F, 0.09F, 0.09F)));
            /*childName = "Head",
localPos = new Vector3(0.0047F, 0.0023F, 0.0001F),
localAngles = new Vector3(271.8949F, 45.2566F, 226.0727F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("GhostOnKill", "DisplayMask", "Head", new Vector3(0.0047F, 0.0023F, 0.0001F), new Vector3(271.8949F, 45.2566F, 226.0727F), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0022F, 0.0081F, 0.0117F),
localAngles = new Vector3(318.3856F, 115.1429F, 342.3295F),
localScale = new Vector3(0.0035F, 0.0035F, 0.0035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Mushroom", "DisplayMushroom", "UpperBody", new Vector3(-0.0022F, 0.0081F, 0.0117F), new Vector3(318.3856F, 115.1429F, 342.3295F), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            /*childName = "Head",
localPos = new Vector3(0.0091F, 0.0027F, -0.0001F),
localAngles = new Vector3(283.0408F, 206.9095F, 77.9315F),
localScale = new Vector3(0.015F, 0.015F, 0.015F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("AttackSpeedOnCrit", "DisplayWolfPelt", "Head", new Vector3(0.0091F, 0.0027F, -0.0001F), new Vector3(283.0408F, 206.9095F, 77.9315F), new Vector3(0.015f, 0.015f, 0.015f)));
            /*childName = "GyroRing",
localPos = new Vector3(-0.0087F, -0.0098F, -0.0014F),
localAngles = new Vector3(56.7247F, 251.181F, 42.4476F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("BleedOnHit", "DisplayTriTip", "GyroRing", new Vector3(-0.0087F, -0.0098F, -0.0014F), new Vector3(56.7247F, 251.181F, 42.4476F), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "Hand.R",
localPos = new Vector3(-0.0002F, 0.0037F, -0.0025F),
localAngles = new Vector3(295.7448F, 78.7334F, 356.259F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("HealOnCrit", "DisplayScythe", "Hand.R", new Vector3(-0.0002F, 0.0037F, -0.0025F), new Vector3(295.7448F, 78.7334F, 356.259F), new Vector3(0.01F, 0.01F, 0.01F)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0002F, 0.0074F, 0.0061F),
localAngles = ,
localScale = new Vector3(0.0035F, 0.0035F, 0.0035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("HealWhileSafe", "DisplaySnail", "UpperBody", new Vector3(-0.0002F, 0.0074F, 0.0061F), new Vector3(20.4601F, 281.2201F, 13.6358F), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            /*childName = "Pelvis",
localPos = new Vector3(0.0038F, 0.0046F, -0.0005F),
localAngles = new Vector3(280.7458F, 338.3366F, 308.2952F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Clover", "DisplayClover", "Pelvis", new Vector3(0.0038F, 0.0046F, -0.0005F), new Vector3(280.7458F, 338.3366F, 308.2952F), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "Head",
localPos = new Vector3(0.0078F, 0.0016F, 0F),
localAngles = new Vector3(287.898F, 274.1387F, 356.9723F),
localScale = new Vector3(0.035F, 0.035F, 0.035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("GoldOnHit", "DisplayBoneCrown", "Head", new Vector3(0.0078F, 0.0016F, 0F), new Vector3(287.898F, 274.1387F, 356.9723F), new Vector3(0.035f, 0.035f, 0.035f)));
            /*childName = "Head",
localPos = new Vector3(-0.0045F, -0.0013F, 0.0002F),
localAngles = new Vector3(274.3626F, 72.4662F, 198.8793F),
localScale = new Vector3(0.025F, 0.025F, 0.025F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("JumpBoost", "DisplayWaxBird", "Head", new Vector3(-0.0045F, -0.0013F, 0.0002F), new Vector3(274.3626F, 72.4662F, 198.8793F), new Vector3(0.025f, 0.025f, 0.025f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("ArmorPlate", "DisplayRepulsionArmorPlate", "UpperLeg.L", new Vector3(-0.003f, 0.0075f, 0.001f), new Vector3(90, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Feather", "DisplayFeather", "LowerArm.L", new Vector3(0, 0.008f, 0), new Vector3(0, 0, 270), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0136F, 0.0047F, -0.0092F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.02F, 0.0175F, 0.0175F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Crowbar", "DisplayCrowbar", "UpperBody", new Vector3(0.0136F, 0.0047F, -0.0092F), new Vector3(270F, 90F, 0F), new Vector3(0.02f, 0.0175f, 0.0175f)));
            /*childName = "",
localPos = new Vector3(0.0009F, -0.004F, -0.0001F),
localAngles = new Vector3(72.3721F, 114.4432F, 31.4792F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("ExecuteLowHealthElite", "DisplayGuillotine", "Head", new Vector3(0.0009F, -0.004F, -0.0001F), new Vector3(72.3721F, 114.4432F, 31.4792F), new Vector3(0.01F, 0.01F, 0.01F)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0067F, 0.0007F, -0.0007F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("EquipmentMagazine", "DisplayBattery", "UpperBody", new Vector3(0.0067F, 0.0007F, -0.0007F) , new Vector3(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Infusion", "DisplayInfusion", "Pelvis", new Vector3(0, 0.002f, 0.008f), new Vector3(0, 0, 0), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0059F, 0.0084F, -0.0009F),
localAngles = new Vector3(282.3817F, 230.6585F, 217.5494F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Medkit", "DisplayMedkit", "UpperBody", new Vector3(0.0059F, 0.0084F, -0.0009F), new Vector3(282.3817F, 230.6585F, 217.5494F), new Vector3(0.02f, 0.02f, 0.02f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Bandolier", "DisplayBandolier", "UpperBody", new Vector3(0, 0.005f, 0), new Vector3(315, 0, 180), new Vector3(0.02f, 0.03f, 0.03f)));

            obj.Add(CloudUtils.CreateGenericDisplayRule("BounceNearby", "DisplayHook", "UpperBody", new Vector3(0, 0.002f, 0), new Vector3(270, 90, 0), new Vector3(0.02f, 0.02f, 0.02f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("StunChanceOnHit", "DisplayStunGrenade", "UpperLeg.R", new Vector3(-0.005f, 0.01f, 0), new Vector3(90, 270, 0), new Vector3(0.03f, 0.03f, 0.03f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("IgniteOnKill", "DisplayGasoline", "Pelvis", new Vector3(0, 0.006f, 0.01f), new Vector3(70, 180, 90), new Vector3(0.02f, 0.02f, 0.02f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Firework", "DisplayFirework", "Pelvis", new Vector3(0, 0.002f, 0.01f), new Vector3(90, 0, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("LunarDagger", "DisplayLunarDagger", "UpperBody", new Vector3(0.01f, 0.002f, 0), new Vector3(290, 90, 0), new Vector3(0.03f, 0.03f, 0.03f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0018F, -0.0118F, -0.0011F),
localAngles = new Vector3(4.2873F, 315.2964F, 6.5038F),
localScale = new Vector3(0.0035F, 0.0035F, 0.0035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Knurl", "DisplayKnurl", "UpperBody", new Vector3(0.0018F, -0.0118F, -0.0011F), new Vector3(4.2873F, 315.2964F, 6.5038F), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            /*childName = "UpperBody",
localPos = new Vector3(0.0014F, 0.0092F, -0.0077F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.0035F, 0.0035F, 0.0035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("BeetleGland", "DisplayBeetleGland", "UpperBody", new Vector3(0.0014F, 0.0092F, -0.0077F)   , new Vector3(0, 270, 0), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("SprintBonus", "DisplaySoda", "Pelvis", new Vector3(0.004f, 0.002f, -0.005f), new Vector3(270, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("StickyBomb", "DisplayStickyBomb", "Pelvis", new Vector3(0.0025f, 0.002f, -0.008f), new Vector3(345, 15, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("TreasureCache", "DisplayKey", "Pelvis", new Vector3(0.006f, 0.002f, -0.003f), new Vector3(0, 25, 270), new Vector3(0.03f, 0.03f, 0.03f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("BossDamageBonus", "DisplayAPRound", "Pelvis", new Vector3(0, 0, 0.008f), new Vector3(90, 0, 0), new Vector3(0.02f, 0.02F, 0.02F)));
            /*childName = "Zipper",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(8.2202F, 102.353F, 176.4449F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("ExtraLife", "DisplayHippo", "Zipper", new Vector3(0F, 0F, 0F), new Vector3(8.2202F, 102.353F, 176.4449F), new Vector3(0.01F, 0.01F, 0.01F)));
            /*childName = "Head",
localPos = new Vector3(0.0098F, 0.0001F, 0F),
localAngles = new Vector3(59.5134F, 92.2518F, 1.2979F),
localScale = new Vector3(0.006F, 0.006F, 0.006F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("KillEliteFrenzy", "DisplayBrainstalk", "Head", new Vector3(0.0098F, 0.0001F, 0F), new Vector3(59.5134F, 92.2518F, 1.2979F), new Vector3(0.006f, 0.006f, 0.006f)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0051F, 0.0091F, -0.0041F),    
localAngles = new Vector3(343.3631F, 350.4558F, 102.6907F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("RepeatHeal", "DisplayCorpseFlower", "UpperBody", new Vector3(-0.0051F, 0.0091F, -0.0041F), new Vector3(343.3631F, 350.4558F, 102.6907F), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0056F, -0.0027F, 0.0036F),
localAngles = new Vector3(10.3942F, 4.0661F, 31.505F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("AutoCastEquipment", "DisplayFossil", "UpperBody", new Vector3(-0.0056F, -0.0027F, 0.0036F), new Vector3(10.3942F, 4.0661F, 31.505F), new Vector3(0.02f, 0.02f, 0.02f)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0068F, 0.0039F, 0.0009F),
localAngles = new Vector3(0F, 235F, 0F),
localScale = new Vector3(0.0085F, 0.0085F, 0.0085F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("TitanGoldDuringTP", "DisplayGoldHeart", "UpperBody", new Vector3(-0.0068F, 0.0039F, 0.0009F), new Vector3(0, 235, 0), new Vector3(0.0085f, 0.0085f, 0.0085f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("SprintWisp", "DisplayBrokenMask", "ShoulderL", new Vector3(0.005f, 0.003f, 0), new Vector3(0, 90, 180), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "UpperBody",
localPos = new Vector3(-0.0055F, 0.0053F, 0.0039F),
localAngles = new Vector3(89.1739F, 348.8374F, 68.889F),
localScale = new Vector3(0.015F, 0.015F, 0.015F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("BarrierOnKill", "DisplayBrooch", "UpperBody", new Vector3(-0.0055F, 0.0053F, 0.0039F), new Vector3(89.1739F, 348.8374F, 68.889F), new Vector3(0.015f, 0.015f, 0.015f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("TPHealingNova", "DisplayGlowFlower", "UpperBody", new Vector3(-0.0055f, 0.003f, -0.004f), new Vector3(0, 250, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            //childName = "Pelvis",
            //            localPos = new Vector3(0.0071F, 0.0051F, -0.0029F),
            //localAngles = new Vector3(0F, 180F, 0F),
            //localScale = new Vector3(0.02F, 0.02F, 0.02F)
            obj.Add(CloudUtils.CreateGenericDisplayRule("LunarUtilityReplacement", "DisplayBirdFoot", "Pelvis", new Vector3(0.0071F, 0.0051F, -0.0029F), new Vector3(0F, 180F, 0F), new Vector3(0.02F, 0.02F, 0.02F)));

            obj.Add(CloudUtils.CreateGenericDisplayRule("Thorns", "DisplayRazorwireLeft", "Broom3", new Vector3(0, 0, 0), new Vector3(270, 0, 0), new Vector3(0.02f, 0.02f, 0.025f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("LunarPrimaryReplacement", "DisplayBirdEye", "Head", new Vector3(-0.003f, 0.0025f, 0), new Vector3(0, 0, 270), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("NovaOnLowHealth", "DisplayJellyGuts", "UpperBody", new Vector3(0, 0.011f, 0), new Vector3(310, 270, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("LunarTrinket", "DisplayBeads", "LowerArm.L", new Vector3(0.002f, 0.004f, 0.0015f), new Vector3(0, 90, 90), new Vector3(0.04f, 0.04f, 0.04f)));
            /*childName = "Head",
localPos = new Vector3(0.0023F, 0.0006F, 0F),
localAngles = new Vector3(284.9118F, 254.7191F, 280.8911F),
localScale = new Vector3(0.004F, 0.004F, 0.004F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Plant", "DisplayInterstellarDeskPlant", "Head", new Vector3(0.0023F, 0.0006F, 0F), new Vector3(284.9118F, 254.7191F, 280.8911F), new Vector3(0.004F, 0.004F, 0.004F)));
            /*childName = "Head",
localPos = new Vector3(0.0078F, 0.0011F, 0F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.002F, 0.002F, 0.002F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("DeathMark", "DisplayDeathMark", "Head", new Vector3(0.0078F, 0.0011F, 0F), new Vector3(270, 90, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("CooldownOnCrit", "DisplaySkull", "Hand.R", new Vector3(0, 0.005f, 0), new Vector3(270, 90, 0), new Vector3(0.0085f, 0.0085f, 0.01f)));
            /*childName = "Broom3",
localPos = new Vector3(-0.0002F, -0.0465F, 0.0003F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.035F, 0.035F, 0.035F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Broom3", new Vector3(-0.0002F, -0.0465F, 0.0003F), new Vector3(0F, 0F, 0F), new Vector3(0.035f, 0.035f, 0.035f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("ExplodeOnDeath", "DisplayWilloWisp", "Pelvis", new Vector3(0, 0, -0.01f), new Vector3(0, 0, 180), new Vector3(0.0025f, 0.0025f, 0.0025f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Seed", "DisplaySeed", "LowerArm.R", new Vector3(0, 0.005f, 0), new Vector3(270, 0, 0), new Vector3(0.0025f, 0.0025f, 0.0025f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Phasing", "DisplayStealthkit", "UpperLeg.L", new Vector3(-0.004f, 0.01f, 0), new Vector3(90, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("ShockNearby", "DisplayTeslaCoil", "UpperBody", new Vector3(0.01f, 0.01f, 0), new Vector3(0, 0, 315), new Vector3(0.01f, 0.01f, 0.01f)));
            /*childName = "Hand.R",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("AlienHead", "DisplayAlienHead", "Hand.R", new Vector3(0F, 0F, 0F), new Vector3(0F, 0F, 0F) , new Vector3(0.05f, 0.05f, 0.05f)));
            /*childName = "Head",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0.1132F, 359.4611F, 294.4649F),
localScale = new Vector3(0.015F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("HeadHunter", "DisplaySkullCrown", "Head", new Vector3(0F, 0F, 0F), new Vector3(0.1132F, 359.4611F, 294.4649F), new Vector3(0.015f, 0.005f, 0.005f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("EnergizedOnEquipmentUse", "DisplayWarHorn", "Pelvis", new Vector3(0.006f, 0.002f, 0.006f), new Vector3(0, 190, 90), new Vector3(0.015f, 0.015f, 0.015f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Tooth", "DisplayToothMeshLarge", "UpperBody", new Vector3(0, 0.012f, 0.005f), new Vector3(290, 0, 0), new Vector3(0.3f, 0.3f, 0.3f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Pearl", "DisplayPearl", "Head", new Vector3(0.0082F, 0.0018F, 0F), new Vector3(346.5741F, 98.3489F, 347.4611F), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "Head",
localPos = new Vector3(0.0082F, 0.0018F, 0F),
localAngles = new Vector3(346.5741F, 98.3489F, 347.4611F),
localScale = new Vector3(0.005F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("ShinyPearl", "DisplayShinyPearl", "Head", new Vector3(0.0082F, 0.0018F, 0F), new Vector3(346.5741F, 98.3489F, 347.4611F), new Vector3(0.005f, 0.005f, 0.005f)));
            /*childName = "Pelvis",
localPos = new Vector3(-0.0051F, -0.0004F, -0.0067F),
localAngles = new Vector3(7.9428F, 200.4164F, 14.3652F),
localScale = new Vector3(0.0025F, 0.0025F, 0.0025F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("BonusGoldPackOnKill", "DisplayTome", "Pelvis", new Vector3(-0.0051F, -0.0004F, -0.0067F), new Vector3(7.9428F, 200.4164F, 14.3652F), new Vector3(0.0025f, 0.0025f, 0.0025f)));
            /*childName = "Head",
localPos = new Vector3(0.0016F, 0.0039F, -0.0006F),
localAngles = new Vector3(325.1126F, 89.0096F, 271.0507F),
localScale = new Vector3(0.0025F, 0.005F, 0.005F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("Squid", "DisplaySquidTurret", "Head", new Vector3(0.0016F, 0.0039F, -0.0006F), new Vector3(325.1126F, 89.0096F, 271.0507F), new Vector3(0.0025F, 0.005F, 0.005F)));
            /*childName = "Broom3",
localPos = new Vector3(0.0074F, -0.041F, 0.0349F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.015F, 0.015F, 0.015F)*/
            obj.Add(CloudUtils.CreateGenericDisplayRule("LaserTurbine", "DisplayLaserTurbine", "Broom3", new Vector3(0.0074F, -0.041F, 0.0349F), new Vector3(0, 90, 0)  , new Vector3(0.015f, 0.015f, 0.015f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("Incubator", "DisplayAncestralIncubator", "BroomModel", new Vector3(0, 0.012f, 0), new Vector3(90, 0, 0), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("SiphonOnLowHealth", "DisplaySiphonOnLowHealth", "Pelvis", new Vector3(-0.006f, 0.004f, 0.006f), new Vector3(0, 315, 180), new Vector3(0.0035f, 0.0035f, 0.0035f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("BleedOnHitAndExplode", "DisplayBleedOnHitAndExplode", "UpperLeg.R", new Vector3(0.005f, 0.005f, 0), new Vector3(0, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("MonstersOnShrineUse", "DisplayMonstersOnShrineUse", "UpperLeg.L", new Vector3(-0.005f, 0.005f, 0.002f), new Vector3(90, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            obj.Add(CloudUtils.CreateGenericDisplayRule("RandomDamageZone", "DisplayRandomDamageZone", "Hand.L", new Vector3(0.002f, 0.005f, 0.001f), new Vector3(0, 270, 270), new Vector3(0.002f, 0.002f, 0.002f)));

            //weird rules here
            obj.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "IncreaseHealing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = CommonAssets.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(0.0048F, -0.0002F, 0.0015F),
localAngles = new Vector3(0.6602F, 1.1322F, 300.2804F),
localScale = new Vector3(0.015F, 0.015F, 0.015F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = CommonAssets.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(0.0048F, -0.0002F, -0.0021F),
localAngles = new Vector3(0.6602F, 1.1322F, 300.2804F),
localScale = new Vector3(0.015F, 0.015F, -0.015F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


        }

        public override void GenerateRenderInfos(List<CharacterModel.RendererInfo> arg1, Transform arg2)
        {
            base.GenerateRenderInfos(arg1, arg2);
            var broom = arg2.Find("Brom");
            var mat = broom.GetComponentInChildren<SkinnedMeshRenderer>();
            arg1.Add(new CharacterModel.RendererInfo
            {
                defaultMaterial = mat.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false,
                renderer = mat,
            });
        }

        public override void CreateMainState(EntityStateMachine machine)
        {
            base.CreateMainState(machine);
            LoadoutAPI.AddSkill(typeof(BaseEmote.CustodianSickness));
            LoadoutAPI.AddSkill(typeof(BaseEmote));
            LoadoutAPI.AddSkill(typeof(WyattMain));
            machine.mainStateType = new SerializableEntityStateType(typeof(WyattMain));
        }

        public override void CreatePassive(SkillLocator skillLocator)
        {
            var passive = skillLocator.passiveSkill;

            passive.enabled = true;
            passive.skillNameToken = "WYATT_PASSIVE_NAME";
            passive.skillDescriptionToken = "WYATT_PASSIVE_DESCRIPTION";
            passive.keywordToken = "KEYWORD_VELOCITY";
            passive.icon = AssetsCore.wyattPassive;

            LanguageAPI.Add(passive.keywordToken, "<style=cKeywordName>Velocity</style><style=cSub>Increases movement speed by X% and health regeneration by X; all stacks lost when out of combat.</style>");
            LanguageAPI.Add(passive.skillNameToken, "Walkman");
            LanguageAPI.Add(passive.skillDescriptionToken, "On hit, gain a stack of <style=cIsUtility>Velocity</style>, up to 10. <style=cIsHealth>Lose two stacks every two seconds</style>");

            skillLocator.passiveSkill = passive;

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
            characterBody.portraitIcon = AssetsCore.mainAssetBundle.LoadAsset<Texture>("WyattPortrait"); //The portrait icon, shows up in multiplayer and the death UI
            characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/SurvivorPod");
        }

        public override void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            //--FIXED--
            //Alright, here's the problem:
            //Interrupt priorities are weird, and because they are weird
            //they cause the primary to not fire off after the secondary is fired.
            //Obvious problem is obvious, but setting the primary's interrupt priority to PrioritySkill 
            //makes it so you can't hold it down. Extremely obvious problem is obvious, but I have no idea how's to fix this.
            //FIX: Apparently I forgot to call base.FixedUpdate(n.k); in its FixedUpdate void, whoops!

            LoadoutAPI.AddSkill(typeof(FullSwing));
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
        public override void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
        {

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
        public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
        {
            LoadoutAPI.AddSkill(typeof(FireWinch));
            LoadoutAPI.AddSkill(typeof(DeepClean));
            LoadoutAPI.AddSkill(typeof(DeeperClean));
            LoadoutAPI.AddSkill(typeof(SS2Dies));
            LoadoutAPI.AddSkill(typeof(DRIVEMETOTHEHIGHWAY));
            LoadoutAPI.AddSkill(typeof(FireRocket));

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            utilitySkillDef.activationState = new SerializableEntityStateType(typeof(DRIVEMETOTHEHIGHWAY));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 4f;
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
        }
        public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
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
            specialSkillDef.interruptPriority = InterruptPriority.PrioritySkill;
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
            specialSkillDef2.interruptPriority = InterruptPriority.PrioritySkill;
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

        protected override void Initialization()
        {
            MAIDManager janniePower = survivorBody.AddComponent<MAIDManager>();
            SfxLocator sfxLocator = survivorBody.GetComponent<SfxLocator>();
            //CharacterDeathBehavior characterDeathBehavior = wyattBody.GetComponent<CharacterDeathBehavior>();
            HANDDroneTracker tracker = survivorBody.AddComponent<HANDDroneTracker>();
            survivorBody.AddComponent<WyattWalkmanBehavior>();
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
        }
    }
}
