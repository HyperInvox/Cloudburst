using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Items.Green
{

    public class MagiciansEarrings : ItemBuilder
    {
        public static MagiciansEarrings instance;

        public static bool Enabled;

        public override string ItemName => "Magicians Earrings";

        public override string ItemLangTokenName => "JAZZANDAWOL";

        public override string ItemPickupDesc => "Every 15 seconds, gain a buff that can grant you armor, or regeneration, or attack speed. Also extend positive buff duration";

        public override string ItemFullDescription => @"""Every 15 seconds, gain a buff which can grant you one of the following:" + Environment.NewLine + Environment.NewLine
+ "<style=cIsUtility>+8 <style=cStack>(+8 per stack)</style> armor" + Environment.NewLine + Environment.NewLine
 + "<style=cIsHealing>20% <style=cStack>(+20% per stack)</style> regeneration</style>" + Environment.NewLine + Environment.NewLine
+ "<style=cIsDamage>10% <style=cStack>(+10% per stack)</style> attack speed</style>" + Environment.NewLine + Environment.NewLine
 + "Also extend <style=cIsUtility>positive buff duration</style> by 2 <style=cStack>(+1 per stack)</style> seconds." + Environment.NewLine + Environment.NewLine;


        public override string ItemLore => @"""The jewelry pictured on the right belonged to Aleksi Kivimäki, more often known during the conflict as 'Kalasmus the Profane.' Descriptions of him from the war describe him having an overbearingly odious presence to him, as well as being prone to fits of hyperactivity and rage. Most notably is the accounts of him possessing 'supernatural abilities' such as explosive conjuration, inducing spontaneous combustion in his enemies, and in some rare cases, being able to enter a pneumatic state for a brief period of time. Many sources from rebel groups competing against him claim he was 'Unusually youthful' and had gotten his rumored powers from 'Drinking of the blood of Lambs' and 'Pacts with devils exiled from their home realm.'

These claims have never been proven, nor disproved, as Aleksi was said to be killed during the latter half of the war by Frederick Ammon of the Stygian Mirror rebellion. Rumors claim that, during his final moments, Aleksi detonated with such force that it left a third of Frederick's battalion dead, alongside fracturing Frederick's armor, creating the infamous 'Lightning scar' spanning his Horned War-mask.
Although much of Aleksi's belongings have been collected from his uncovered residence, no body has ever been found. Old tales say that he still walked the earth even after being struck down. The world may never know what truly became of him.""
- 2019, The year that shook the world.""";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/Earrings/IMDLEarrings.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/Earrings/magicicon.png";

        public static List<BuffDef> blackList = new List<BuffDef> {
            RoR2Content.Buffs.MedkitHeal,
            RoR2.RoR2Content.Buffs.HiddenInvincibility,
            RoR2Content.Buffs.ElementalRingsCooldown,
            RoR2Content.Buffs.EngiShield
        };

        public override void CreateConfig(ConfigFile config)
        {

        }

        /*public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }*/

        protected override void Initialization()
        {
            instance = this;
            Enabled = true;
        }

        public override void Hooks()
        {
            GlobalHooks.onAddTimedBuff += GlobalHooks_onAddTimedBuff;
            GlobalHooks.onInventoryChanged += GlobalHooks_onInventoryChanged;
        }

        private void GlobalHooks_onInventoryChanged(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                body.AddItemBehavior<MagiciansEarringsBehavior>(GetCount(body));
            }
        }

        private void GlobalHooks_onAddTimedBuff(CharacterBody body, ref BuffDef buffType, ref float duration)
        {
            var inv = body.inventory;
            if (inv)
            {
                var earringsCount = GetCount(inv);

                bool blackListed = blackList.Contains(buffType) || buffType.isDebuff;
                if (earringsCount > 0 && blackListed == false) 
                {
                    //do thing???
                    duration += 2 + (1 * earringsCount);
                }
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
    }
    public class MagiciansEarringsBehavior : CharacterBody.ItemBehavior
    {
        private float timer = 0;
        private float oldCount;
        private Xoroshiro128Plus rng;
        private EffectData effectData = new EffectData
        {
            //origin = body.transform.position,
            rotation = Quaternion.identity,
            scale = 20,
        };

        public void Start()
        {
            rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);
        }
        public void FixedUpdate()
        {
            if (body)
            {
                timer += Time.deltaTime;
                if (timer >= 15)
                {
                    CloudUtils.SafeRemoveBuff(BuffCore.instance.magicArmor, body);
                    CloudUtils.SafeRemoveBuff(BuffCore.instance.magicAttackSpeed, body);
                    CloudUtils.SafeRemoveBuff(BuffCore.instance.magicRegen, body);

                    int count = rng.RangeInt(1, 3);

                    //roll me a new number you Worthless Fucking Number Generator 128+
                    if (count == oldCount)
                    {
                        count = rng.RangeInt(1, 3);
                    }
                    effectData.origin = body.transform.position;
                    switch (count)
                    {
                        case 1:
                            EffectManager.SpawnEffect(EffectCore.magicArmor, effectData, true);
                            // Util.PlaySound("Play_item_use_gainArmor", base.gameObject);
                            EffectManager.SpawnEffect(EffectCore.reallyCoolEffect, new EffectData()
                            {
                                origin = base.transform.position,
                                scale = 10,
                                rotation = Quaternion.identity,
                            }, false);
                            body.AddBuff(BuffCore.instance.magicArmor);
                            break;
                        case 2:
                            EffectManager.SpawnEffect(EffectCore.magicAttackSpeed, effectData, true);
                            Util.PlaySound("Play_item_proc_crit_attack_speed1", base.gameObject);
                            EffectManager.SpawnEffect(EffectCore.trulyCoolEffect, new EffectData()
                            {
                                origin = base.transform.position,
                                scale = 10,
                                rotation = Quaternion.identity,
                            }, false);
                            body.AddBuff(BuffCore.instance.magicAttackSpeed);
                            break;
                        case 3:
                            EffectManager.SpawnEffect(EffectCore.coolEffect, new EffectData()
                            {
                                origin = base.transform.position,
                                scale = 10,
                                rotation = Quaternion.identity,
                            }, false);
                            EffectManager.SpawnEffect(EffectCore.magicRegen, effectData, true);
                            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/FruitHealEffect"), effectData, true);
                            Util.PlaySound("char_healing_drone_heal_02", base.gameObject);
                            body.AddBuff(BuffCore.instance.magicRegen);
                            break;

                    }
                    oldCount = count;
                    timer = 0;
                }
            }
        }
    }
}