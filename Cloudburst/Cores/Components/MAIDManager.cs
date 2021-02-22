using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class MAIDManager : NetworkBehaviour
    {
        private GameObject maid;
        private GameObject winch;
        private bool startReel;
        private float _stopwatch = 0;

        private SkillLocator skillLocator;
        private CharacterBody body;
        private CharacterMotor characterMotor;
        private CharacterDirection characterDirection;
        public void Awake()
        {
            skillLocator = GetComponent<SkillLocator>();
            characterMotor = GetComponent<CharacterMotor>();
            body = GetComponent<CharacterBody>();
            characterDirection = GetComponent<CharacterDirection>();
            //click and she's gone!
        }

        private void FixedUpdate()
        {
            if (startReel == true && maid)
            {
                body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                Vector3 lossyScale = maid.transform.lossyScale;
                var volume = lossyScale.x * 2f * (lossyScale.y * 2f) * (lossyScale.z * 2f);

                Vector3 velocity = (maid.transform.position - base.transform.position).normalized * 120f;
                characterMotor.velocity = velocity;
                characterDirection.forward = characterMotor.velocity.normalized;
                //float distance = volume;

                if (_stopwatch > 5)
                {
                    CloudburstPlugin.Destroy(maid);
                    CloudburstPlugin.Destroy(winch);
                    body.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                    characterMotor.velocity = Vector3.zero;
                    startReel = false;
                    RpcSetDeploy();

                }
                float distance = Vector3.Distance(base.transform.position, maid.transform.position);
                if (distance <= 1.185805)
                {
                    body.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

                    CloudburstPlugin.Destroy(maid);
                    CloudburstPlugin.Destroy(winch);
                    characterMotor.velocity = Vector3.up * 30f;
                    startReel = false;
                    RpcSetDeploy();
                }
            }
        }
        #region Deployment
        public void DeployMAIDAuthority(GameObject maid)
        {
            if (NetworkServer.active)
            {
                DeployMAIDInternal(maid);
                return;
            }
            CmdDeployMAIDInternal(maid);
        }

        public GameObject GetWinch(GameObject winch) {
            if (this.winch) {
                Destroy(this.winch);

            }
            this.winch = winch;

            return maid;
        }

        public void GetMAID() {
            RpcSetRetrieve();
            Destroy(winch);
        }

        [Server]
        private void DeployMAIDInternal(GameObject maid)
        {
            //      CloudburstPlugin.Destroy(this.maid);
            //    CloudburstPlugin.Destroy(winch);
            LogCore.LogI("Deployed maid!");
            this.maid = maid;
            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
            {
                crit = false,
                damage = 0,
                damageColorIndex = DamageColorIndex.Default,
                force = 0f,
                owner = base.gameObject,
                position = maid.transform.position,
                procChainMask = default(ProcChainMask),
                projectilePrefab = ProjectileCore.winch,
                rotation = Quaternion.identity,
                target = maid.gameObject,
                useSpeedOverride = true,
                speedOverride = 500
            };
            EffectManager.SimpleMuzzleFlash(Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashWinch"), base.gameObject, "WinchHole", true);
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            RpcSetRetrieve();
            LogCore.LogI(skillLocator.special.skillDef.skillName);
        }

        [Command]
        private void CmdDeployMAIDInternal(GameObject maid)
        {
            DeployMAIDInternal(maid);
        }
        #endregion

        #region Retrival
        [Server]
        private void RetrieveMAIDInternal()
        {
            LogCore.LogI("Retrieved maid!");
            if (maid)
            {
                LogCore.LogI("maid exists");
                startReel = true;
                RpcSetDeploy();
                maid.GetComponent<MAID>().FullStop();
                //Destroy(maid);
            }
        }

        [Command]
        private void CmdRetrieveMAIDInternal()
        {
            RetrieveMAIDInternal();
        }

        public void RetrieveMAIDAuthority()
        {
            if (NetworkServer.active)
            {
                RetrieveMAIDInternal();
                return;
            }
            CmdRetrieveMAIDInternal();
        }
        #endregion


        [ClientRpc]
        private void RpcSetDeploy()
        {
            skillLocator.special.UnsetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Replacement);
            skillLocator.special.SetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Replacement);
        }
        [ClientRpc]
        private void RpcSetRetrieve()
        {
            skillLocator.special.UnsetSkillOverride(this, Custodian.retrievePrimary, GenericSkill.SkillOverridePriority.Replacement);

            skillLocator.special.SetSkillOverride(this, Custodian.retrievePrimary, GenericSkill.SkillOverridePriority.Replacement);
        }
    }
}
