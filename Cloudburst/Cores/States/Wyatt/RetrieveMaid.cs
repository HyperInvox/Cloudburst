using EntityStates;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.HAND;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    class RetrieveMaid : BaseSkillState
    {
        public static float baseDuration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority) {
                gameObject.GetComponent<MAIDManager>().RetrieveMAIDAuthority();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= baseDuration && isAuthority)
            {
                skillLocator.special.SetBaseSkill(WyattCore.instance.throwPrimary);
                //skillLocator.special.SetPropertyValue("cooldownRemaining", (Single)3);
                //skillLocator.special.cooldownRemaining
                outer.SetNextStateToMain(); 
            };
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}