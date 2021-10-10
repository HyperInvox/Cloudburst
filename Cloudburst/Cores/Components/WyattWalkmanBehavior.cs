﻿using System;
using RoR2;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components.Wyatt
{
    public class WyattWalkmanBehavior : NetworkBehaviour, IOnDamageDealtServerReceiver
    {
        private CharacterBody characterBody;
        private ParticleSystem grooveEffect;
        private ParticleSystem grooveEffect2;
        private ChildLocator childLocator;
        private bool loseStacks { get { return stopwatch >= 3 && !flowing; } }

        private float stopwatch = 0;

        private float drainTimer = 0;

        [SyncVar]
        public bool flowing = false;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            childLocator = base.gameObject.GetComponentInChildren<ChildLocator>();

            grooveEffect = childLocator.FindChild("MusicEffect1").GetComponent<ParticleSystem>();
            grooveEffect2 = childLocator.FindChild("MusicEffect2").GetComponent<ParticleSystem>();

            //.GetComponent<ParticleSystem>();
        }

        private void Start() {
            GlobalHooks.onFinalBuffStackLost += GlobalHooks_onFinalBuffStackLost;
        }

        private void GlobalHooks_onFinalBuffStackLost(CharacterBody body, BuffDef def)
        {
            if (flowing && NetworkServer.active && characterBody == body) {
                //flowing has stopped
                CloudUtils.SafeRemoveAllOfBuff(BuffCore.instance.wyattCombatIndex, characterBody);
                flowing = false;
            }
        }

        public void FixedUpdate() {
            if (NetworkServer.active) {
                //fixedupdate but only on server
                ServerFixedUpdate();
                    
            }
        }

        private void ServerFixedUpdate()
        {
            if (flowing == false)
            {
                stopwatch += Time.fixedDeltaTime;
                if (loseStacks)
                {
                    drainTimer += Time.fixedDeltaTime;
                    if (drainTimer >= 0.5f)
                    {
                        CloudUtils.SafeRemoveBuffs(BuffCore.instance.wyattCombatIndex, characterBody, 2);
                        drainTimer = 0;
                    }
                }
            }
            else {
                //welll rested enigma, it's me
                //your 12:38am counterpart.
                //i'd very much LIKE IT
                //IF YOU COULD IMPLEMENT THE REST
                //OF THIS FUCKING CODE
                //WHEN YOU'RE WELL RESTED
            }
        }

        [Server]
        private void TriggerBehaviorInternal(float stacks)
        {
            var cap = 9 + stacks;
            if (characterBody && characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex) < cap)
            {

                grooveEffect2.Play();
                grooveEffect.Play();
                /*EffectManager.SpawnEffect(EffectCore.wyattGrooveEffect, new EffectData()
                {
                    scale = 1,
                    origin = grooveEffect.transform.position
                }, true);*/
                    characterBody.AddBuff(BuffCore.instance.wyattCombatIndex);
                //characterBody.AddTimedBuff(BuffCore.instance.wyattCombatIndex, 3);
            }
            stopwatch = 0;
        }



        public void ActivateFlowAuthority()
        {
            if (NetworkServer.active)
            {
                ActivateFlowInternal();
                return;
            }
            CmdActivateFlow();
        }

        [Command]
        private void CmdActivateFlow()
        {
            ActivateFlowInternal();
        }

        [Server]
        private void ActivateFlowInternal()
        {
            int grooveCount = characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex);
            float duration = 4;

            for (int i = 0; i < grooveCount; i++)
            {
                //add flow until we can't
                if (duration != 8)
                {
                    duration += 0.4f;
                }
            }

            characterBody.AddTimedBuff(BuffCore.instance.wyattFlow, duration);
            flowing = true;
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (damageReport.damageInfo?.inflictor == base.gameObject && flowing == false)
            {
                TriggerBehaviorInternal(1);
            }
        }
    }
}