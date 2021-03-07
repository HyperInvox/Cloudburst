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
        public GameObject maid;
        public GameObject winch;
        public bool startReel;
        private float _stopwatch = 0;

        public event Action<bool, GenericSkill> OnRetrival;
        public event Action<GenericSkill> OnDeploy;
        public event Action sunset;

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

        public void Invoke(bool bo) {
            if (NetworkServer.active) {
                RpcSetDeploy(bo);
            }
        }

        private void FixedUpdate()
        {
            if (startReel == true && maid)
            {
                body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                Vector3 lossyScale = maid.transform.lossyScale;
                var volume = lossyScale.x * 2f * (lossyScale.y * 2f) * (lossyScale.z * 2f);

                _stopwatch += Time.fixedDeltaTime;

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
                    sunset.Invoke();
                    //RpcSetDeploy(true);

                }
                float distance = Vector3.Distance(base.transform.position, maid.transform.position);
                if (distance <= 1.185805)
                {
                    body.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

                    CloudburstPlugin.Destroy(maid);
                    CloudburstPlugin.Destroy(winch);
                    characterMotor.velocity = Vector3.up * 30f;
                    startReel = false;
                   // RpcSetDeploy(false);
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
           // OnRetrival.Invoke(true, skillLocator.special);
            //Destroy(winch);

        }

        [Server]
        private void DeployMAIDInternal(GameObject maid)
        {
            //      CloudburstPlugin.Destroy(this.maid);
            //    CloudburstPlugin.Destroy(winch);
            LogCore.LogI("Deployed maid!");
            this.maid = maid;
            RpcSetRetrieve();
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
                _stopwatch = 0;
                startReel = true;
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
        private void RpcSetDeploy(bool natRetrival)
        {
            LogCore.LogI("invoke");
            OnRetrival?.Invoke(natRetrival, skillLocator.special);
            //  skillLocator.special.UnsetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Replacement);
            //skillLocator.special.SetSkillOverride(this, Custodian.throwPrimary, GenericSkill.SkillOverridePriority.Replacement);
        }
        [ClientRpc]
        private void RpcSetRetrieve()
        {
            OnDeploy?.Invoke(skillLocator.special);
        }
    }
}
