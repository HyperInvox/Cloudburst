using System;
using RoR2;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components.Wyatt
{
    public class WyattWalkmanBehavior : NetworkBehaviour, IOnDamageInflictedServerReceiver {
        private CharacterBody characterBody;

        private void Awake()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
        }

        private void Start()
        {
            //GiveHeadphonesAuthority();
        }

        public void GiveHeadphonesAuthority()
        {
            if (NetworkServer.active)
            {
                GiveHeadphonesInternal();
                return;
            }
            CmdGiveHeadphonesInternal();
        }

        [Command]
        private void CmdGiveHeadphonesInternal()
        {
            GiveHeadphonesInternal();
        }

        [Server]
        private void GiveHeadphonesInternal()
        {
            if (characterBody && characterBody.inventory && characterBody.inventory.GetItemCount(ItemCore.instance.wyattWalkmanIndex) == 0)
            {
                characterBody.inventory.GiveItem(ItemCore.instance.wyattWalkmanIndex, 1);
            }
        }

        public void TriggerBehaviorAuthority(float stacks)
        {
            if (NetworkServer.active)
            {
                TriggerBehaviorInternal(stacks);
                return;
            }
            CmdTriggerBehaviorInternal(stacks);
        }

        [Command]
        private void CmdTriggerBehaviorInternal(float stacks)
        {
            TriggerBehaviorInternal(stacks);
        }

        [Server]
        private void TriggerBehaviorInternal(float stacks)
        {
            var cap = 9 + stacks;
            if (characterBody && characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex) < cap && !characterBody.outOfCombat)
            {
                characterBody.AddBuff(BuffCore.instance.wyattCombatIndex);
            }
            else if (characterBody.outOfCombat) {
                characterBody.RemoveBuff(BuffCore.instance.wyattCombatIndex);
            }
        }

        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            TriggerBehaviorAuthority(1);
        }
    }
}