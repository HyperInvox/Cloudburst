using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class EmitVoidBrother : MonoBehaviour
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

                if (PhaseCounter.instance) {
                    var target = SearchForTarget();
                    switch (PhaseCounter.instance.phase) {
                        case 1:
                            var pos1 = FindBestPosition(target);

                            EffectData data = new EffectData()
                            {
                                rotation = Quaternion.Euler(target.transform.forward),
                                scale = 15,
                                origin = pos1,
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
                                position = pos1,
                                procChainMask = default,
                                projectilePrefab = ProjectileCore.orbitalOrb,
                                rotation = Util.QuaternionSafeLookRotation(body.inputBank.GetAimRay().direction),
                                target = null,
                                useFuseOverride = false,
                                useSpeedOverride = true,
                                _speedOverride = 100
                            };
                            ProjectileManager.instance.FireProjectile(info);
                            break;
                        case 2:
                            break;
                        case 3:
                            for (int i = 0; i < 2; i++)
                            {
                                var pos2 = FindBestPosition(target);

                                EffectData adata = new EffectData()
                                {
                                    rotation = Quaternion.Euler(target.transform.forward),
                                    scale = 15,
                                    origin = pos2,
                                };
                                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), adata, true);
                                FireProjectileInfo ainfo = new FireProjectileInfo()
                                {
                                    crit = false,
                                    damage = 0.2f * body.damage,
                                    damageColorIndex = RoR2.DamageColorIndex.Default,
                                    damageTypeOverride = DamageType.Generic,
                                    force = 0,
                                    owner = base.gameObject,
                                    position = pos2,
                                    procChainMask = default,
                                    projectilePrefab = ProjectileCore.orbitalOrb,
                                    rotation = Util.QuaternionSafeLookRotation(body.inputBank.GetAimRay().direction),
                                    target = null,
                                    useFuseOverride = false,
                                    useSpeedOverride = true,
                                    _speedOverride = 100
                                };
                                ProjectileManager.instance.FireProjectile(ainfo);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < 3; i++) {
                                var pos3 = FindBestPosition(target);
                                EffectData ndata = new EffectData()
                                {
                                    rotation = Quaternion.Euler(target.transform.forward),
                                    scale = 15,
                                    origin = pos3,
                                };
                                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), ndata, true);
                                FireProjectileInfo ninfo = new FireProjectileInfo()
                                {
                                    crit = false,
                                    damage = 0.4f * body.damage,
                                    damageColorIndex = RoR2.DamageColorIndex.Default,
                                    damageTypeOverride = DamageType.Generic,
                                    force = 0,
                                    owner = base.gameObject,
                                    position = pos3,
                                    procChainMask = default,
                                    projectilePrefab = ProjectileCore.orbitalOrb,
                                    rotation = Util.QuaternionSafeLookRotation(body.inputBank.GetAimRay().direction),
                                    target = null,
                                    useFuseOverride = false,
                                    useSpeedOverride = true,
                                    _speedOverride = 100
                                };
                                ProjectileManager.instance.FireProjectile(ninfo);
                            }
                            break;
                    }
                }
            }
        }

    }
}
