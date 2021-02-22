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
        readonly BullseyeSearch bullseyeSearch = new BullseyeSearch();

        public void Start() {
            owner = new ProjectileOwnerInfo(GetComponent<ProjectileController>().owner, "fug");
            bullseyeSearch.searchOrigin = base.transform.position;
            bullseyeSearch.maxDistanceFilter = 500;
            bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(owner.characterBody.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(owner.gameObject);
        }
        private void OnDestroy() {
            float num = 22.5f;
            Vector3 point = Vector3.ProjectOnPlane(owner.characterBody.inputBank.aimDirection, Vector3.up);
            int num2 = 0;
            var target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
            while ((float)num2 < 16f)
            {
                Vector3 forward = Quaternion.AngleAxis(num * (float)num2, Vector3.up) * point;

                ProjectileManager.instance.FireProjectile(EntityStates.BrotherMonster.Weapon.FireLunarShards.projectilePrefab/*FistSlam.waveProjectilePrefab*/, base.transform.position, Util.QuaternionSafeLookRotation(forward), owner.gameObject, owner.characterBody.damage * 5, FistSlam.waveProjectileForce, owner.characterBody.RollCrit(), DamageColorIndex.Default, target.healthComponent.gameObject, -1f);
                num2++;
            }
        }
    }
}