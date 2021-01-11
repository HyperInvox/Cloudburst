using EntityStates;
using Cloudburst.Cores.HAND;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    class DeployMaid : BaseSkillState
    {
        public static float baseDuration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                FireProjectile();
            }
        }

        public void FireProjectile() {
            var aimRay = base.GetAimRay();
            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = 0,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = DamageType.Generic,
                force = 0,
                owner = gameObject,
                position = aimRay.origin,
                procChainMask = default,
                projectilePrefab = ProjectileCore.wyattMaidBubble,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                target = null,
                useFuseOverride = false,
                useSpeedOverride = false
            };
            ProjectileManager.instance.FireProjectile(info);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= baseDuration && isAuthority)
            {
                skillLocator.special.SetSkillOverride(this, WyattCore.instance.retrievePrimary, GenericSkill.SkillOverridePriority.Contextual);
                outer.SetNextStateToMain();
            };
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}