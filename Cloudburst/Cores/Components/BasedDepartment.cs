using EntityStates.Merc;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class BasedDepartment : MonoBehaviour
    {
        private BasicOwnerInfo info;
        private Vector3 direction;
        private CharacterMotor motor;
                private CharacterDirection characterDirection;

        public float interval = 0;
        private float stopwatch;
        public void Start()
        {
            info = new BasicOwnerInfo(base.gameObject, "");
            direction = info.inputBank.aimDirection;
            motor = info.characterMotor;
            characterDirection = GetComponent<CharacterDirection>();


            motor.onHitGround += Motor_onHitGround;

        }

        private void SetToEmpty()
        {
            var thing = base.GetComponent<ModelLocator>().modelTransform.GetComponent<Animator>();
            thing.speed = 1f;
            thing.Update(0f);
            int layerIndex = thing.GetLayerIndex("Fullbody, Override");
            thing.CrossFadeInFixedTime("BuffEmpty", 0.5f, layerIndex);
        }

        void Motor_onHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            SetToEmpty();

            EffectManager.SpawnEffect(EffectCore.wyattSlam, new EffectData
            {
                scale = 10,
                rotation = Quaternion.identity,
                origin = hitGroundInfo.position,
            }, true);

            /*EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleQueenDeathImpact")/*EffectCore.wyattSlam/*Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardGroundSlam"), new EffectData
            {
                scale = 1,
                rotation = Quaternion.identity,
                origin = hitGroundInfo.position,
            }, true);
            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardGroundSlam")/*EffectCore.wyattSlam/*Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardGroundSlam"), new EffectData
            {
                scale = 1,
                rotation = Quaternion.identity,
                origin = hitGroundInfo.position,
            }, true);*/





            new BlastAttack
            {
                position = hitGroundInfo.position,
                //baseForce = 3000,
                attacker = base.gameObject,
                inflictor = gameObject,
                teamIndex = info.characterBody.teamComponent.teamIndex,
                baseDamage = info.characterBody.damage * 8,
                attackerFiltering = default,
                //bonusForce = new Vector3(0, -3000, 0),
                damageType = DamageType.Stun1s, //| DamageTypeCore.spiked,
                crit = info.characterBody.RollCrit(),
                damageColorIndex = DamageColorIndex.WeakPoint,
                falloffModel = BlastAttack.FalloffModel.None,
                //impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/PulverizedEffect").GetComponent<EffectIndex>(),
                procCoefficient = 0,
                radius = 15
            }.Fire();

            var sphere = Physics.OverlapSphere(transform.position, 10);
            foreach (var body in sphere)
            {
                var cb = body.gameObject.GetComponentInParent<CharacterBody>();
                if (cb)
                {
                    if (cb.characterMotor && cb != info.characterBody && cb.isChampion == false)
                    {
                        CloudUtils.AddExplosionForce(cb.characterMotor, cb.characterMotor.mass * 25, transform.position, 25, 5, false);
                    }
                }
            }

            motor.onHitGround -= Motor_onHitGround;

            Destroy(this);
        }

        public void OnDestroy() {
            motor.onHitGround -= Motor_onHitGround;
        } 

        public void FixedUpdate() {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= (interval - 0.001f)) {
                motor.ApplyForce((direction * 125 * Assaulter2.speedCoefficient), true, false);
                characterDirection.forward = motor.rootMotion.normalized;
            }

            if (stopwatch >= interval) {
                SetToEmpty();
                //		protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
                Destroy(this);
                //base.PlayCrossfade("Fullbody, Override", "BufferEmpty", 0.5f);
            }

            var wow = (direction * 3 * Assaulter2.speedCoefficient) * Time.fixedDeltaTime;
            motor.rootMotion += wow;
            characterDirection.forward = motor.rootMotion.normalized;
        }
    }
}
