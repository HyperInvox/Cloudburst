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


        public override void CreateConfig(ConfigFile config)
        {

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
            if (GetCount(onHitInfo.attackerBody) > 0)
            {
                float exp = 1 + (GetCount(onHitInfo.attackerBody) * 2);
                TeamManager.instance.GiveTeamExperience(TeamComponent.GetObjectTeam(onHitInfo.attackerBody.gameObject), (uint)exp);
            }
        }
    }
}