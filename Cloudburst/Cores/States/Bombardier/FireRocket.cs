using System;
using EntityStates;
using Cloudburst.Cores.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Huntress.HuntressWeapon;
using System.Collections.Generic;

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

        private OverlapAttack attack;


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            base.StartAimMode(2f, false);
            base.PlayCrossfade("Fullbody, Override", "CoolSwing", "CoolSwing.playbackRate", this.duration, 0.05f);

            attack = new OverlapAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.Default,
                damage = 2 * base.damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageTypeCore.spiked, //overclock.GetDamageType(),
                forceVector = new Vector3(0, -1000, 0),
                hitEffectPrefab = null,
                impactSound = RoR2.Audio.NetworkSoundEventIndex.Invalid,
                inflictor = base.gameObject,
                isCrit = base.RollCrit(),
                maximumOverlapTargets = 100,
                procChainMask = default,
                procCoefficient = 1,
                pushAwayForce = 100,
                teamIndex = base.characterBody.teamComponent.teamIndex,
                hitBoxGroup = CloudUtils.FindHitBoxGroup("TempHitboxLarge", base.GetModelTransform()),
            };

            if (base.isAuthority)
            {
                FireProjectile();
            }
        }

        public virtual void FireProjectile()
        {

            var aimRay = GetAimRay();

            EffectManager.SimpleMuzzleFlash(Resources.Load<GameObject>("prefabs/effects/lemurianbitetrail"), base.gameObject, "SwingTrail", true);

            var list = new List<HealthComponent>();
            attack.Fire(list);

            if (base.teamComponent.teamIndex == TeamIndex.Player)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    HealthComponent healthComponent = list[i] ;
                    if (healthComponent)    
                    {
                        var charb = healthComponent.body;
                        if (charb && charb.modelLocator && charb != base.characterBody)
                        {
                            if (!charb.modelLocator.modelTransform.gameObject.GetComponent<SquashedComponent>())
                            {
                                charb.modelLocator.modelTransform.gameObject.AddComponent<SquashedComponent>().speed = 5f;
                                EffectManager.SpawnEffect(EffectCore.willIsNotPoggers/*Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfx")*/, new EffectData()
                                {
                                    origin = charb.transform.position,
                                    scale = 10,
                                    rotation = Quaternion.identity,
                                }, false);
                            }

                        }
                    }
                }
            }

            //ProjectileManager.instance.FireProjectile(GetInfo(aimRay));


            if (characterBody && ShouldApplyBloom())
            {
                characterBody.AddSpreadBloom(GetBloom());
            }

           /*var isNeikOnCrack = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlAudrey");
            var isNeikOnCrackButReal = UnityEngine.Object.Instantiate<GameObject>(isNeikOnCrack);
            isNeikOnCrackButReal.transform.position = transform.position;
            isNeikOnCrackButReal.transform.localScale = new Vector3(1, 1, 1);
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

        public override void OnExit()
        {
            base.OnExit();

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}