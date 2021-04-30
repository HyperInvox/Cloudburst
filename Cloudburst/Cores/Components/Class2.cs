using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    public class ProjectileEffectManager : MonoBehaviour
    {
        private ProjectileController controller;
        private GameObject effectInstance;
        public GameObject effect;

        public void Awake()
        {
            controller = GetComponent<ProjectileController>();
        }

        public void Start()
        {
            effectInstance = Instantiate<GameObject>(effect, controller.ghost.transform, true);
        }

        public void OnDestroy() {
            if (effectInstance) {
                Destroy(effectInstance);
            }
        }
    }
}