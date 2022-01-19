/*using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.Items.White
{

    public class BrokenBodyArmor : ItemBuilder
    {

        public static BrokenBodyArmor instance;

        public static bool Enabled;

        public override string ItemName => "Broken Body Armor";

        public override string ItemLangTokenName => "BBA";

        public override string ItemPickupDesc => "Gain an armor buff when hurt";

        public override string ItemFullDescription => "Gain a buff that grants 8 <style=cStack>(+8 per stack)</style> <style=cIsUtility>armor</style> when hurt.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override string ItemModelPath => "Assets/Cloudburst/Items/BrokenBodyArmor/IMDLBrokenBodyArmor.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/BrokenBodyArmor/brokenarmoricon.png";


        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var bbaMDL = Resources.Load<GameObject>(ItemModelPath);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = bbaMDL,
        childName = "Chest",
        localPos = new Vector3(-0.001f, -0.5F, -0.004f),
        localAngles = Vector3.zero,
        localScale = new Vector3(0.5f   ,  0.5f, 0.8f)

}
            });
            return rules;
        }

        protected override void Initialization()
        {
            //armorBuff = new BrokenBodyArmorBuff();
            //armorBuff.RoR2Content.Items = Index;
            //armorBuff.Init();
            //SingletonHelper.Assign<BrokenBodyArmor>(instance, this);
            instance = this;
            Enabled = true;
        }

        public override void Hooks()
        {
            GlobalHooks.takeDamage += GlobalHooks_takeDamage;
        }

        private void GlobalHooks_takeDamage(ref DamageInfo info, UnityEngine.GameObject victim, GlobalHooks.OnHitEnemy onHitInfo)
        {
            //keep it sane 
            //make it right

            var victimBody = onHitInfo.victimBody;

            if (victimBody)
            {
                int count = GetCount(victimBody);
                if (count > 0)
                {
                    victimBody.AddTimedBuff(BuffCore.instance.skinb, 5);
                }
            }

        }
    }
}*/