﻿using System;
using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.MegaMushrum
{
    public class SporeGrenades : BaseState
    {
        public static GameObject chargeEffectPrefab = EntityStates.MiniMushroom.SporeGrenade.chargeEffectPrefab;
        public static string attackSoundString = EntityStates.MiniMushroom.SporeGrenade.attackSoundString;
        public static string chargeUpSoundString = EntityStates.MiniMushroom.SporeGrenade.chargeUpSoundString;
        public static float recoilAmplitude = 1f;
        public static GameObject projectilePrefab = EntityStates.MiniMushroom.SporeGrenade.projectilePrefab;
        public static float baseDuration = 2f;
        public static string muzzleString = EntityStates.MiniMushroom.SporeGrenade.muzzleString;
        public static float damageCoefficient = EntityStates.MiniMushroom.SporeGrenade.damageCoefficient * 2;
        public static float timeToTarget = 3f;
        public static float projectileVelocity = 25;
        public static float minimumDistance = 10;
        public static float maximumDistance = 30;
        public static float baseChargeTime = 2f;
        private uint chargeupSoundID;
        private float duration;
        private float chargeTime;
        private bool hasFired;
        private Animator modelAnimator;
        private GameObject chargeEffectInstance;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = SporeGrenades.baseDuration / this.attackSpeedStat;
            this.chargeTime = SporeGrenades.baseChargeTime / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();
            //Debug.Log(""+ EntityStates.MiniMushroom.SporeGrenade.projectilePrefab.name.ToString() + "");
            if (this.modelAnimator)
            {
                this.modelAnimator.SetBool("isCharged", false);
                base.PlayAnimation("Gesture, Additive", "Charge");
                this.chargeupSoundID = Util.PlaySound(SporeGrenades.chargeUpSoundString, base.characterBody.modelLocator.modelTransform.gameObject);
            }
            Transform transform = base.FindModelChild("ChargeSpot");
            if (transform)
            {
                this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(SporeGrenades.chargeEffectPrefab, transform);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.chargeTime)
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    Animator animator = this.modelAnimator;
                    if (animator != null)
                    {
                        animator.SetBool("isCharged", true);
                    }
                    if (base.isAuthority)
                    {
                        this.FireGrenade(SporeGrenades.muzzleString);
                    }
                }
                if (base.isAuthority && base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }
        public override void OnExit()
        {
            base.PlayAnimation("Gesture, Additive", "Empty");
            AkSoundEngine.StopPlayingID(this.chargeupSoundID);
            if (this.chargeEffectInstance)
            {
                EntityState.Destroy(this.chargeEffectInstance);
            }
            base.OnExit();
        }
        private void FireGrenade(string targetMuzzle)
        {
            Ray aimRay = base.GetAimRay();
            Ray ray = new Ray(aimRay.origin, Vector3.up);
            Transform transform = base.FindModelChild(targetMuzzle);
            if (transform)
            {
                ray.origin = transform.position;
            }
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            if (base.teamComponent)
            {
                bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
            }
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
            bullseyeSearch.RefreshCandidates();
            HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
            bool flag = false;
            Vector3 a = Vector3.zero;
            RaycastHit raycastHit;
            if (hurtBox)
            {
                a = hurtBox.transform.position;
                flag = true;
            }
            else if (Physics.Raycast(aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
            {
                a = raycastHit.point;
                flag = true;
            }
            float magnitude = SporeGrenades.projectileVelocity;
            if (flag)
            {
                Vector3 vector = a - ray.origin;
                Vector2 a2 = new Vector2(vector.x, vector.z);
                float magnitude2 = a2.magnitude;
                Vector2 vector2 = a2 / magnitude2;
                if (magnitude2 < SporeGrenades.minimumDistance)
                {
                    magnitude2 = SporeGrenades.minimumDistance;
                }
                if (magnitude2 > SporeGrenades.maximumDistance)
                {
                    magnitude2 = SporeGrenades.maximumDistance;
                }
                float y = Trajectory.CalculateInitialYSpeed(SporeGrenades.timeToTarget, vector.y);
                float num = magnitude2 / SporeGrenades.timeToTarget;
                Vector3 direction = new Vector3(vector2.x * num, y, vector2.y * num);
                magnitude = direction.magnitude;
                ray.direction = direction;
            }
            for (var i = 0; i < 3; i++)
            {
                Quaternion rotation = Util.QuaternionSafeLookRotation(ray.direction + UnityEngine.Random.insideUnitSphere * 0.05f);
                ProjectileManager.instance.FireProjectile(SporeGrenades.projectilePrefab, ray.origin, rotation, base.gameObject, this.damageStat * SporeGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, magnitude);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}