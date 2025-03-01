﻿using System;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.MegaMushrum
{
    public class Unplant : BaseState
    {
        public static GameObject plantEffectPrefab = EntityStates.MiniMushroom.UnPlant.plantEffectPrefab;
        public static float baseDuration = EntityStates.MiniMushroom.UnPlant.baseDuration;
        public static string UnplantOutSoundString = EntityStates.MiniMushroom.UnPlant.UnplantOutSoundString;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration / this.attackSpeedStat;
            EffectManager.SimpleMuzzleFlash(plantEffectPrefab, base.gameObject, "BurrowCenter", false);
            Util.PlaySound(UnplantOutSoundString, base.gameObject);
            base.PlayAnimation("Plant", "PlantEnd", "PlantEnd.playbackRate", this.duration);
            for (float num = 0f; num < 9f; num += 1f)
            {
                float num2 = 6.2831855f;
                Vector3 forward = new Vector3(Mathf.Cos(num / 9f * num2), 0f, Mathf.Sin(num / 9f * num2));
                var projInfo = new FireProjectileInfo
                {
                    crit = base.RollCrit(),
                    damage = this.damageStat * (SporeGrenades.damageCoefficient * 1.2f),
                    owner = base.gameObject,
                    position = base.transform.position,
                    projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/Sunder"),
                    rotation = Quaternion.LookRotation(forward),
                    damageColorIndex = DamageColorIndex.Default,
                    force = 2500,
                    procChainMask = default
                };
                ProjectileManager.instance.FireProjectile(projInfo);
            }
        }
        public override void OnExit()
        {
            base.PlayAnimation("Plant, Additive", "Empty");
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}