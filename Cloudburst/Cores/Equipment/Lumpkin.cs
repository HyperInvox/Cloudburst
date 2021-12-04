using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.Equipment
{


    public class ExampleEquipment : EquipmentBuilder
    {
        /*            R2API.LanguageAPI.Add("EQUIPMENT_LUMPKIN_NAME", "The Lumpkin");
            R2API.LanguageAPI.Add("EQUIPMENT_LUMPKIN_PICKUP", "And his screams were Brazilian...");
            R2API.LanguageAPI.Add("EQUIPMENT_LUMPKIN_DESC", "Release a Brazilian scream that does <style=cIsDamage>500% damage, and twice your maximum health for damage</style>.");
            R2API.LanguageAPI.Add("EQUIPMENT_LUMPKIN_LORE", "");

*/
        public override string EquipmentName => "The Lumpkin";

        public override string EquipmentLangTokenName => "LUMPKIN";

        public override string EquipmentPickupDesc => "And his screams were Brazilian...";

        public override string EquipmentFullDescription => "Release a Brazilian scream that does <style=cIsDamage>500% damage, and twice your maximum health for damage</style>.";

        public override string EquipmentLore => "\"Lumpkin, one of the many rebel commanders of WW19, possessed a scream that could deafen his oppenents. In many battles and skrimishes, he would often use this scream to ambush squads and slaughter them. He continued this practice until the final battle of WW19, when he had his left lung sforcibly ripped from his chest and eaten. \r\n\r\nHis lungs, pictured above, allowed him to scream loudly without injuring himself.\"\r\n\r\n-Exhibit at The National WW19 Museum";

        public override string EquipmentModelPath => "Assets/Cloudburst/Equipment/Lumpkin/IMDLLumpkin.prefab";

        public override string EquipmentIconPath => "Assets/Cloudburst/Equipment/Lumpkin/icon.png";


        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var mdl = AssetsCore.mainAssetBundle.LoadAsset<GameObject>(EquipmentModelPath);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();

            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(-0.1546F, 0.2269F, 0.3945F),
localAngles = new Vector3(2.1125F, 264.9913F, 92.1811F),
localScale = new Vector3(1F, 1F, 1F)
}
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
        childName = "Chest",
localPos = new Vector3(-0.1546F, 0.2269F, 0.3945F),
localAngles = new Vector3(2.1125F, 264.9913F, 92.1811F),
localScale = new Vector3(1F, 1F, 1F)
}
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(-0.4381F, 1.8153F, 4.3646F),
localAngles = new Vector3(3.4262F, 271.8089F, 104.9852F),
localScale = new Vector3(8F, 8F, 8F)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.0335F, 0.3745F, 0.3836F),
localAngles = new Vector3(322.4962F, 232.2146F, 118.0735F),
localScale = new Vector3(1F, 1F, 1F)             }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.0531F, 0.0726F, 0.3056F),
localAngles = new Vector3(3.4891F, 290.111F, 61.6155F),
localScale = new Vector3(1F, 1F, 1F)           }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "Chest",
localPos = new Vector3(0.0439F, 0.2073F, 0.3381F),
localAngles = new Vector3(359.9271F, 289.9793F, 85.8721F),
localScale = new Vector3(1F, 1F, 1F)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "MechHandR",
localPos = new Vector3(-0.0264F, 0.2244F, -0.011F),
localAngles = new Vector3(298.3183F, 204.5738F, 95.844F),
localScale = new Vector3(1F, 1F, 1F)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "HandL",
localPos = new Vector3(0.9815F, 1.4904F, -0.0826F),
localAngles = new Vector3(359.7866F, 89.432F, 70.9637F),
localScale = new Vector3(9F, 9F, 9F)
    }
            }); rules.Add("mdlTreebot", new ItemDisplayRule[]
 {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl,
childName = "MuzzleSyringe",
localPos = new Vector3(-0.1103F, -0.2115F, -1.274F),
localAngles = new Vector3(357.233F, 271.4698F, 277.0092F),
localScale = new Vector3(2F, 2F, 2F)
    }
 });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = mdl  ,
childName = "Head",
localPos = new Vector3(0.3209F, 0.2037F, -0.2083F),
localAngles = new Vector3(331.4015F, 25.9899F, 115.6633F),
localScale = new Vector3(1F, 1F, 1F)
    }
            });


            return rules;
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
