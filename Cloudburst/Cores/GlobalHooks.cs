using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores
{
    class GlobalHooks
    {
        public static void CharacterSpawnCard_Awake(On.RoR2.CharacterSpawnCard.orig_Awake orig, CharacterSpawnCard self) {
            self.loadout = new SerializableLoadout();
            orig(self);
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool antiGravType = (damageInfo.damageType & DamageTypeCore.antiGrav) != DamageType.Generic;
            bool spikedType = (damageInfo.damageType & DamageTypeCore.spiked) != DamageType.Generic;
            bool falldmgType = (damageInfo.damageType & DamageType.FallDamage) != DamageType.Generic;
            bool pullType = (damageInfo.damageType & DamageTypeCore.pullEnemies) != DamageType.Generic;
            //bool isOsp2 = (damageInfo.damageType & DamageType.AOE) != DamageType.Generic;

            var rigid = self.body.rigidbody;
            if (self.body)
            {
                var motor = self.body.characterMotor;
                if (antiGravType && self.body)
                {
                    //implement alternate behavior if it's a flier
                    if (self.body.characterMotor)
                    {
                        self.body.AddTimedBuff(BuffCore.instance.antiGravIndex, 5);

                    }

                }
                if (damageInfo.attacker && pullType)
                {
                    //var distance = Vector3.Distance(damageInfo.attacker.transform.position, self.transform.position);
                    Vector3 position2 = self.transform.position;
                    Vector3 normalized = (damageInfo.attacker.transform.position - position2).normalized;

                    if (motor)
                    {
                        motor.ApplyForce(normalized * 700, true);
                    }
                    else if (rigid)
                    {
                        rigid.AddForce(normalized * 700, ForceMode.Impulse);
                    }
                }
                if (spikedType)
                {
                    //self.body.RemoveBuff(BuffCore.instance.antiGravIndex);
                    var force = new Vector3(0, -15000, 0);
                    //var force = new Vector3(0, -2, 0);
                    if (rigid)
                    {
                        //force *= rigid.mass;
                        //force /= 2;
                        rigid.AddForce(force);
                    }

                    if (motor)
                    {
                        //motor.useGravity = true;
                        //force *= motor.mass;
                        //force /= 2;
                        //force * motor.mass /= 2;  
                        motor.ApplyForce(force, true, true);
                    }

                    //bing bing WA-FUCKING-HOO 
                }
                if (motor)
                {

                }
            }
            if (self.body.HasBuff(BuffCore.instance.antiGravIndex) && falldmgType)
            {
                damageInfo.damage *= 2;
                //real good feeling.
            }

            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            if (attackerBody && attackerBody.inventory)
            {
                var victimBody = self.body;

                Inventory inv = attackerBody.inventory;
                Inventory inv2 = null;

                if (self.body && self.body.inventory)
                {
                    inv2 = self.body.inventory;
                }
                var count = (int)0;

                if (inv2)
                {
                    count = inv2.GetItemCount(ItemCore.instance.barrierOnLevelIndex);
                }

                int experienceOnHitCount = inv.GetItemCount(ItemCore.instance.experienceOnHitIndex);

                if (count > 0 && victimBody)
                {
                    victimBody.AddTimedBuff(BuffCore.instance.skinIndex, 5);
                }

                if (experienceOnHitCount > 0 && attackerBody.teamComponent)
                {
                    float exp = 0.1f + (experienceOnHitCount * 0.4f);
                    TeamManager.instance.GiveTeamExperience(attackerBody.teamComponent.teamIndex, (uint)exp);
                }
            }

            orig(self, damageInfo);
        }


        public static  void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            bool shouldOrigSelf = true;
            if (self)
            {
                if (buffType == EliteCore.instance.warFriendlyBuffIndex && self.HasBuff(EliteCore.instance.warIndex))
                {
                    shouldOrigSelf = false;
                }
                if (self.inventory)
                {
                    var inv = self.inventory;
                    var earringsCount = inv.GetItemCount(ItemCore.instance.extendEnemyBuffDurationIndex);
                    var lemCount = inv.GetItemCount(ItemCore.instance.lemdogIndex);
                    var def = BuffCatalog.GetBuffDef(buffType);

                    if (lemCount > 0)
                    {
                        if (def.isDebuff && Util.CheckRoll(25 + (lemCount * 2.5f), self.master))
                        {
                            var random = UnityEngine.Random.Range(0, ItemCore.instance.lemdogList.Count);
                            var buff = ItemCore.instance.lemdogList[random];
                            buffType = buff;
                        }
                    }
                    if (earringsCount > 0 && buffType != BuffIndex.MedkitHeal & buffType != BuffIndex.ElementalRingsCooldown && !def.isDebuff)
                    {
                        //do thing???
                        duration += 1 + (1 * earringsCount);
                    }
                }
            }
            if (shouldOrigSelf)
            {
                orig(self, buffType, duration);
            }
        }
    }
}
