using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Bombardier
{
    class GrandSlam : BaseSkillState
    {
        private float timer = 0;
        private float stopwatch = 0;
        private GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfx");
        private GameObject explosionEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLarge");
        private OverlapAttack pogo;
        private OverlapAttack fat;
        private bool hit;
        public override void OnEnter()
        {
            base.OnEnter();

            pogo = new OverlapAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.Default,
                damage = 6f * damageStat,
                damageColorIndex = DamageColorIndex.Item,
                damageType = DamageType.BonusToLowHealth,
                forceVector = Vector3.zero,
                hitBoxGroup = CloudUtils.FindHitBoxGroup("TempHitboxGrandSlamPogoFAT", base.GetModelTransform()),
                inflictor = base.gameObject,
                isCrit = RollCrit(),
                procChainMask = default,
                procCoefficient = 1,
                teamIndex = GetTeam(),
            }; fat = new OverlapAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.Default,
                damage = 1f * damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Generic,
                forceVector = Vector3.down * 100,
                hitBoxGroup = CloudUtils.FindHitBoxGroup("TempHitboxGrandSlam", base.GetModelTransform()),
                inflictor = base.gameObject,
                isCrit = false,
                procChainMask = default,
                procCoefficient = 1,
                teamIndex = GetTeam(),
            };

            if (characterBody)
            {
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }

            base.StartAimMode(2f, false);

            if (base.isAuthority)
            {
                Slam();
            }
        }


        public virtual void Slam()
        {
            if (characterMotor)
            {
                //characterMotor.ApplyForce(new Vector3(characterMotor.moveDirection.x, characterMotor.mass * 15, characterMotor.moveDirection.z), true, false);
                characterMotor.ApplyForce(new Vector3(characterMotor.moveDirection.x, -(characterMotor.mass * 70), characterMotor.moveDirection.z), true, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterBody.AddSpreadBloom(10);
            timer += Time.deltaTime;
            stopwatch += Time.deltaTime;    

            //if (timer >= .1f)
            //{
            //    timer = 0;
                //LogCore.LogI("hi");
                if (isAuthority)
                {
                fat.Fire();
                    bool hit = pogo.Fire(null);
                    if (hit)
                    {
                        this.hit = true;
                        EffectManager.SpawnEffect(hitEffectPrefab, new EffectData
                        {
                            origin = base.transform.position,
                            scale = 8
                        }, true);
                        //LogCore.LogI("hi2");
                        characterMotor.Motor.ForceUnground();
                        characterMotor.velocity = Vector3.zero;
                        characterMotor.velocity = Vector3.up * 30;
                        outer.SetNextStateToMain();
                        return;
                    }
                }
            //}

            /*if (stopwatch >= .5f)
            {
                stopwatch = 0;
                BlastAttack attack = new BlastAttack()
                {
                    attacker = base.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = 3f * base.damageStat,
                    baseForce = 0,
                    bonusForce = new Vector3(0, 0, 0),
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.Linear,
                    impactEffect = EffectIndex.Invalid,
                    inflictor = base.gameObject,
                    losType = BlastAttack.LoSType.NearestHit,
                    position = base.transform.position,
                    procChainMask = default,
                    procCoefficient = 1f,
                    radius = 10,
                    teamIndex = GetTeam(),
                };
                EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData
                {
                    origin = base.transform.position,
                    scale = 10
                }, true);
                attack.Fire();
            }*/
            if (isGrounded && !hit && isAuthority) {
                outer.SetNextStateToMain();
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleQueenDeathImpact"), new EffectData
                {
                    origin = characterBody.footPosition,
                    scale = 15,
                }, true);
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit(); if (hit)
            {
                skillLocator.utility.rechargeStopwatch += 2;
            }
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }
    }
}