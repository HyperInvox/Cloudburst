using BepInEx.Configuration;
using R2API;
using RoR2;

namespace Cloudburst.Cores.Items.White
{

    public class GlassHarvester : ItemBuilder
    {
        public override ItemTag[] ItemTags => new ItemTag[2]
{
        ItemTag.Utility,
        ItemTag.AIBlacklist
};
        public override string ItemName => "Glass Harvester";

        public override string ItemLangTokenName => "GLASSHARVESTER";

        public override string ItemPickupDesc => "Gain experience on hit";

        public override string ItemFullDescription => "Gain 3 <style=cStack>(+2 per stack)</style> <style=cIsUtility>experience</style> on hit.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override string ItemModelPath => "Assets/Cloudburst/Items/Harvester/IMDLHarvester.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/Harvester/icon.png";

        public ConfigEntry<float> BaseExp;
        public ConfigEntry<float> StackingExp;

        public override void CreateConfig(ConfigFile config)
        {
            BaseExp = config.Bind<float>(ConfigName, "Base Experience", 3, "How much experience you get from a single stack of this item.");
            StackingExp = config.Bind<float>(ConfigName, "Stacking Experience", 2, "How much extra experience you get from extra stacks of this item.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override void Initialization()
        {

        }

        public override void Hooks()
        {
            GlobalHooks.onHitEnemy += GlobalHooks_onHitEnemy;
        }

        private void GlobalHooks_onHitEnemy(ref DamageInfo info, UnityEngine.GameObject victim, GlobalHooks.OnHitEnemy onHitInfo)
        {
            int count = GetCount(onHitInfo.attackerBody);
            if (count > 0)
            {

                float exp = BaseExp.Value;
                if (count > 1) {
                    exp += (StackingExp.Value * count);
                }

                //this stacking is probably awful, but it shall suffice
                //11/26/21  

                TeamManager.instance.GiveTeamExperience(TeamComponent.GetObjectTeam(onHitInfo.attackerBody.gameObject), (uint)exp);
            }
        }
    }
}