using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class VoidProjectile : MonoBehaviour
    {
        private float stopwatch = 0;
        public float limit;
        private void FixedUpdate() {
            stopwatch += Time.deltaTime;
            if (stopwatch >= limit) {
                Destroy(gameObject);
            }
        }
        private void OnDestroy() {
            EffectData data = new EffectData()
            {
                rotation = transform.rotation,
                scale = 15,
                origin = transform.position,
            };
            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), data, true);
        }
    }
}