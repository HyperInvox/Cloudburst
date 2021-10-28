using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    class SS2Dies : BaseSkillState
    {
        private bool hasLanded = false;
        private float timer = 0;
        public override void OnEnter()
        {
            base.OnEnter();
            if (characterBody)
            {
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (base.isAuthority)
            {
                characterMotor.ApplyForce(new Vector3(characterMotor.moveDirection.x, -(characterMotor.mass * 70), characterMotor.moveDirection.z), true, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer =+ Time.deltaTime;
            if (isGrounded && base.isAuthority) {
                hasLanded = true;
                BlastAttack attack = new BlastAttack()
                {
                    attacker = base.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = 5 * base.damageStat,
                    baseForce = 3000,
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
                    procCoefficient = 0.5f,
                    radius = 15,
                    teamIndex = GetTeam(),
                };
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleQueenDeathImpact"), new EffectData
                {
                    origin = characterBody.footPosition,
                    scale = 15,
                }, true);
                attack.Fire();
            }
            if (timer >= 0.5f)
            {
                if (base.isAuthority)
                {
                    timer = 0;
                    characterMotor.ApplyForce(new Vector3(characterMotor.moveDirection.x, -(characterMotor.mass * 30), characterMotor.moveDirection.z), true, false);
                    BlastAttack attack = new BlastAttack()
                    {
                        attacker = base.gameObject,
                        attackerFiltering = AttackerFiltering.Default,
                        baseDamage = 5 * base.damageStat,
                        baseForce = 3000,
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
                        procCoefficient = 0.5f,
                        radius = 15,
                        teamIndex = GetTeam(),
                    };
                    attack.Fire();
                }
            }
            if (hasLanded) {
                outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }
    }
}