using EntityStates.CaptainDefenseMatrixItem;
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
        private BasicOwnerInfo owner = default;
        private BoomerangProjectile boomer;
        private float pauseStopwatch;
        private List<Rigidbody> bodies;
        private Vector3 distance;
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
            owner = new BasicOwnerInfo(controller.owner, "");

            owner.gameObject.GetComponent<MAIDManager>().DeployMAIDAuthority(base.gameObject);
            boomer.onFlyBack.AddListener(OnHit);
        }

        private void OnHit() {

        }

        private bool PassesFilter(TeamFilter filter) {
            if (filter.teamIndex != TeamIndex.Player) {
                return true;
            }
            return false;
        }


        private void OnTriggerEnter(Collider other) {
            Rigidbody component = other.GetComponent<Rigidbody>();
            ProjectileController controller = other.GetComponent<ProjectileController>();
            //prevent multiple cringes from being cringed!
            if (component && controller && PassesFilter(controller.teamFilter) && !controller.gameObject.HasComponent<CringeDepartment>()) {
                EffectData effectData = new EffectData
                {
                    origin = base.transform.position,
                    //pls god
                    start = component.transform.position
                };
                EffectManager.SpawnEffect(/*DefenseMatrixOn.tracerEffectPrefab*/EffectCore.maidTouchEffect, effectData, true);

                EffectData nads = new EffectData
                {
                    origin = controller.transform.position,
                    scale = 1,
                    //pls vs
                };
                EffectManager.SpawnEffect(EffectCore.maidCleanseEffect, nads, true);

                Util.PlaySound("step_land_shallow_water_01", component.gameObject);

                var cing = controller.AddComponent<CringeDepartment>();
                cing.maxVelocityMagnitude = 3;
                cing.antiGravity = 1;
            }
        }

        public void FixedUpdate() {

            if (boomer.stopwatch >= boomer.maxFlyStopwatch && triggered == false)
            {
                triggered = true;
                pause = true;
                base.GetComponent<ProjectileProximityBeamController>().enabled = true;
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
            LogCore.LogI(stop);

            owner.gameObject.GetComponent<MAIDManager>().Invoke(stop, distance);

            //owner.gameObject?.GetComponent<SkillLocator>()?.special?.SetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Contextual); ; }
        }

        public void Unpause()
        {
            pause = false;
            base.GetComponent<ProjectileProximityBeamController>().enabled = false;
            base.GetComponent<BoomerangProjectile>().enabled = true;
        }

        public void FullStop() {
            stop = true;
            distance = (base.transform.position - owner.gameObject.transform.position).normalized;// * 120f;
            base.GetComponent<BoomerangProjectile>().enabled = false;
        }
    }
}
