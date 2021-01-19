using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class MAID : MonoBehaviour
    {
        private ProjectileController controller;
        /// <summary>
        /// List containing ground enemies, like lemurians and beetles/
        /// </summary>
        public EnigmaticList<CharacterMotor> characterMotors;
        public EnigmaticList<Rigidbody> rigidBodies;


        private float stopwatch;
        private ProjectileOwnerInfo owner = default;
        public void Awake()
        {
            characterMotors = new EnigmaticList<CharacterMotor>();
            rigidBodies = new EnigmaticList<Rigidbody>();

            controller = base.gameObject.GetComponent<ProjectileController>();
        }

        public void Start()
        {
            owner = new ProjectileOwnerInfo(controller.owner, "");
            owner.gameObject.GetComponent<MAIDManager>().DeployMAIDAuthority(base.gameObject);
        }

        #region Collision
        public void OnTriggerEnter(Collider collider)
        {
            CharacterMotor motor = collider.GetComponent<CharacterMotor>();
            CharacterBody body = collider.GetComponent<CharacterBody>();
            Rigidbody rigid = collider.GetComponent<Rigidbody>();

            if (motor && body && body.HasBuff(BuffCore.instance.antiGravIndex))
            {
                characterMotors.Add(motor);
            }
            if (rigid && body && body.HasBuff(BuffCore.instance.antiGravIndex)) {
                rigidBodies.Add(rigid);
            }
        }

        public void OnEntry(CharacterMotor motor = null, Rigidbody body = null) {
            if (motor) {
                characterMotors.Add(motor);

            }
            if (body && !motor) {
                rigidBodies.Add(body);
            }

        }

        public void OnTriggerExit(Collider collider)
        {
            CharacterMotor motor = collider.GetComponent<CharacterMotor>();
            Rigidbody rigid = collider.GetComponent<Rigidbody>();

            if (motor)
            {
                characterMotors.Remove(motor);
            }
            if (rigid && !motor) {
                rigidBodies.Remove(rigid);
            }
        }
        #endregion

        public void FixedUpdate() {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= 1.5f)
            {
                EffectManager.SpawnEffect(EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab, new EffectData
                {
                    origin = transform.position,
                    scale = 2.5f
                }, true);
                for (int i = 0; i < characterMotors.Count; i++)
                {
                    var rigid = characterMotors[i];
                    if (rigid)
                    {
                        var body = rigid.body;

                        if (body && !body.HasBuff(BuffCore.instance.antiGravIndex)) {
                            characterMotors.Remove(rigid);
                        }

                        if (body && body.HasBuff(BuffCore.instance.antiGravIndex))
                        {
                            HandleAntiGravMotor(rigid);
                        }

                    }
                }
                for (int i = 0; i < rigidBodies.Count; i++)
                {
                    var rigid = rigidBodies[i];
                    if (rigid)
                    {
                        var body = rigid.GetComponent<CharacterBody>();

                        if (body && !body.HasBuff(BuffCore.instance.antiGravIndex)) {
                            rigidBodies.Remove(rigid);
                        }

                        if (body && body.HasBuff(BuffCore.instance.antiGravIndex))
                        {
                            HandleAntiGravRigid(rigid);
                        }
                    }
                }
                stopwatch = 0;
            }
        }

        private void HandleAntiGravMotor(CharacterMotor motor)
        {
            if (motor)
            {
                if (motor.isGrounded)
                {
                    motor.Motor.ForceUnground();
                    motor.ApplyForce(new Vector3(0, motor.mass, 0), true, true);
                }
                Vector3 normalized = (transform.position - motor.transform.position).normalized;
                motor.ApplyForce(normalized * 700, true, true);
            }
            else characterMotors.Remove(motor);
        }

        private void HandleAntiGravRigid(Rigidbody body)
        {
            if (body)
            {
                Vector3 normalized = (transform.position - body.transform.position).normalized;
                body.AddForce(normalized * 700, ForceMode.Acceleration);
            }
            else rigidBodies.Remove(body);
        }


        public void OnDestroy() {
            //ensure that everything is wiped on death
            characterMotors = null;
            rigidBodies = null;
        }
    }
}
