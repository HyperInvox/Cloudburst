using System;
using System.Security.Policy;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.HAND
{
    class Cleanup : BasicMeleeAttack
    {
        public float charge = 0f;
        public static float recoilAmplitude = 0.5f;
        public static float baseDurationBeforeInterruptable = 0.5f;
        public float bloom = 1f;

        private string animationStateName;

        public float damageMultiplier   = 1;
        private float durationBeforeInterruptable;
        private float minDamageCoef = 1.5f;
        private float maxDamageCoef = 2.5f;

        public override bool allowExitFire
        {
            get
            {
                return base.characterBody && !base.characterBody.isSprinting;
            }
        }

        public override void OnEnter()
        {
            this.hitBoxGroupName = "TempHitbox";
            this.baseDuration = 0.5f;
            this.hitPauseDuration = 0.01f;
            damageCoefficient = 4;
            swingEffectPrefab = Resources.Load<GameObject>("prefabs/effects/handslamtrail");
            hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfxmedium");
            base.OnEnter();
            base.characterDirection.forward = base.GetAimRay().direction;
            this.durationBeforeInterruptable = Cleanup.baseDurationBeforeInterruptable / this.attackSpeedStat;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType = DamageType.Generic;
        }

        public override void PlayAnimation()
        {
            //this.animationStateName = "Secondary5";
            //base.PlayAnimation("FullBody, Override", this.animationStateName, "GroundLight.playbackRate", this.duration);// ; (, this.animationStateName, , this.duration, 0.05f);
        }

        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            base.characterBody.AddSpreadBloom(this.bloom);
        }

        public override void BeginMeleeAttackEffect()
        {
            this.swingEffectMuzzleString = this.animationStateName;
            base.BeginMeleeAttackEffect();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.durationBeforeInterruptable)
            {
                return InterruptPriority.Skill;
            }
            return InterruptPriority.PrioritySkill;
        }

    }
}
