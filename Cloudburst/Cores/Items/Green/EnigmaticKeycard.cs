
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.Items
{
    public class EnigmaticKeycard : ItemBuilder
    {
        public override string ItemName =>
            "Enigmatic Keycard";

        public override string ItemLangTokenName =>
            "ENIGMACARD";

        public override string ItemPickupDesc =>
            "Chance to spawn an orb on hit that follows and shocks enemies.";

        public override string ItemFullDescription => Chance.Value + "% chance on hit to spawn a seeking <style=cIsDamage>orb</style> that does shocks nearby enemies for <style=cIsDamage>" + (BaseDamage.Value * 100) + "% base damage <style=cStack>(+"+ (StackingDamage.Value * 100)+ "% per stack)</style></style> on impact.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/UESKeycard/IMDLPricard.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/UESKeycard/icon.png";

        public ConfigEntry<float> BaseDamage;
        public ConfigEntry<float> StackingDamage;
        public ConfigEntry<float> Chance;

        public override void CreateConfig(ConfigFile file)
        {
            base.CreateConfig(file);
            BaseDamage = file.Bind<float>(ConfigName, "Base Damage", 1, "How much base damage the orb does. Multiply this by 100 to get the true value. e.x: 1 is 100% damage, 2.4 is 240% damage, and so on.");
            StackingDamage = file.Bind<float>(ConfigName, "Damage Stack", 1, "How much extra damage is added per stack. Multiply this by 100 to get the true value. e.x: 1 is 100% damage, 2.4 is 240% damage, and so on.");
            Chance = file.Bind<float>(ConfigName, "Spawning Chance", 8, "The chance of whether or not an orb spawns.");
        }

        protected override void Initialization()
        {

        }

        public override void Hooks()
        {
            GlobalHooks.onHitEnemy += GlobalHooks_onHitEnemy;
        }

        private void GlobalHooks_onHitEnemy(ref DamageInfo damageInfo, UnityEngine.GameObject victim, GlobalHooks.OnHitEnemy info)
        {
            var count = GetCount(info.attackerBody);
            var victimBody = info.victimBody;
            var attackerBody = info.attackerBody;
            var attackerMaster = info.attackerMaster;

            if (count > 0 && Util.CheckRoll(4 * damageInfo.procCoefficient, attackerMaster) && victimBody && !damageInfo.procChainMask.HasProc(ProcType.AACannon))
            {
                damageInfo.procChainMask.AddProc(ProcType.AACannon);
                var pos = CloudUtils.FindBestPosition(victimBody.mainHurtBox);

                EffectData data = new EffectData()
                {
                    rotation = Quaternion.Euler(victimBody.transform.forward),
                    scale = 1,
                    origin = pos,
                };
                var ddamage = count + (StackingDamage.Value * count);
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), data, true);
                FireProjectileInfo _info = new FireProjectileInfo() 
                {
                    crit = false,
                    damage = ddamage * attackerBody.damage,
                    damageColorIndex = RoR2.DamageColorIndex.Default,
                    damageTypeOverride = DamageType.Generic,
                    force = 0,
                    owner = attackerBody.gameObject,
                    position = pos,
                    procChainMask = default,
                    projectilePrefab = ProjectileCore.orbitalOrbPlayer,
                    rotation = Util.QuaternionSafeLookRotation(victimBody.transform.position),
                    target = victim,
                    useFuseOverride = false,
                    useSpeedOverride = true,
                    _speedOverride = 100
                };
                ProjectileManager.instance.FireProjectile(_info);
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var MDL = Load();
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();//new ItemDisplayRule[]
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[] {

    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.18733F, 0.4266F, -0.02852F),
localAngles = new Vector3(345.5522F, 277.2636F, 279.5949F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)

}
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.0018F, 0.18417F, 0.19908F),
localAngles = new Vector3(283.5806F, 128.7164F, 314.9402F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
}
            }); rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(-0.4835F, 1.75338F, 2.45922F),
localAngles = new Vector3(12.25006F, 354.9668F, 268.7202F),
localScale = new Vector3(2F, 2F, 2F)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.34062F, 0.38039F, -0.01067F),
localAngles = new Vector3(282.0785F, 348.7747F, 192.8666F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)          }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.01487F, 0.25356F, 0.11555F),
localAngles = new Vector3(342.1007F, 264.4269F, 271.5425F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)   }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.20435F, 0.2908F, -0.03178F),
localAngles = new Vector3(86.65052F, 122.2733F, 299.3474F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Chest",
localPos = new Vector3(0.27107F, 0.40974F, 0.1679F),
localAngles = new Vector3(4.58482F, 1.00541F, 178.4006F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Head",
localPos = new Vector3(-1.00932F, 4.08654F, -0.47985F),
localAngles = new Vector3(355.2828F, 178.0522F, 206.0567F),
localScale = new Vector3(5F, 5F, 5F)
    }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "Chest",
localPos = new Vector3(0.38487F, 0.32796F, -0.08475F),
localAngles = new Vector3(58.55202F, 315.1845F, 203.6885F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
    }
            }); rules.Add("mdlTreebot", new ItemDisplayRule[]
 {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "WeaponPlatform",
localPos = new Vector3(-0.19204F, 0.24397F, 0.29024F),
localAngles = new Vector3(0.73761F, 359.0455F, 354.586F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)    }
 }); rules.Add("mdlBandit2", new ItemDisplayRule[]
  {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "Chest",
localPos = new Vector3(0.14106F, 0.29356F, -0.22604F),
localAngles = new Vector3(276.5795F, 15.1902F, 245.6862F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
    }
  });


            return rules;
        }
    }
}