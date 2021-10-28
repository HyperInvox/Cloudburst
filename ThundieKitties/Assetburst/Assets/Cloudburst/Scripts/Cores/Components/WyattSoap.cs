    using Cloudburst.Cores.States.Wyatt;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cloudburst.Cores.Components
{  
    class WyattSoap : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileController controller = null;
        private BasicOwnerInfo owner = default;

        public void Awake()
        {
            controller = base.GetComponent<ProjectileController>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = base.transform.position;
            bullseyeSearch.maxDistanceFilter = 100;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            bullseyeSearch.teamMaskFilter.RemoveTeam(owner.characterBody.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            bullseyeSearch.GetResults().ToList().ForEach(FireBullet);

            void FireBullet(HurtBox hurtBox)
            {
                Vector3 vector = base.transform.position;
                if (hurtBox)
                {
                    vector = hurtBox.transform.position - base.transform.position;
                }

                new BulletAttack
                {
                    owner = owner.gameObject,
                    weapon = base.gameObject,
                    origin = base.transform.position,
                    aimVector = vector.normalized,
                    minSpread = 0f,
                    maxSpread = 0f,
                    damage = 3 * owner.characterBody.damage,
                    force = 100f,
                    tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerGoldGat"),

                    hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/BulletImpactSoft"),
                    isCrit = owner.characterBody.RollCrit(),
                    damageType = DamageType.Stun1s
                }.Fire();
            }
        }

        public void Start()
        {
            owner = new BasicOwnerInfo(controller.owner, "Weapon");
        }
    }
}
