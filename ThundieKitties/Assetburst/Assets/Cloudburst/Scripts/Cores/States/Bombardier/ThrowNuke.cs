﻿using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Toolbot;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cloudburst.Cores.States.Bombardier
{
    public class ThrowNuke : AimThrowableBase
    {
        private float duration = 1;
        private AimStunDrone _goodState;
        private ThrowGrenade _grenade;
        public ThrowNuke()
        {
            if (_goodState == null || _grenade == null)
            {
                _goodState = EntityStateCatalog.InstantiateState(typeof(AimStunDrone)) as AimStunDrone;
                _grenade = EntityStateCatalog.InstantiateState(typeof(ThrowGrenade)) as ThrowGrenade;
            }
            maxDistance = 1000;
            rayRadius = _goodState.rayRadius;
            arcVisualizerPrefab = _goodState.arcVisualizerPrefab;
            projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile");
            endpointVisualizerPrefab = _goodState.endpointVisualizerPrefab;
            endpointVisualizerRadiusScale = _goodState.endpointVisualizerRadiusScale;
            setFuse = false; //_goodSate.setFuse;
            damageCoefficient = _goodState.damageCoefficient;
            baseMinimumDuration = 0.1f;
            //rayRadius = _goodState.rayRadius;
            //rayRadius = _goodState.rayRadius;
        }
        public override void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
        {
            string muzzleName = "MuzzleCenter";

            if (_grenade.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(_grenade.effectPrefab, base.gameObject, muzzleName, false);
            }

            base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
            base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}