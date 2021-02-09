using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores
{
    class GlobalHooks
    {
        public struct OnHitEnemy
        {
            public Inventory attackerInventory;
            public CharacterMaster attackerMaster;
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public OnHitEnemy(GameObject victim, GameObject attacker) {
                victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                attackerBody = attacker ? attacker.GetComponent<CharacterBody>() : null;
                attackerMaster = attackerBody ? attackerBody.master : null;
                attackerInventory = attackerMaster ? attackerMaster.inventory : null;
            }
        }

        public delegate void DamageInfoCloudGate(ref DamageInfo info, GameObject victim, OnHitEnemy onHitInfo);
        public delegate void CharacterBodyCloudGate(CharacterBody body);
        public delegate void CharacterBodyAddTimedBuffCloudGate(CharacterBody body, ref BuffIndex type, ref float duration);
        public delegate void CritCloudGate( CharacterBody attackerBody, CharacterMaster attackerMaster, float procCoeff, ProcChainMask procMask);


        public static event DamageInfoCloudGate onHitEnemy;
        public static event CritCloudGate onCrit;
        public static event DamageInfoCloudGate takeDamage;
        public static event CharacterBodyCloudGate onInventoryChanged;
        public static event CharacterBodyAddTimedBuffCloudGate onAddTimedBuff;
        public static event CharacterBodyCloudGate recalculateStats;

        public static void Init()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCrit += GlobalEventManager_OnCrit;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
        }


        private static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            onAddTimedBuff.Invoke(self, ref buffType, ref duration);
            orig(self, buffType, duration);
        }

        private static void GlobalEventManager_OnCrit(On.RoR2.GlobalEventManager.orig_OnCrit orig, GlobalEventManager self, CharacterBody body, CharacterMaster master, float procCoefficient, ProcChainMask procChainMask)
        {
            if (body && procCoefficient > 0f && body && master && master.inventory)
            {
                onCrit.Invoke(body, master, procCoefficient, procChainMask);
            }
            orig(self, body, master, procCoefficient, procChainMask );
        }

        private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            onInventoryChanged.Invoke(self);
            orig(self);
        }

        private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            onHitEnemy.Invoke(ref damageInfo, victim, new OnHitEnemy(victim, damageInfo.attacker));
            orig(self, damageInfo, victim);
        }

        public static void CharacterSpawnCard_Awake(On.RoR2.CharacterSpawnCard.orig_Awake orig, CharacterSpawnCard self) {
            self.loadout = new SerializableLoadout();
            orig(self);
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            takeDamage.Invoke(ref damageInfo, self.gameObject, new OnHitEnemy(self.gameObject, damageInfo.attacker));
            orig(self, damageInfo);
        }
    }
}
