using System;
using RoR2;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components.Wyatt
{
    public class WyattWalkmanBehavior : NetworkBehaviour, IOnDamageDealtServerReceiver
    {
        private CharacterBody characterBody;

        private bool loseStacks { get { return stopwatch >= 3; } }

        [SyncVar]
        private float stopwatch = 0;

        [SyncVar]
        private float drainTimer = 0;
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
            stopwatch += Time.fixedDeltaTime;
            if (loseStacks) {
                drainTimer += Time.fixedDeltaTime;
                if (drainTimer >= 0.5f) {
                    CloudUtils.SafeRemoveBuffs(BuffCore.instance.wyattCombatIndex, characterBody, 2);
                    drainTimer = 0;
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
            stopwatch = 0;
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (damageReport.damageInfo?.inflictor == base.gameObject)
            {
                TriggerBehaviorInternal(1);
            }
        }
    }
}