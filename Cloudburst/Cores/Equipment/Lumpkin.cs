using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.Equipment
{


    public class ExampleEquipment : EquipmentBuilder
    {
        /*            LanguageAPI.Add("EQUIPMENT_LUMPKIN_NAME", "The Lumpkin");
            LanguageAPI.Add("EQUIPMENT_LUMPKIN_PICKUP", "And his screams were Brazilian...");
            LanguageAPI.Add("EQUIPMENT_LUMPKIN_DESC", "Release a Brazilian scream that does <style=cIsDamage>500% damage, and twice your maximum health for damage</style>.");
            LanguageAPI.Add("EQUIPMENT_LUMPKIN_LORE", "");

*/
        public override string EquipmentName => "The Lumpkin";

        public override string EquipmentLangTokenName => "LUMPKIN";

        public override string EquipmentPickupDesc => "And his screams were Brazilian...";

        public override string EquipmentFullDescription => "Release a Brazilian scream that does <style=cIsDamage>500% damage, and twice your maximum health for damage</style>.";

        public override string EquipmentLore => "\"Lumpkin, one of the many rebel commanders of WW19, possessed a scream that could deafen his oppenents. In many battles and skrimishes, he would often use this scream to ambush squads and slaughter them. He continued this practice until the final battle of WW19, when he had his left lung sforcibly ripped from his chest and eaten. \r\n\r\nHis lungs, pictured above, allowed him to scream loudly without injuring himself.\"\r\n\r\n-Exhibit at The National WW19 Museum";

        public override string EquipmentModelPath => "@Cloudburst:Assets/Cloudburst/Equipment/Lumpkin/IMDLLumpkin.prefab";

        public override string EquipmentIconPath => "@Cloudburst:Assets/Cloudburst/Equipment/Lumpkin/icon.png";


        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override void Initialization()
        {

        }

        private bool Scream(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                CharacterBody body = slot.characterBody;
                BlastAttack impactAttack = new BlastAttack
                {
                    attacker = slot.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = (5f * body.damage) + (body.maxHealth * 2f),
                    baseForce = 5000,
                    bonusForce = new Vector3(0, 5000, 0),
                    crit = false,
                    damageColorIndex = DamageColorIndex.CritHeal,
                    damageType = DamageType.AOE,
                    falloffModel = BlastAttack.FalloffModel.None,
                    inflictor = body.gameObject,
                    losType = BlastAttack.LoSType.NearestHit,
                    position = body.transform.position,
                    procChainMask = default,
                    procCoefficient = 2f,
                    radius = 15,
                    teamIndex = body.teamComponent.teamIndex
                };
                impactAttack.Fire();
                EffectData effect = new EffectData()
                {
                    origin = body.footPosition,
                    scale = 15
                };
                EffectManager.SpawnEffect(EffectCore.lumpkinEffect, effect, true);
                return true;
            }
            return false;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return Scream(slot);
        }
    }
}
