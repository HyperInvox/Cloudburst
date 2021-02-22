using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class MAID : MonoBehaviour
    {
        private ProjectileController controller;

        private bool stop = false;
        private bool triggered = false;
        private float stopwatch = 0;
        private bool pause = false;
        private Rigidbody body;
        private ProjectileOwnerInfo owner = default;
        private BoomerangProjectile boomer;
        private float pauseStopwatch;
        private List<Rigidbody> bodies;
        public void Awake()
        {
            bodies = new List<Rigidbody>(); 
            controller = base.gameObject.GetComponent<ProjectileController>();
            boomer = base.gameObject.GetComponent<BoomerangProjectile>();
            body = GetComponent<Rigidbody>();
            stop = false;
        }

        public void Start()
        {
            owner = new ProjectileOwnerInfo(controller.owner, "");

            owner.gameObject.GetComponent<MAIDManager>().DeployMAIDAuthority(base.gameObject);
            boomer.onFlyBack.AddListener(OnHit);
        }

        private void OnHit() {

        }

        private void OnTriggerEnter(Collider other) {
            LogCore.LogI(other);
        }

        public void OnTriggerExit(Collider other)
        {
            LogCore.LogI(other);

        }

        public void FixedUpdate() {

                    if (boomer.stopwatch >= boomer.maxFlyStopwatch && triggered == false)
            {
                triggered = true;
                pause = true;
                base.GetComponent<BoomerangProjectile>().enabled = false;
                return;
            }
            if (stop == true) {
                body.velocity = Vector3.zero;
            };
            if (pause == true) {
                stopwatch += Time.fixedDeltaTime;
                body.velocity = Vector3.zero;
                if (stopwatch >= 2) {
                    Unpause();
                }

            }
        }

        public void OnDestroy() {   
            owner.gameObject?.GetComponent<MAIDManager>()?.GetMAID();
           owner.gameObject?.GetComponent<SkillLocator>()?.special?.SetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Contextual); ; }


        public void Unpause()
        {
            pause = false;
            base.GetComponent<BoomerangProjectile>().enabled = true;
        }

        public void FullStop() {
            stop = true;
            base.GetComponent<BoomerangProjectile>().enabled = false;
        }
    }
}
