using BepInEx.Configuration;

using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Cloudburst.Cores.Items.Red
{

    public class Lemdog : ItemBuilder
    {
        public List<BuffDef> lemdogList;
        public override string ItemName => "Lemdog";

        public override string ItemLangTokenName => "LEMMYDOGS";

        public override string ItemPickupDesc => "Chance for debuffs to become beneficial buffs when applied";
        //
        public override string ItemFullDescription => "25% <style=cStack>(+2.5% per stack)</style> chance for <style=cIsUtility>applied debuffs to become beneficial buffs</style>";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier3;

        public override string ItemModelPath => "Assets/Cloudburst/Items/Lemdog/IMDLLemDog.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/Lemdog/LemDog_TexIcon.png";


        public override void CreateConfig(ConfigFile config)
        {

        }

        /* public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var lemdogMDL = Resources.Load<GameObject>(ItemModelPath);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
{
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0, 0.4f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)

}
});
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "HeadCenter",
        localPos = new Vector3(0.08f, 0.899f, 1.84f),
        localAngles = new Vector3(0,-90,0),
        localScale = new Vector3(5, 5, 5)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "HeadCenter",
        localPos = new Vector3(0, 0.3f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)
    }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0, 0.4f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)
    }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0, 0.4f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)
    }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "FlowerBase",
        localPos = new Vector3(0, 2, 0),
        localAngles = new Vector3(90,90,180),
        localScale = new Vector3(2,  2, 2)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0, 0.4f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0.48f, 5f, -0.01f),
        localAngles = new Vector3(15.79083f, 90,106.9842f),
        localScale = new Vector3(5, 5, 5)
    }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = lemdogMDL,
        childName = "Head",
        localPos = new Vector3(0, 0.4f, 0),
        localAngles = new Vector3(0,90,0),
        localScale = new Vector3(1   ,  1, 1)
    }
            });

            return rules;
        }*/


        protected override void Initialization()
        {
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManager_onContentPacksAssigned;
        }

        private void ContentManager_onContentPacksAssigned(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            lemdogList = new List<BuffDef>{
            RoR2Content.Buffs.Warbanner,
            //RoR2Content.Buffs.Cloak,
            RoR2Content.Buffs.CloakSpeed,
            //RoR2Content.Buffs.EngiShield,
            RoR2Content.Buffs.MeatRegenBoost,
            RoR2Content.Buffs.WhipBoost,
            RoR2Content.Buffs.TeamWarCry,
        };
        }

        public override void Hooks()
        {
            GlobalHooks.onAddTimedBuff += GlobalHooks_onAddTimedBuff;
        }

        private void GlobalHooks_onAddTimedBuff(CharacterBody body, ref BuffDef buffType, ref float duration)
        {
            var inv = body.inventory;
            if (inv)
            {
                var count = GetCount(inv);
                if (count > 0)
                {
                    if (buffType.isDebuff && Util.CheckRoll(25 + (count * 2.5f), body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Masterless) ? null : body.master) && buffType != RoR2Content.Buffs.NullSafeZone)
                    {
                        var random = UnityEngine.Random.Range(0, lemdogList.Count);
                        var buff = lemdogList[random];
                        buffType = buff;
                    }
                }
            }
        }
    }
}