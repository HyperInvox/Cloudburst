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

        public override string ItemPickupDesc => "Every 15 seconds, gain a buff that can grant you armor, or regeneration, or attack speed.";// Also extend the duration of positive buffs.";

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

        public override string ItemModelPath => "Assets/Cloudburst/Items/RestlessRings/IMDLRestlessRings.prefab";


        public override string ItemIconPath => "Assets/Cloudburst/Items/RestlessRings/icon.png";

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

        private void CreateFollowerPrefab() {
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
            var ItemBodyModelPrefab = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("RestlessFollowerDummy.prefab");
            var ItemFollowerPrefab = Load();
            var ItemFollower = ItemBodyModelPrefab.AddComponent<ItemFollowerSmooth>();
            ItemFollower.itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemFollower.itemDisplay.rendererInfos = CloudUtils.GatherRenderInfos(ItemBodyModelPrefab);
            ItemFollower.followerPrefab = ItemFollowerPrefab;
            ItemFollower.targetObject = ItemBodyModelPrefab;
            ItemFollower.distanceDampTime = 0.25f;
            ItemFollower.distanceMaxSpeed = 100;
            ItemFollower.SmoothingNumber = 0.25f;

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();

            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
  childName = "Base",
localPos = new Vector3(-1.66776F, -0.21747F, -0.52862F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
     });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-1.32395F, 0.51393F, -0.71607F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            //ruleLookup.Add("mdlHuntress", 0.1f);
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(8.79843F, 2.4368F, 5.00038F),
localAngles = new Vector3(270F, 180F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-2.26939F, -1.21903F, -0.00543F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-0.94248F, -0.05662F, -0.01945F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-0.91384F, -0.37104F, 0.1309F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
  childName = "Base",
localPos = new Vector3(-2.34945F, 0.69277F, -1.27591F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Base",
localPos = new Vector3(-1.26621F, -0.11008F, 0.05336F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
  childName = "Base",
localPos = new Vector3(-7.26764F, -5.35658F, 4.52467F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "Base",
localPos = new Vector3(-1.0712F, -0.0798F, -0.59302F),
localAngles = new Vector3(270F, 0F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-0.45782F, -0.51298F, -0.39241F),
localAngles = new Vector3(347.6326F, 269.9879F, 89.96059F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            return rules;
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
