using EntityStates.Merc;
using EntityStates.Toolbot;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class SpikingComponent : MonoBehaviour
    {
        private BasicOwnerInfo spikerInfo;
        private Vector3 direction;

        private CharacterMotor characterMotor;

        public GameObject originalSpiker;

        public float interval = 0;
        private float stopwatch;
        public void Start()
        {
            characterMotor = base.gameObject.GetComponent<CharacterMotor>();
            spikerInfo = new BasicOwnerInfo(originalSpiker, "");
                direction = Vector3.down;
            characterMotor.disableAirControlUntilCollision = true;

            characterMotor.onHitGround += Motor_onHitGround;
        }

        void Motor_onHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {

            EffectManager.SpawnEffect(EffectCore.tiredOfTheDingDingDing, new EffectData
            {
                scale = 10,
                rotation = Quaternion.identity,
                origin = hitGroundInfo.position,
            }, true);

            new BlastAttack
            {
                position = hitGroundInfo.position,
                //baseForce = 3000,
                attacker = originalSpiker,
                inflictor = originalSpiker,
                teamIndex = spikerInfo.characterBody.teamComponent.teamIndex,
                baseDamage = spikerInfo.characterBody.damage * 5,
                attackerFiltering = AttackerFiltering.NeverHit,
                //bonusForce = new Vector3(0, -3000, 0),
                damageType = DamageType.Stun1s, //| DamageTypeCore.spiked,
                crit = spikerInfo.characterBody.RollCrit(),
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
                    bool cannotHit = false;
                    if (cb.isChampion)
                    {
                        cannotHit = true;
                    }
                    if (cb.baseNameToken == "BROTHER_BODY_NAME")
                    {
                        cannotHit = false;
                    }
                    if (cb.characterMotor && cb != characterMotor.body && cannotHit == false && !(cb.gameObject == originalSpiker))
                    {
                        CloudUtils.AddExplosionForce(cb.characterMotor, cb.characterMotor.mass * 25, transform.position, 25, 5, false);
                    }
                }
            }

            characterMotor.onHitGround -= Motor_onHitGround;

            Destroy(this);
        }

        public void OnDestroy()
        {
            characterMotor.onHitGround -= Motor_onHitGround;
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (NetworkServer.active)
            {
                if (stopwatch >= (interval - 0.001f))
                {
                    characterMotor.ApplyForce((direction * 62.5f * Assaulter2.speedCoefficient), true, false);
                }

                if (stopwatch >= interval)
                {
                    //		protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
                    Destroy(this);
                    //base.PlayCrossfade("Fullbody, Override", "BufferEmpty", 0.5f);
                }
                var wow = (direction * 2 * Assaulter2.speedCoefficient) * Time.fixedDeltaTime;
                characterMotor.rootMotion += wow;

            }
        }
    }
}
