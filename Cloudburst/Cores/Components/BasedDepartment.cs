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

        }

        public void FixedUpdate() {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= interval) {
                var thing = base.GetComponent<ModelLocator>().modelTransform.GetComponent<Animator>();
                thing.speed = 1f;
                thing.Update(0f);
                int layerIndex = thing.GetLayerIndex("Fullbody, Override");
                thing.CrossFadeInFixedTime("BuffEmpty", 0.5f, layerIndex);
                //		protected void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
                Destroy(this);
                //base.PlayCrossfade("Fullbody, Override", "BufferEmpty", 0.5f);
            }
            var wow = (direction * 2 * Assaulter2.speedCoefficient) * Time.fixedDeltaTime;
            motor.rootMotion += wow;
            characterDirection.forward = motor.rootMotion.normalized;
        }
    }
}
