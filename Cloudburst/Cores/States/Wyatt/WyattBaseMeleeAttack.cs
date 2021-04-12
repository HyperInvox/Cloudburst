using System;
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
        public static float baseDurationBeforeInterruptable = 0.55f;
        public float bloom = 1f;
        /*public static float comboFinisherBaseDuration = 0.5f;
        public static float comboFinisherhitPauseDuration = 0.15f;
        public static float comboFinisherBloom = 0.5f;
        public static float comboFinisherBaseDurationBeforeInterruptable = 0.5f;
        private string animationStateName;*/
        private float durationBeforeInterruptable;

        private bool isComboFinisher
        {
            get
            {
                return this.step == 2;
            }
        }

        private bool spawnEffect = false;

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
            this.mecanimHitboxActiveParameter = "BroomSwing.Hitbox";
            this.baseDuration = 0.69f;
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
            this.durationBeforeInterruptable = baseDurationBeforeInterruptable / this.attackSpeedStat;
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
                overlapAttack.damageType = DamageTypeCore.antiGrav | DamageType.Generic;
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
            /*this.animationStateName = "";
            switch (this.step)
            {
                case 0:
                    this.animationStateName = "Primary1";
                    break;
                case 1:
                    this.animationStateName = "Primary2";
                    break;
                case 2:
                    this.animationStateName = "Primary3";
                    break;
            }
            bool @bool = this.animator.GetBool("isMoving");
            bool bool2 = this.animator.GetBool("isGrounded");

            if (!@bool && bool2)
            {
                base.PlayCrossfade("FullBody, Override", this.animationStateName, "Primary.rate", this.duration, 0.05f);
            }
            else
            {
                base.PlayCrossfade("UpperBody, Override", this.animationStateName, "Primary.rate", this.duration, 0.05f);
            }*/

            base.PlayCrossfade("Gesture, Override", "BroomSwing", "BroomSwing.playbackRate", this.duration, 0.05f);
        }

        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            base.characterBody.AddSpreadBloom(this.bloom);
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.durationBeforeInterruptable)
            {
                return InterruptPriority.Skill;
            }
            return InterruptPriority.PrioritySkill;
        }

        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            this.step = i;
        }
    }
}

