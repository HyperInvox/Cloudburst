using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class EmitVoid : MonoBehaviour
    {
        public float maxTrackingDistance = 20f;
        public float maxTrackingAngle = 20f;
        private BullseyeSearch bullseyeSearch = new BullseyeSearch();

        private CharacterBody body;
        private float stopwatch;
        public void Awake()
        {
            body = base.GetComponent<CharacterBody>();
        }

        public void Start()
        {
        }

        private HurtBox SearchForTarget()
        {
            bullseyeSearch.searchOrigin = transform.position;
            bullseyeSearch.maxDistanceFilter = 200;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            return bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
        }

        private Vector3 FindBestPosition(HurtBox target)
        {
            float radius = 15f;
            var originPoint = target.transform.position;
            originPoint.x += UnityEngine.Random.Range(-radius, radius);
            originPoint.z += UnityEngine.Random.Range(-radius, radius);
            originPoint.y += UnityEngine.Random.Range(radius, radius);
            return originPoint;
        }

        public void FixedUpdate()
        {
            stopwatch += Time.deltaTime;
            if (stopwatch >= 10 && body && !body.outOfCombat)
            {
                stopwatch = 0;

                var target = SearchForTarget();
                var pos = FindBestPosition(target);

                EffectData data = new EffectData()
                {
                    rotation = Quaternion.Euler(target.transform.forward),
                    scale = 15,
                    origin = pos,
                };
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), data, true);
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    crit = false,
                    damage = 0.2f * body.damage,
                    damageColorIndex = RoR2.DamageColorIndex.Default,
                    damageTypeOverride = DamageType.Generic,
                    force = 0,
                    owner = base.gameObject,
                    position = pos,
                    procChainMask = default,
                    projectilePrefab = ProjectileCore.orbitalOrb,
                    rotation = Util.QuaternionSafeLookRotation(body.inputBank.GetAimRay().direction),
                    target = null,
                    useFuseOverride = false,
                    useSpeedOverride = true,
                    _speedOverride = 100
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

    }
}
