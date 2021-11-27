
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Items
{
    public class MechanicalTrinket : ItemBuilder
    {
        public override string ItemName =>
            "Mechanical Trinket";

        public override string ItemLangTokenName =>
            "MECHANICALTRINKET_ITEM";

        public override string ItemPickupDesc =>
            "Increase teleporter radius and summon drones when the teleporter is activated. ";

        public override string ItemFullDescription =>
            "Summon 1 <style=cStack>(+1 per stack, up to 4) drone that has <style=cIsDamage>100% <style=cStack>(+50% per stack)</style> damage</style> and <style=cIsHealing>100% <style=cStack>(+100% per stack)</style> health</style>. Also <style=cIsUtility>increase teleporter radius by 5m</style> <style=cStack>(+5m per stack)</style>. ";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/MechanicalTrinket/IMDLMechanicalTrinket.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/MechanicalTrinket/Icon.png";


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var mdl = AssetsCore.mainAssetBundle.LoadAsset<GameObject>(ItemModelPath);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.1122F, 0.3168F, 0.212F),
localAngles = new Vector3(350.1537F, 0.0057F, 359.9348F),
localScale = new Vector3(4F, 4F, 4F)

}
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,

}
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Head",
localPos = new Vector3(0.4493F, 3.1995F, -1.1082F),
localAngles = new Vector3(54.7524F, 359.9821F, 0.1173F),
localScale = new Vector3(25F, 25F, 25F)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "CannonHeadR",
localPos = new Vector3(-0.2175F, 0.2798F, -0.0528F),
localAngles = new Vector3(0.4107F, 271.4308F, 238.6879F),
            localScale = new Vector3(7F, 7F, 7F)              }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.1244F, 0.1703F, 0.1248F),
localAngles = new Vector3(331.7253F, 358.0778F, 4.2768F),
localScale = new Vector3(3F, 3F, 3F)            }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.0034F, 0.1911F, -0.2936F),
localAngles = new Vector3(13.9531F, 359.9117F, 359.09F),
localScale = new Vector3(5F, 5F, 5F)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "MechLowerArmL",
localPos = new Vector3(0.0018F, 0.6337F, 0.0048F),
localAngles = new Vector3(281.7989F, 159.6738F, 152.303F),
localScale = new Vector3(10F, 10F, 10F)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            }); rules.Add("mdlTreebot", new ItemDisplayRule[]
 {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Eye",
localPos = new Vector3(0.0203F, 0.7649F, 0.0108F),
localAngles = new Vector3(270.5436F, 0F, 0F),
localScale = new Vector3(12F, 12F, 12F)
    }
 });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl  ,
childName = "Pelvis",
localPos = new Vector3(-0.0545F, -0.1244F, -0.205F),
localAngles = new Vector3(345.5598F, 194.8679F, 174.2104F),
localScale = new Vector3(4F, 4F, 4F)
    }
            });


            return rules;
        }




        protected override void Initialization()
        {

        }



        public override void Hooks()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteractionOnTeleporterBeginChargingGlobal;
        }
        public void TeleporterInteractionOnTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            int count = Util.GetItemCountForTeam(TeamIndex.Player, Index.itemIndex, false, true);
            if (count > 0)
            {
                obj.holdoutZoneController.baseRadius +=(count * 5);
                if (NetworkServer.active)
                {
                    for (int i = 0; i < count; i++) {
                    if (i < 4) {
//FUCK YO MAMA
                            float y = Quaternion.LookRotation(obj.transform.forward).eulerAngles.y;
                            float d = 3f;

                                Quaternion rotation = Quaternion.Euler(0f, y + 10, 0f);
                                Quaternion rotation2 = Quaternion.Euler(0f, y + 10 + 180f, 0f);
                                Vector3 position2 = obj.transform.position + rotation * (Vector3.forward * d);
                                CharacterMaster characterMaster = new MasterSummon
                                {
                                    masterPrefab = Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster"),
                                    position = position2,
                                    rotation = rotation2,
                                    teamIndexOverride = new TeamIndex?(TeamIndex.Player),
                                    ignoreTeamMemberLimit = false
                                }.Perform();
                                if (characterMaster)
                                {
                                    //I AM lazy

                                    characterMaster.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = 120 + UnityEngine.Random.Range(0f, 3f);
                                    float numButReal = 1f;
                                    float num2ButReal = 1f;
                                    num2ButReal += Run.instance.difficultyCoefficient / 8f;
                                    numButReal += Run.instance.difficultyCoefficient / 2f;
                                    numButReal *= Mathf.Pow((float)count, 1f);
                                    num2ButReal *= Mathf.Pow((float)count, 0.5f);
                                    characterMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((numButReal - 1f) * 10f));
                                    characterMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((num2ButReal - 1f) * 10f));                                }
                            }
                        }
                    }
                    
            }
        }
    }
}