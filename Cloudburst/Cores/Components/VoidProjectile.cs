using EntityStates.BrotherMonster;
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
        private ProjectileOwnerInfo owner;

        public void Start() {
            owner = new ProjectileOwnerInfo(GetComponent<ProjectileController>().owner, "fug");
        }
        private void OnDestroy() {
            float num = 22.5f;
            Vector3 point = Vector3.ProjectOnPlane(owner.characterBody.inputBank.aimDirection, Vector3.up);
            int num2 = 0;
            while ((float)num2 < 16f)
            {
                Vector3 forward = Quaternion.AngleAxis(num * (float)num2, Vector3.up) * point;
                ProjectileManager.instance.FireProjectile(FistSlam.waveProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(forward), owner.gameObject, owner.characterBody.damage * 5, FistSlam.waveProjectileForce, owner.characterBody.RollCrit(), DamageColorIndex.Default, null, -1f);
                num2++;
            }
        }
    }
}