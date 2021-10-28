﻿using System;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Wyatt
{
    //TODO:
    //Fix the combo finisher being weird.

    class WyattBaseMeleeAttack : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {

        public int step = 0;
        public static float recoilAmplitude = 0.7f;
        public static float baseDurationBeforeInterruptable = 0.5f;
        public float bloom = 1f;
        //public static float comboFinisherBaseDuration = 0.5f;
        //public static float comboFinisherhitPauseDuration = 0.15f;
        //public static float comboFinisherBloom = 0.5f;
        //public static float comboFinisherBaseDurationBeforeInterruptable = 0.5f;
        //private string animationStateName;
        private float durationBeforeInterruptable;

        private bool isComboFinisher
        {
            get
            {
                return this.step == 2;
            }
        }

        private bool spawnEffect = false;
        private string animationStateName;

        public override bool allowExitFire
        {
            get
            {
                return base.characterBody && !base.characterBody.isSprinting;
            }
        }

        private GameObject obj
        {
            get
            {
                if (this.isComboFinisher)
                {
                    return EffectCore.wyattSwingTrail;

                }

                return EffectCore.wyatt2SwingTrail;

            }
        }

        public override void OnEnter()
        {
            //this.hitBoxGroupName = "TempHitbox";
            this.hitBoxGroupName = "TempHitboxLarge";
            this.mecanimHitboxActiveParameter = "BroomSwing.shittybasicmeleeattackparameter";
            this.baseDuration = 0.7f;
            this.duration = this.baseDuration / base.attackSpeedStat;
            this.hitPauseDuration = 0.1f;
            this.damageCoefficient = (2f + (characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex) * 0.1f));
            this.procCoefficient = 1f;

            spawnEffect = false;
            //swingEffectPrefab = Resources.Load<GameObject>("prefabs/effects/GrandparentGroundSwipeTrailEffect");
            hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfxmedium");
            //swingEffectMuzzleString = "WinchHole";//"//SwingTrail";

            /*var obj = CloudburstPlugin.Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlSpitter"), new Vector3(201f, -128.8f, 143f), Quaternion.Euler(new Vector3(0, -43.019f, 0)));

            obj.layer = LayerIndex.world.intVal;
            obj.transform.position = base.transform.position;
            obj.transform.localScale = new Vector3(10, 10, 10);
            NetworkServer.Spawn(obj);*/

            /*EffectManager.SpawnEffect(EffectCore.shaderEffect, new EffectData()
            {
                origin = base.transform.position,
            }, false);*/

            //LogCore.LogW(step);

            if (isComboFinisher)
            {
                // LogCore.LogW("finisher");
                this.hitBoxGroupName = "TempHitbox";
                forceVector = new Vector3(0, 1000, 0);
                //this.baseDuration = 1f;
                //this.duration = this.baseDuration / base.attackSpeedStat;
                this.hitPauseDuration = 0.2f;
                this.damageCoefficient = 4f;
            }
            //else { LogCore.LogW("not finisher"); }
            
            base.OnEnter();
            base.characterDirection.forward = base.GetAimRay().direction;
            base.characterMotor.ApplyForce(GetAimRay().direction * 100, true, false);
            
            
            this.durationBeforeInterruptable = baseDurationBeforeInterruptable * duration;
        }

        
        public override void BeginMeleeAttackEffect()
        {
            if (!spawnEffect)
            {
                spawnEffect = true;
                if (base.isAuthority)
                {

                    EffectManager.SimpleMuzzleFlash(obj, base.gameObject, "SwingTrail", true);
                }
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            if (this.isComboFinisher)
            {
                //overlapAttack.damageType = DamageTypeCore.antiGrav | DamageType.Generic;
                R2API.DamageAPI.AddModdedDamageType(overlapAttack, DamageTypeCore.antiGrav);
            }
        }

        public override void PlayAnimation()
        {
            /*EffectManager.SpawnEffect(EffectCore.blackHoleIncisionEffect, new EffectData()
            {
                origin = base.transform.position,
                scale = 10,
                rotation = Quaternion.identity, 
            }, false);*/
            this.animationStateName = "";
            switch (this.step)
            {
                case 0:
                    this.animationStateName = "Swing1";
                    break;
                case 1:
                    this.animationStateName = "Swing2";
                    break;
                case 2:
                    this.animationStateName = "Swing3";
                    break;
            }
            bool moving = this.animator.GetBool("isMoving");
            bool grounded = this.animator.GetBool("isGrounded");

            if (!moving && grounded)
            {
                base.PlayCrossfade("FullBody, Override", this.animationStateName, "BroomSwing.playbackRate", this.duration, 0.05f);
            }

            base.PlayCrossfade("Gesture, Override", this.animationStateName, "BroomSwing.playbackRate", this.duration, 0.05f);
        }
        
        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            base.characterBody.AddSpreadBloom(this.bloom);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            //why is it interrupting itself lol
            //if (base.fixedAge >= this.durationBeforeInterruptable)
            //{
            //    return InterruptPriority.Any;
            //}
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)this.step);
        }
        
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.step = (int)reader.ReadByte();
        }

        //skip combo shit until animations are fixed
        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            this.step = 0;
        }
    }
}

