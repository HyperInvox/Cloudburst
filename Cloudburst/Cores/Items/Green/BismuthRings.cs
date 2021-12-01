using BepInEx.Configuration;
using R2API;
using RoR2;

namespace Cloudburst.Cores.Items.Green
{

    public class BismuthRings : ItemBuilder
    {
        public static BismuthRings instance;

        public static bool Enabled;

        public override ItemTag[] ItemTags => new ItemTag[1] {
            ItemTag.Healing,
        };

        public override string ItemName => "Bismuth Rings";

        public override string ItemLangTokenName => "BARRIERONCRIT";

        public override string ItemPickupDesc => "Gain barrier on critical hits";

        public override string ItemFullDescription => "Gain a <style=cIsHealing>temporary barrier</style> on critical hits for <style=cIsHealing>"+ BaseBarrier.Value + " health</style> <style=cStack>(+"+ StackingBarrier.Value+ " per stack)</style>. Also gain <style=cIsDamage>5% critical hit chance</style>.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/TopazLense/IMDLBismuthRings.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/TopazLense/icon.png";

        public ConfigEntry<float> BaseBarrier;
        public ConfigEntry<float> StackingBarrier;

        public override void CreateConfig(ConfigFile config)
        {
            BaseBarrier = config.Bind<float>(ConfigName, "Base Barrier", 5, "How much barrier a single stack of this item gives you.");
            StackingBarrier = config.Bind<float>(ConfigName, "Stacking Barrier", 3, "How much extra barrier stacks of this item gives you.");

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override void Initialization()
        {
            instance = this;
            Enabled = true;
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            GlobalHooks.onCrit += GlobalHooks_onCrit;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (GetCount(sender) > 0)
            {
                args.critAdd += 5;
            }
        }

        private void GlobalHooks_onCrit(CharacterBody attackerBody, CharacterMaster attackerMaster, float procCoeff, ProcChainMask procMask)
        {
            Inventory inv = attackerMaster.inventory;
            HealthComponent healthComponent = attackerBody.healthComponent;
            int topazLense = GetCount(inv) ;
            //int lightningEye = masterInventory.GetItemCount(lightningOnCritIndex);
            if (topazLense > 0 && healthComponent)
            {
                //oddly, this doesn't null reference
                healthComponent.AddBarrier(BaseBarrier.Value + (topazLense * StackingBarrier.Value));
            }
        }
    }
}