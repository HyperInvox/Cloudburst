using RoR2;
using RoR2.Projectile;

using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    [RequireComponent(typeof(ProjectileTargetComponent))]
    public class OrbitingProjectileBehaviour : NetworkBehaviour
    {
        private ProjectileTargetComponent targetFinder;

        // sync it across the network
        [SyncVar]
        private GameObject target;

        public Transform center;
        public Vector3 axis = Vector3.up;
        public Vector3 desiredPosition;

        //sync all of these assholes 
        public GameObject effect;
        public EffectData data;

        [SyncVar]
        public float radius = 2.0f;

        [SyncVar]
        public float radiusSpeed = 0.5f;

        [SyncVar]
        public float rotationSpeed = 80.0f;
        
        private void Awake() {
            targetFinder = base.GetComponent<ProjectileTargetComponent>();

            //just to be safe!
            //target = targetFinder.target.gameObject;
        }

        void Start()
        {
            target = targetFinder.target.gameObject;

            center = target.transform;
            transform.position = (transform.position - center.position).normalized * radius + center.position;
            radius = 2.0f;
        }

        void FixedUpdate()
        {
            if (!target)
            {
                if (effect) {
                    EffectManager.SpawnEffect(effect, data, true);
                }
                Destroy(base.gameObject);
                return;
            }
            if (NetworkServer.active)
            {
                transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
                desiredPosition = (transform.position - center.position).normalized * radius + center.position;
                transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
            }
        }
    }
}