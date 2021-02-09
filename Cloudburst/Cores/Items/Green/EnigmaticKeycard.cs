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
            "ENIGMATICKEYCARD";

        public override string ItemPickupDesc =>
            "Chance to spawn a void orb on hit.";

        public override string ItemFullDescription => "8% chance on hit to spawn a <style=cIsDamage>void orb</style> that does <style=cIsDamage>100% <style=cStack>(+100% per stack)</style></style>.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "@Cloudburst:Assets/Cloudburst/Items/UESKeycard/IMDLKeycard.prefab";

        public override string ItemIconPath => "@Cloudburst:Assets/Cloudburst/Items/UESKeycard/icon.png";


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

        private void GlobalHooks_onHitEnemy(ref DamageInfo damageInfo, UnityEngine.GameObject victim, GlobalHooks.OnHitEnemy info)
        {
            var count = GetCount(info.attackerBody);
            var victimBody = info.victimBody;
            var attackerBody = info.attackerBody;
            var attackerMaster = info.attackerMaster;

            if (count > 0 && Util.CheckRoll(8 * damageInfo.procCoefficient, attackerMaster) && victimBody && !damageInfo.procChainMask.HasProc(ProcType.AACannon))
            {
                var pos = CloudUtils.FindBestPosition(victimBody.mainHurtBox);

                damageInfo.procChainMask.AddProc(ProcType.AACannon);
                EffectData data = new EffectData()
                {
                    rotation = Quaternion.Euler(victimBody.transform.forward),
                    scale = 1,
                    origin = pos,
                };
                var ddamage = count * 1f;
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
    }
}