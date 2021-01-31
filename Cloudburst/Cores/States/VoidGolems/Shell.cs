using EntityStates;
using Cloudburst.Cores.HAND;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using EntityStates.LunarGolem;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.VoidGolems
{
    class EngageShell : BaseSkillState
    {
        private bool readyToActivate;

        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shell.baseDuration / this.attackSpeedStat;
            Util.PlaySound(Shell.preShieldSoundString, base.gameObject);
            base.PlayCrossfade("Gesture, Additive", "PreShield", 0.2f);
            EffectManager.SimpleMuzzleFlash(Shell.preShieldEffect, base.gameObject, "Center", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= Shell.preShieldAnimDuration && !this.readyToActivate)
            {
                this.readyToActivate = true;
                Util.PlaySound(Shell.shieldActivateSoundString, base.gameObject);
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(BuffIndex.LunarShell, 25);
                }
                if (base.isAuthority)
                {
                    outer.SetNextStateToMain(); ;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}