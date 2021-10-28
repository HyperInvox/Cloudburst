using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Captain
{
    public class CallOrbitalLaser : BaseSkillState
    {
        public static float baseDuration = 1f;
        private Animator modelAnimator;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                base.PlayAnimation("Gesture, Override", "CallSupplyDrop", "CallSupplyDrop.playbackRate", this.duration);
                base.PlayAnimation("Gesture, Additive", "CallSupplyDrop", "CallSupplyDrop.playbackRate", this.duration);
                AddRecoil(0f, 0f, 1f, 1f);
            }
            if (characterBody)
            {
                characterBody.SetAimTimer(2f);
            }

            if (NetworkServer.active)
            {
                Vector3 position = transform.position;
                RaycastHit raycastHit;
                if (Physics.Raycast(GetAimRay(), out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
                {
                    position = raycastHit.point;
                }
                GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/NetworkedObjects/OrbitalLaser"), position, Quaternion.identity);
                gameObject.GetComponent<OrbitalLaserController>().ownerBody = characterBody;
                NetworkServer.Spawn(gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}