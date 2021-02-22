using System;
using EntityStates;
using Cloudburst.Cores.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Huntress.HuntressWeapon;

namespace Cloudburst.Cores.States.Bombardier
{
    public class FireRocket : BaseSkillState
    {
        public static GameObject projectilePrefab = ProjectileCore.bombardierBombProjectile;
        public static GameObject effectPrefab = EntityStates.Commando.CommandoWeapon.FireRocket.effectPrefab;

        public static DamageType damageType = DamageType.Generic;
        public static float damageCoeff = 3.75f;
        public static float procCoeff = 1;
        public static float force = 5000;
        public static float baseDuration = 1f;

        public static float timeBeforeKnockback;

        private float duration;
        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            base.StartAimMode(2f, false);
            base.PlayCrossfade("Fullbody, Override", "CoolSwing", "CoolSwing.playbackRate", this.duration, 0.05f);

            if (base.isAuthority)
            {
                FireProjectile();
            }
        }

        public virtual void FireProjectile()
        {

            var aimRay = GetAimRay();
            if (base.teamComponent.teamIndex == TeamIndex.Player)
            {

                Collider[] array = Physics.OverlapSphere(GetModelChildLocator().FindChild("TempHitboxSquish").position, 10, LayerIndex.defaultLayer.mask);
                for (int i = 0; i < array.Length; i++)
                {
                    HealthComponent healthComponent = array[i].GetComponent<HealthComponent>();
                    if (healthComponent)
                    {
                        var charb = healthComponent.body;
                        if (charb && charb.modelLocator && charb != base.characterBody)
                        {
                            if (!charb.modelLocator.modelTransform.gameObject.GetComponent<SquashedComponent>())
                            {
                                charb.modelLocator.modelTransform.gameObject.AddComponent<SquashedComponent>().speed = 5f;
                            }

                        }
                    }
                }
            }


            ProjectileManager.instance.FireProjectile(GetInfo(aimRay));


            if (characterBody && ShouldApplyBloom())
            {
                characterBody.AddSpreadBloom(GetBloom());
            }

            /*var isNeikOnCrack = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("IMDLLumpkin");
            var isNeikOnCrackButReal = UnityEngine.Object.Instantiate<GameObject>(isNeikOnCrack);
            isNeikOnCrackButReal.transform.position = transform.position;
            isNeikOnCrackButReal.transform.localScale = new Vector3(50, 50, 50);
            NetworkServer.Spawn(isNeikOnCrackButReal);*/

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

        public virtual GameObject GrabProjectile()
        {
            return ProjectileCore.bombardierBombProjectile; //Resources.Load<GameObject>("prefabs/projectiles/RedAffixMissileProjectile");
        }

        public virtual bool ShouldApplyBloom()
        {
            return true;
        }

        public virtual float GetBloom()
        {
            return 100;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            characterMotor.velocity.y = characterMotor.velocity.y + ThrowGlaive.antigravityStrength * Time.fixedDeltaTime * (1f - this.stopwatch / this.duration);
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}