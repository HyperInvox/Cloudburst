
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

        public override string ItemFullDescription => Chance.Value + "% chance on hit to spawn a seeking <style=cIsDamage>orb</style> that does shocks nearby enemies for <style=cIsDamage>" + (BaseDamage.Value * 100) + "% <style=cStack>(+"+ StackingDamage.Value + "% per stack)</style></style> on impact.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/UESKeycard/IMDLKeycard.prefab";

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
                var pos = CloudUtils.FindBestPosition(victimBody.mainHurtBox);

                damageInfo.procChainMask.AddProc(ProcType.AACannon);
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
            return new ItemDisplayRuleDict();
        }
    }
}