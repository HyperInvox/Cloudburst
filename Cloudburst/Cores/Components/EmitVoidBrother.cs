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
        private float stopwatch2;

        bool phase4 = false; bool phase3 = false;
        bool spawned = false;

        private float limit = 3;
        private float summonLimit = 60;

        public void Awake()
        {
            body = base.GetComponent<CharacterBody>();
        }

        public void Start()
        {
            if (PhaseCounter.instance)
            {

                switch (PhaseCounter.instance.phase)
                {
                    case 1:
                        limit = 16;
                        break;
                    case 3:
                        limit = 13;
                        summonLimit = 50;
                        phase3 = true;
                        break;
                    case 4:
                            limit = 10;
                            phase4 = true;

                        break;
                }
            }
        }

        private HurtBox SearchForTarget()
        {
            bullseyeSearch.searchOrigin = transform.position;
            bullseyeSearch.maxDistanceFilter = 5000;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            return bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
        }

        private void SpawnGlass() {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBrotherGlass"), new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 3f,
                maxDistance = 20f,
                spawnOnTarget = base.gameObject.transform
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = base.gameObject;
            DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
            directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
            {
                spawnResult.spawnedInstance.GetComponent<Inventory>().GiveItem(ItemIndex.HealthDecay, 65);
            }));
            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
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

        private Vector3 FindBestClosePosition(HurtBox target)
        {
            var originPoint = target.transform.position;
            originPoint += UnityEngine.Random.insideUnitSphere * 16;
            return originPoint;
        }



        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                stopwatch += Time.deltaTime;



                stopwatch2 += Time.deltaTime;
                if (stopwatch2 >= 15 && spawned == false)
                {

                    for (int i = 0; i < 2; i++)
                    {
                        SpawnGlass();
                    }
                    spawned = true;
                }


                if (phase4)
                {

                    stopwatch2 += Time.deltaTime;
                    if (stopwatch2 >= 10 && spawned == false)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            SpawnGlass();
                        }
                        spawned = true;
                    }
                }

                if (PhaseCounter.instance && stopwatch >= limit)
                {
                    stopwatch = 0;
                    var target = SearchForTarget();
                    var pos = FindBestPosition(target);

                    if (Util.HasEffectiveAuthority(gameObject))
                    {
                        switch (PhaseCounter.instance.phase)
                        {
                            case 1:
                                Phase1(pos, target);
                                break;
                            case 2:
                                break;
                            case 3:
                                for (int i = 0; i < 2; i++)
                                {
                                    Phase1(FindBestPosition(target), target);
                                    Phase3(pos, target);
                                }
                                break;
                            case 4:
                                for (int i = 0; i < 3; i++)
                                {
                                    Phase1(pos, target);
                                }
                                Phase3(pos, target);
                                Phase4(pos, target);

                                break;
                        }
                    }
                }
            }
        }
        public void Phase4(Vector3 position, HurtBox target)
        {

        }

        public void Phase3(Vector3 positiona, HurtBox target)
        {
            //I'M THE BOMB
            var position = FindBestClosePosition(target);

            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = false,
                damage = 4f * body.damage,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = DamageType.Nullify,
                force = 0,
                owner = base.gameObject,
                position = position,
                procChainMask = default,
                projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/NullifierPreBombProjectile"),
                rotation = Util.QuaternionSafeLookRotation(body.inputBank.GetAimRay().direction),
                target = null,
                useFuseOverride = false,
                useSpeedOverride = true,
                _speedOverride = 100
            };
            ProjectileManager.instance.FireProjectile(info);

        }

        public void Phase1(Vector3 position, HurtBox target) {
            EffectData data = new EffectData()
            {
                rotation = Quaternion.Euler(target.transform.forward),
                scale = 15,
                origin = position,
            };
            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), data, true);
            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = false,
                damage = .7f * body.damage,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = DamageType.Generic,
                force = 0,
                owner = base.gameObject,
                position = position,
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
