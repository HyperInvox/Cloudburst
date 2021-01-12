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
        private EnigmaticList<CharacterMotor> characterMotors;
        private EnigmaticList<Rigidbody> rigidBodies;


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
                LogCore.LogI("proced MAID");
                EffectManager.SpawnEffect(EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab, new EffectData
                {
                    origin = transform.position,
                    scale = 2.5f
                }, true);
                for (int i = 0; i < characterMotors.Count; i++)
                {
                    HandleAntiGravMotor(characterMotors[i]);
                }
                for (int i = 0; i < rigidBodies.Count; i++)
                {
                    HandleAntiGravRigid(rigidBodies[i]);
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
