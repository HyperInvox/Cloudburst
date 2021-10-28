using System;
using EntityStates;
using Cloudburst.Cores.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Bombardier
{
    public class FireRocketInThrees : BaseSkillState
    {
        public static GameObject projectilePrefab = ProjectileCore.bombardierBombProjectile;
        public static GameObject effectPrefab = EntityStates.Commando.CommandoWeapon.FireRocket.effectPrefab;

        public static DamageType damageType = DamageType.Generic;
        public static float damageCoeff = 3.75f;
        public static float procCoeff = 1;
        public static float force = 5000;
        public static float baseDuration = 1f;

        private float barrage;
        private float rockCount = 0;
        public override void OnEnter()
        {
            base.OnEnter();
            base.StartAimMode(2f, false);
        }

        public virtual void FireProjectile()
        {
            if (base.isAuthority)
            {
                var aimRay = GetAimRay();
                ProjectileManager.instance.FireProjectile(GetInfo(aimRay));                

                if (ShouldHaveKnockback() && !isGrounded && characterMotor)
                {
                    characterMotor.ApplyForce(GetKnockback(aimRay), false, false);
                }

                if (characterBody )
                {
                    characterBody.AddSpreadBloom(100);
                }
            }
        }

        public virtual FireProjectileInfo GetInfo(Ray aimRay)
        {
            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageCoeff * damageStat,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = damageType,
                force = 1500,
                owner = gameObject,
                position = transform.position,
                procChainMask = default,
                projectilePrefab = GrabProjectile(),
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                target = null,
                useFuseOverride = false,
                useSpeedOverride = true,
                speedOverride = 150
            };
            return info;
        }

        public virtual bool ShouldPopStickies()
        {
            return true;
        }

        public virtual bool ShouldHaveKnockback()
        {
            return true;
        }

        public virtual GameObject GrabProjectile()
        {
            return ProjectileCore.bombardierFireBombProjectile;
        }

        public virtual Vector3 GetKnockback(Ray aimRay)
        {
            return aimRay.direction * (-characterMotor.mass * 30);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            barrage += Time.deltaTime;
            if (barrage >= .3)
            {
                FireProjectile();
                rockCount += 1;
                barrage = 0;
            }

            if (rockCount >= 3 && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}