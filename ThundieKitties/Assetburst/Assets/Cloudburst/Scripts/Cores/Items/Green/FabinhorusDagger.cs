using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.Items.Green
{

    public class FabinhorusDagger : ItemBuilder
    {
        public override ItemTag[] ItemTags => new ItemTag[1]
{
    ItemTag.Damage
};

        public override string ItemName => "Fabinhorus Dagger";

        public override string ItemLangTokenName => "YOUCANBEYOU";

        public override string ItemPickupDesc => "Chance to cripple enemies on hit.";

        public override string ItemFullDescription => "8% Chance to <style=cIsDamage>cripple enemies</style> for 3 <style=cStack>(+3 seconds per stack)</style> seconds.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/FabinhoruDagger/IMDLDagger.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/FabinhoruDagger/icon.png";


        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var fabDagMDL = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("Assets/Cloudburst/Items/FabinhoruDagger/IMDLDagger.prefab");
            //LogCore.LogI(fabDagMDL);
            //fabDagMDL..SetTexture("_NormalTex", texturehere);
            //material.SetFloat("_NormalStrength", normalstrengthhere);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(//new ItemDisplayRule[]
            /*{
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(5/2, 2/2, 3/2)

}
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3/2, 2/2, 3/2)
}
            }); rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = fabDagMDL  ,
        childName = "Chest",
        localPos = new Vector3(0, 0.2f, 0.3f),
        localAngles = new Vector3(-180f, 0, -0),
        localScale = new Vector3(3, 2, 3)
    }
            }*/);


            return rules;
        }

        protected override void Initialization()
        {

        }

        public override void Hooks()
        {
            GlobalHooks.onHitEnemy += GlobalHooks_onHitEnemy;
        }

        private void GlobalHooks_onHitEnemy(ref DamageInfo info, GameObject victim, GlobalHooks.OnHitEnemy onHitInfo)
        {
            if (GetCount(onHitInfo.attackerBody) > 0 && Util.CheckRoll(8 * info.procCoefficient, onHitInfo.attackerMaster) && onHitInfo.attackerMaster && onHitInfo.victimBody)
            {
                onHitInfo.victimBody.AddTimedBuff(RoR2Content.Buffs.Cripple, 3 * GetCount(onHitInfo.attackerBody));
            }
        }
    }
}