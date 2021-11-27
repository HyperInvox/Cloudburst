using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Cloudburst.Cores.BuffCore;

namespace Cloudburst.Cores.Items.Green
{

    public class RestlessRings : ItemBuilder
    {
        public static RestlessRings instance;

        public override string ItemName => "Restless Rings";

        public override string ItemLangTokenName => "RESTLESSRINGS";

        public override string ItemPickupDesc => "Every 15 seconds, gain a buff that can grant you armor, or regeneration, or attack speed. Also extend the duration of positive buffs.";

        public override string ItemFullDescription => @"Every 15 seconds, gain a buff which can grant you one of the following:" + Environment.NewLine
+ "<style=cIsUtility>+8 <style=cStack>(+8 per stack)</style> armor" + Environment.NewLine
 + "<style=cIsHealing>20% <style=cStack>(+20% per stack)</style> regeneration</style>" + Environment.NewLine
+ "<style=cIsDamage>10% <style=cStack>(+10% per stack)</style> attack speed</style>";
        //Also extend <style=cIsUtility>positive buff duration</style> by 2 <style=cStack>(+1 per stack)</style> seconds.

        public override string ItemLore => @"A pantheon of siblings. 
A celestial mystery. 
A throne awaiting a heir. 

The one with the most brilliant mind among them had found the thread that connects them. Born of grand divinity and the arcane, it was of his nature to peel back the skin of existence and seek the shimmering, guiding light underneath. 
So close he came to touching the source of said light, but in his hubris he had wronged so many. The scythe of death had fell upon him before, and had condemned him until the earth had revolved about the sun one hundred times. In his freedom, derrangement and fervor proliferated within him, and soon enough he would make a fatal mistake. 
He had tried to sacrifice the blood of his nemesis' next of kin. With this folly, he would die thrice more. Each at the hand of malice itself.














far...














Far did he fall.
";

        public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "Assets/Cloudburst/Items/Earrings/IMDLEarringss.prefab";
        

        public override string ItemIconPath => "Assets/Cloudburst/Items/Earrings/icon.png";

        public BuffDef magicArmor;
        public BuffDef magicRegen;
        public BuffDef magicAttackSpeed;

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
                        
            BuildBuffs();

        }

        private void BuildBuffs()
        {
            this.magicRegen = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#3CB043"),
            }.BuildBuff();

            this.magicAttackSpeed = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#FFA500"),
            }.BuildBuff();

            this.magicArmor = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#4D516D"),
            }.BuildBuff();
        }
        public override void Hooks()
        {
            GlobalHooks.onAddTimedBuff += GlobalHooks_onAddTimedBuff;
            GlobalHooks.onInventoryChanged += GlobalHooks_onInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int magicCount = GetCount(body);
            if (body.HasBuff(magicArmor))
            {
                args.armorAdd += (8f * magicCount);
            }
            if (body.HasBuff(magicAttackSpeed))
            {
                args.attackSpeedMultAdd += (0.1f * magicCount);
            }
            if (body.HasBuff(magicRegen))
            {
                args.baseRegenAdd += (0.2f * magicCount);
            }
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
            AddBuff(rng.RangeInt(1, 3));
        }
        void AddBuff(int count)
        {
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
                    body.AddBuff(RestlessRings.instance.magicArmor);
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
                    body.AddBuff(RestlessRings.instance.magicAttackSpeed);
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
                    body.AddBuff(RestlessRings.instance.magicRegen);
                    break;

            }
        }
        public void FixedUpdate()
        {
            if (body)
            {
                timer += Time.deltaTime;
                if (timer >= 15)
                {
                    CloudUtils.SafeRemoveBuff(RestlessRings.instance.magicArmor, body);
                    CloudUtils.SafeRemoveBuff(RestlessRings.instance.magicAttackSpeed, body);
                    CloudUtils.SafeRemoveBuff(RestlessRings.instance.magicRegen, body);

                    int count = rng.RangeInt(1, 3);
                    AddBuff(count);
                    //roll me a new number you Worthless Fucking Number Generator 128+
                    if (count == oldCount)
                    {
                        count = rng.RangeInt(1, 3);
                    }
                    oldCount = count;
                    timer = 0;
                }
            }
        }
    }
}
