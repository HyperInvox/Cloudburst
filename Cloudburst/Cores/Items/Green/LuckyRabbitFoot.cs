using BepInEx.Configuration;
using R2API;
using RoR2;

namespace Cloudburst.Cores.Items.Green
{

    public class LuckyRabbitFoot : ItemBuilder
    {
        public static LuckyRabbitFoot instance;

        public static bool Enabled;

        public override string ItemName => "Lucky Rabbit Foot";

        public override string ItemLangTokenName => "LUCKYRABBITFOOTMOMENT";

        public override string ItemPickupDesc => "Gain barrier on critical hits";

        public override string ItemFullDescription => "Gain a <style=cIsHealing>temporary barrier</style> on critical hits for <style=cIsHealing>5 health</style> <style=cStack>(+3 per stack)</style>.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "@Cloudburst:Assets/Cloudburst/Items/TopazLense/IMDLRabbitFoot.prefab";

        public override string ItemIconPath => "@Cloudburst:Assets/Cloudburst/Items/TopazLense/icon.png";


        public override void CreateConfig(ConfigFile config)
        {

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
            GlobalHooks.onCrit += GlobalHooks_onCrit;
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
                healthComponent.AddBarrier(5f + (topazLense * 3f));
            }
        }
    }
}