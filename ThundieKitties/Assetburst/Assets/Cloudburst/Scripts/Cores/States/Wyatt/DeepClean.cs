﻿using EntityStates;
using EntityStates.ParentMonster;
using RoR2;
using RoR2.Navigation;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Wyatt
{
    class DeepClean : BaseSkillState
    {

        private float timer;
        private OverlapAttack attack;
        public static float baseDuration = 0.2f;
        public override void OnEnter()
        {
            base.OnEnter();

            //var obj = Cloudburst.Instantiate<GameObject>(Resources.Load<GameObject>("Assets/Cloudburst/Items/BrokenBodyArmor/MDLBrokenBodyArmor.prefab"), transform.position, transform.rotation);
            //NetworkServer.Spawn(obj);

            attack = new OverlapAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.Default,
                damage = 5f * damageStat,
                damageColorIndex = DamageColorIndex.Item,
                //damageType = DamageTypeCore.pullEnemies,
                forceVector = base.inputBank.aimDirection * (7 * 35 * Time.fixedDeltaTime),
                hitBoxGroup = CloudUtils.FindHitBoxGroup("TempHitboxFAT", base.GetModelTransform()),
                inflictor = base.gameObject,
                isCrit = RollCrit(),
                procChainMask = default,
                procCoefficient = 1,
                teamIndex = GetTeam(),
                };

            R2API.DamageAPI.AddModdedDamageType(attack, DamageTypeCore.pullEnemies);

            if (base.isAuthority)
            {
                characterMotor.velocity = new Vector3(characterMotor.velocity.x, 0, characterMotor.velocity.z);

                CreateBlinkEffect(Util.GetCorePosition(base.gameObject), characterMotor.moveDirection);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += Time.deltaTime;

            if (base.isAuthority)
            {
                var a = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                //base.inputBank.aimDirection
                if (timer > 0.1)
                {
                    attack.Fire(null);
                    timer = 0;
                }
                base.characterMotor.rootMotion += base.inputBank.aimDirection * (7 * (35 /2) * Time.fixedDeltaTime);
            }

            if (fixedAge >= baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
            };
        }
        public override void OnExit()
        {
            //if (characterBody)
            //{
                //characterBody.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;
            //}
            /*if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            } */
            if (base.characterMotor)
            {
                //base.characterMotor.enabled = true;
            }
            base.OnExit();
        }

        public void ShockEnemies()
        {
            LightningOrb lightningOrb2 = new LightningOrb();
            lightningOrb2.origin = base.transform.position;
            lightningOrb2.damageValue = 1f * damageStat;
            lightningOrb2.isCrit = RollCrit();
            lightningOrb2.bouncesRemaining = 1;
            lightningOrb2.teamIndex = GetTeam();
            lightningOrb2.attacker = gameObject;
            lightningOrb2.bouncedObjects = new List<HealthComponent>
                            {
                                base.healthComponent//.GetComponent<HealthComponent>()
                            };
            lightningOrb2.procChainMask = default;
            lightningOrb2.procCoefficient = 1;
            lightningOrb2.lightningType = LightningOrb.LightningType.Tesla;
            lightningOrb2.damageColorIndex = DamageColorIndex.Default;
            lightningOrb2.range += 10;
            lightningOrb2.damageType = (DamageType.Stun1s);
            HurtBox hurtBox2 = lightningOrb2.PickNextTarget(transform.position);
            if (hurtBox2)
            {
                lightningOrb2.target = hurtBox2;
                OrbManager.instance.AddOrb(lightningOrb2);
            }
            R2API.DamageAPI.AddModdedDamageType(lightningOrb2, DamageTypeCore.antiGrav);

        }

        private void CreateBlinkEffect(Vector3 origin, Vector3 direction)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(direction);
            effectData.origin = origin;
            EffectManager.SpawnEffect(LoomingPresence.blinkPrefab, effectData, true);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
