using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.Items
{
    public class MechanicalTrinket : ItemBuilder
    {
        public override string ItemName =>
            "Mechanical Trinket";

        public override string ItemLangTokenName =>
            "MECHANICALTRINKET_ITEM";

        public override string ItemPickupDesc =>
            "Increase teleporter radius.";

        public override string ItemFullDescription =>
            "<style=cIsUtility>Increase teleporter radius by 5m</style> <style=cStack>(+5m per stack)</style>.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "@Cloudburst:Assets/Cloudburst/Items/MechanicalTrinket/IMDLMechanicalTrinket.prefab";

        public override string ItemIconPath => "@Cloudburst:Assets/Cloudburst/Items/MechanicalTrinket/Icon.png";


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var mechanicalTrinketMDL = Resources.Load<GameObject>(ItemModelPath);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
    {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mechanicalTrinketMDL,
        childName = "Chest",
        localPos = new Vector3(-0.0012f, 0.2745f, 0.2034f),
        localAngles = new Vector3(-18.398f, 0.02f, -0.125f),
        localScale = new Vector3(10,10, 10)

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
            int count = Util.GetItemCountForTeam(TeamIndex.Player, Index, false, true);
            if (count > 0)
            {
                obj.holdoutZoneController.baseRadius = obj.holdoutZoneController.baseRadius + (count * 5);
            }
        }
    }
}