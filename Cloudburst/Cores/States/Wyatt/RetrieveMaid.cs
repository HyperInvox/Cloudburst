using EntityStates;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.HAND;

using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    class RetrieveMaid : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                gameObject.GetComponent<MAIDManager>().RetrieveMAIDAuthority();
                //skillLocator.special.SetPropertyValue("cooldownRemaining", (Single)3);
                //skillLocator.special.cooldownRemaining
                outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}