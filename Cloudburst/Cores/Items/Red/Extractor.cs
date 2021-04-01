using BepInEx.Configuration;

using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace Cloudburst.Cores.Items.Red
{

    public class Extractor : ItemBuilder
    {
        public List<ItemDef> bossitemList = new List<ItemDef>{
            RoR2Content.Items.NovaOnLowHealth,
            RoR2Content.Items.Knurl,
            RoR2Content.Items.BeetleGland,
            RoR2Content.Items.TitanGoldDuringTP,
            RoR2Content.Items.SprintWisp,
            RoR2Content.Items.BleedOnHitAndExplode,
            RoR2Content.Items.FireballsOnHit 
            //Excluding pearls because those aren't boss Item, they come from the Cleansing Pool 
        };
        public void DropShipCall(Transform transform, int itemCount)
        {
            List<PickupIndex> list = Util.CheckRoll((5 * itemCount)) ? Run.instance.availableTier2DropList : Run.instance.availableTier1DropList;
            int item = Run.instance.treasureRng.RangeInt(0, list.Count);

            PickupDropletController.CreatePickupDroplet(list[item], transform.position, new Vector3(0, 10, 0));


            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/PodGroundImpact"), new EffectData
            {
                origin = transform.position,
                scale = 15
            }, true);
        }

        public ItemDef GetRandomItem(List<ItemDef> Item)
        {
            int itemID = UnityEngine.Random.Range(0, Item.Count);
            return Item[itemID];
        }

        public override ItemTag[] ItemTags => new ItemTag[2]
{
    ItemTag.OnKillEffect,
    ItemTag.AIBlacklist
};

        public override string ItemName => "Extractor";

        public override string ItemLangTokenName => "EXTRACTINGMOMENT";

        public override string ItemPickupDesc => "Chance for debuffs to become beneficial buffs when applied";
        //
        public override string ItemFullDescription => "On boss death, <style=cIsUtility>25% chance <style=cStack>(+10% per stack)</style> for bosses to drop a random boss item when killed.</style> Nearby projectiles are also destroyed, gain 5 <style=cStack>(+5 per stack)</style> barrier for each destroyed projectile. ";

        public override string ItemLore => "On death, bosses have a chance to drop an item. Nearby projectiles are also destroyed, gain barrier for each destroyed projectile.";

        public override ItemTier Tier => ItemTier.Tier3;

        public override string ItemModelPath => "Assets/Cloudburst/Items/Grinder/IMDLGrinder.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/Grinder/icon.png";


        public override void CreateConfig(ConfigFile config)
        {

        }

        /*public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }*/


        protected override void Initialization()
        {

        }

        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var itemChampionOnKillCount = GetCount(damageReport.attackerBody);
            var attackerBody = damageReport.attackerBody;
            var attackerMaster = damageReport.attackerMaster;
            var victimBody = damageReport.victimBody;
            if (itemChampionOnKillCount > 0 && damageReport.victimIsBoss && attackerBody && attackerMaster)
            {
                Util.PlaySound("ui_obj_casinoChest_open", attackerBody.gameObject);
                if (Util.CheckRoll(15 + (itemChampionOnKillCount * 5)))
                {
                    EffectData data = new EffectData
                    {
                        scale = 1000,
                        origin = victimBody.transform.position,
                    };
                    LogCore.LogI("YOOOOOOOOOOO!");
                    EffectManager.SpawnEffect(EntityStates.Toolbot.ToolbotDash.knockbackEffectPrefab, data, true);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(GetRandomItem(bossitemList).itemIndex), victimBody.transform.position, new Vector3(0, 50, 0));
                }
                Collider[] array = Physics.OverlapSphere(victimBody.transform.position, 10, LayerIndex.projectile.mask);


                for (int i = 0; i < array.Length; i++)
                {
                    ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                    if (pc)
                    {
                        //if its not a friendly
                        if (pc.teamFilter.teamIndex != attackerBody.teamComponent.teamIndex)
                        {
                            attackerBody.healthComponent.AddBarrier(5 * itemChampionOnKillCount);
                        }
                    }
                }
            }
        }
    }
}