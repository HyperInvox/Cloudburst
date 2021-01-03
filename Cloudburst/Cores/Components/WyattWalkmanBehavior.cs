using System;
using RoR2;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components.Wyatt
{
    public class WyattWalkmanBehavior : NetworkBehaviour, IOnDamageDealtServerReceiver {
        private CharacterBody characterBody;

        private float stopwatch = 0;
        private void Awake()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
        }


        public void FixedUpdate() {
            if (NetworkServer.active) {
                //fixedupdate but only on server
                ServerFixedUpdate();
                    
            }
        }

        private void ServerFixedUpdate()
        {
            stopwatch += Time.deltaTime;
            if (stopwatch >= 3)
            {
                stopwatch = 0;
                if (characterBody)
                {
                    if (characterBody.HasBuff(BuffCore.instance.wyattCombatIndex))
                    {
                        characterBody.RemoveBuff(BuffCore.instance.wyattCombatIndex);
                    }
                }
            }
        }

        [Server]
        private void TriggerBehaviorInternal(float stacks)
        {
            var cap = 9 + stacks;
            if (characterBody && characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex) < cap)
            {
                characterBody.AddBuff(BuffCore.instance.wyattCombatIndex);
                //characterBody.AddTimedBuff(BuffCore.instance.wyattCombatIndex, 3);
            }
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            TriggerBehaviorInternal(1);
        }
    }
}