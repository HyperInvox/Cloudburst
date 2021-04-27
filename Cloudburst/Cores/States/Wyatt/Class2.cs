using System;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.States.Nemmando
{
    class BladeOfCessation2 : BaseSkillState
    {

        public static string hitboxString = "SwordHitbox";

        public static float damageCoefficient = 1.6f;
        public static float procCoefficient = 1f;

        public static float baseDuration = 1.1f;
        public static float baseEarlyExitTime = 0.65f;

        public static float hitboxStartTime = 0.07f;
        public static float hitboxEndTime = 0.469f;

        public static float baseEffectTime = 0.1f;

        public static float attackRecoil = 0.5f;
        public static float hitHopVelocity = 5f;
        public static float hitPauseDuration = 0.1f;
        public static float hitBloom = 0.5f;

        public int currentSwing;

        private OverlapAttack overlapAttack;
        private Animator animator;
        //private ChildLocator childLocator;
        //private Transform modelBaseTransform;

        private float duration;
        private float earlyExitDuration;

        private bool hasFired;
        private bool hasHopped;
        private bool playedEffect;

        private float stopwatch;
        private float hitPauseTimer;
        private BaseState.HitStopCachedState hitStopCachedState;
        private bool inHitPause;
        private float effectTime;

        public override void OnEnter()
        {

            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            this.earlyExitDuration = this.duration * baseEarlyExitTime;
            this.effectTime = duration * baseEffectTime;
            this.hasFired = false;

            this.animator = base.GetModelAnimator();
            //this.childLocator = base.GetModelChildLocator();
            //this.modelBaseTransform = base.GetModelBaseTransform();

            HitBoxGroup hitBoxGroup = base.FindHitBoxGroup(hitboxString);

            this.overlapAttack = new OverlapAttack();
            this.overlapAttack.attacker = base.gameObject;
            this.overlapAttack.inflictor = base.gameObject;
            this.overlapAttack.teamIndex = base.GetTeam();
            this.overlapAttack.damage = BladeOfCessation2.damageCoefficient * base.damageStat;
            this.overlapAttack.procCoefficient = BladeOfCessation2.procCoefficient;
            this.overlapAttack.forceVector = Vector3.zero;
            this.overlapAttack.pushAwayForce = 169f;
            this.overlapAttack.hitBoxGroup = hitBoxGroup;
            this.overlapAttack.isCrit = base.RollCrit();

            Util.PlaySound(EntityStates.Merc.Weapon.GroundLight2.slash1Sound, base.gameObject);

            PlayAnimation();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
               // if (this.animator) this.animator.SetFloat("Primary.rate", 0f);
            }

            if (this.stopwatch >= this.duration * hitboxStartTime && this.stopwatch <= this.duration * hitboxEndTime)
            {
                this.FireAttack();
            }

            if (this.fixedAge > this.effectTime && !this.playedEffect)
            {
                this.playedEffect = true;
                string swingMuzzle = "SwingLeft";
                if (this.currentSwing > 0) swingMuzzle = "SwingRight";
            }

            if (base.fixedAge >= this.earlyExitDuration && base.inputBank.skill1.down)
            {
                var nextSwing = new BladeOfCessation2();
                nextSwing.currentSwing = this.currentSwing + 1;
                this.outer.SetNextState(nextSwing);
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void PlayAnimation()
        {
            string swingAnimState = currentSwing % 2 == 0 ? "Primary1" : "Primary2";

            bool moving = this.animator.GetBool("isMoving");
            bool grounded = this.animator.GetBool("isGrounded");

            if (!moving && grounded)
            {
                base.PlayCrossfade("FullBody, Override", swingAnimState, "Primary.rate", this.duration, 0.05f);
            }
            else
            {
                base.PlayCrossfade("UpperBody, Override", swingAnimState, "Primary.rate", this.duration, 0.05f);
            }
        }

        public void FireAttack()
        {

            if (!this.hasFired)
            {
                this.hasFired = true;

                base.AddRecoil(-1f * BladeOfCessation2.attackRecoil,
                               -2f * BladeOfCessation2.attackRecoil,
                               -0.5f * BladeOfCessation2.attackRecoil,
                               0.5f * BladeOfCessation2.attackRecoil);
            }

            if (base.isAuthority)
            {

                if (this.overlapAttack.Fire())
                {

                    Util.PlaySound(EntityStates.Merc.GroundLight.hitSoundString, base.gameObject);
                    base.characterBody.AddSpreadBloom(BladeOfCessation2.hitBloom);

                    base.AddRecoil(-1f * BladeOfCessation2.attackRecoil,
                                   -2f * BladeOfCessation2.attackRecoil,
                                   -0.5f * BladeOfCessation2.attackRecoil,
                                   0.5f * BladeOfCessation2.attackRecoil);

                    if (!this.hasHopped)
                    {
                        if (base.characterMotor && !base.characterMotor.isGrounded)
                        {
                            base.SmallHop(base.characterMotor, BladeOfCessation2.hitHopVelocity);
                        }

                        this.hasHopped = true;
                    }

                    if (!this.inHitPause)
                    {
                        this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Primary.rate");
                        this.hitPauseTimer = (BladeOfCessation2.hitPauseDuration) / this.attackSpeedStat;
                        this.inHitPause = true;
                    }
                }
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)this.currentSwing);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.currentSwing = (int)reader.ReadByte();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            //if (base.fixedAge >= this.durationBeforeInterruptable)
            //{
            //	return InterruptPriority.Any;
            //}
            return InterruptPriority.Skill;
        }
    }
}
