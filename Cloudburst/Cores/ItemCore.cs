using Cloudburst.Cores.Components.Wyatt;
using EntityStates.Scrapper;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores
{
    class ItemCore
    {
        public static ItemCore instance;

        //TODO:
        //Give Items Models
        //Give items icons
        //These values are set by RegisterItem(ItemIndex, ItemDef).
        protected internal ItemIndex itemChampionOnKillIndex; //model and icon (I LOVE YOU ROB)
        protected internal ItemIndex largerTeleporterRadiusIndex; //model and icon
        protected internal ItemIndex crippleOnHitIndex; //model and icon
        protected internal ItemIndex cloakOnInteractionIndex; //model and icon
        protected internal ItemIndex itemOnLevelUpIndex; //rob if he wants to do texturing
        protected internal ItemIndex randomDebuffOnHitIndex; //model and icon
        protected internal ItemIndex moneyOnInteractionIndex; //rob
        protected internal ItemIndex barrierOnCritIndex; //rob
        protected internal ItemIndex barrierOnLevelIndex; //jello
        protected internal ItemIndex experienceOnHitIndex; //rob
        protected internal ItemIndex lemdogIndex; //model and icon
        protected internal ItemIndex extendEnemyBuffDurationIndex; //model and icon
        //protected internal ItemIndex damageOnDamagedIndex;
        //protected internal ItemIndex skinIndex;
        //protected internal ItemIndex freezeEnemiesOnHitIndex;
        //protected internal ItemIndex geigerCounterIndex;
        //protected internal ItemIndex heartIndex;
        //protected internal ItemIndex chipIndex;
        //protected internal ItemIndex wyattWalkmanIndex;

        public GameObject extractorMDL;
        public GameObject mechanicalTrinketMDL;
        public GameObject fabDagMDL;
        public GameObject japeCloakMDL;
        public GameObject carePackageMDL;
        public GameObject redactedMDL;
        public GameObject keyCardMDL;
        public GameObject topazLensMDL;
        public GameObject bbaMDL;
        public GameObject harvesterMDL;
        public GameObject lemdogMDL;
        public GameObject magicsMDL;





        public List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            ItemIndex.Incubator,
            ItemIndex.BleedOnHitAndExplode,
            ItemIndex.FireballsOnHit 
            //Excluding pearls because those aren't boss Item, they come from the Cleansing Pool 
        };

        public List<BuffIndex> scpBuffList = new List<BuffIndex>{
            BuffIndex.NullifyStack,
            BuffIndex.ClayGoo,
            BuffIndex.BeetleJuice,
            BuffIndex.HealingDisabled
        };
        public List<BuffIndex> eliteBuffList = new List<BuffIndex>{
            BuffIndex.AffixPoison,
            BuffIndex.AffixRed,
            BuffIndex.AffixBlue,
            BuffIndex.AffixWhite
        };
        public List<BuffIndex> lemdogList = new List<BuffIndex>{
            BuffIndex.Warbanner,
            //BuffIndex.Cloak,
            BuffIndex.CloakSpeed,
            //BuffIndex.EngiShield,
            BuffIndex.MeatRegenBoost,
            BuffIndex.WhipBoost,
            BuffIndex.TeamWarCry,
        };
        public void DropShipCall(Transform transform, int itemCount)
        {
            List<PickupIndex> list = Util.CheckRoll((5 * itemCount)) ? Run.instance.availableTier2DropList : Run.instance.availableTier1DropList;
            int item = Run.instance.treasureRng.RangeInt(0, list.Count);

            PickupDropletController.CreatePickupDroplet(list[item], transform.position, new Vector3(0, 10, 0));


            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/impacteffects/PodGroundImpact"), new EffectData
            {
                origin = transform.position,
                scale = 15
            }, true);
        }   

        public ItemIndex GetRandomItem(List<ItemIndex> Item)
        {
            int itemID = UnityEngine.Random.Range(0, Item.Count);
            return Item[itemID];
        }

        public ItemCore() => RegisterItems();

        protected internal void RegisterItems()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            instance = this;

            foreach (var item in AssetsCore.mainAssetBundle.LoadAllAssets<GameObject>()) {
                if (item.name.Contains("IMDL")) {
                    switch (item.name) {
                        case "IMDLBrokenBodyArmor":
                            bbaMDL = item;
                            break;
                        case "IMDLCarePackageRequester":
                            carePackageMDL = item;
                            break;
                        case "IMDLCloak":
                            japeCloakMDL = item;
                            break;
                        case "IMDLEarrings":
                            magicsMDL = item;
                            break;
                        case "IMDLDagger":
                            fabDagMDL = item;
                            break;
                        case "IMDLGrinder":
                            extractorMDL = item;
                            break;
                        case "IMDLLemDog":
                            lemdogMDL = item;
                            break;
                        case "IMDLMechanicalTrinket":
                            mechanicalTrinketMDL = item;
                            break;
                        case "IMDLKeycard":
                            keyCardMDL = item;
                            break;
                    }
                    LogCore.LogI("Item model found! " + item.name);
                    var display = item.AddComponent<ItemDisplay>();
                    display.rendererInfos = CloudUtils.GatherRenderInfos(item);
                }

            } 

            RegisterTokens();
            ItemTag[] itemChampionOnKillTags = new ItemTag[2]
            {
                ItemTag.OnKillEffect,
                ItemTag.AIBlacklist
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_ITEMONCHAMPIONKILL_DESC",
                hidden = false,
                loreToken = "ITEM_ITEMONCHAMPIONKILL_LORE",
                name = "ItemChampionOnKill",
                nameToken = "ITEM_ITEMONCHAMPIONKILL_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/Grinder/icon.png", 
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/Grinder/IMDLGrinder.prefab",
                pickupToken = "ITEM_ITEMONCHAMPIONKILL_PICKUP",
                tags = itemChampionOnKillTags,
                tier = ItemTier.Tier3,
                unlockableName = ""
            });
            ItemTag[] largerTeleporterRadiusTags = new ItemTag[2]
            {
                ItemTag.AIBlacklist,
                ItemTag.Utility
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_LARGERTELEPORTERRADIUS_DESC",
                hidden = false,
                loreToken = "",
                name = "LargerTeleporterRadius",
                nameToken = "ITEM_LARGERTELEPORTERRADIUS_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/MechanicalTrinket/Icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/MechanicalTrinket/IMDLMechanicalTrinket.prefab",
                pickupToken = "ITEM_LARGERTELEPORTERRADIUS_PICKUP",
                tags = largerTeleporterRadiusTags,
                tier = ItemTier.Tier2,
                unlockableName = ""
            });
            ItemTag[] crippleOnHitTags = new ItemTag[1]
            {
                ItemTag.Damage
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_CRIPPLEONHIT_DESC",
                hidden = false,
                loreToken = "",
                name = "CrippleOnHit",
                nameToken = "ITEM_CRIPPLEONHIT_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/FabinhoruDagger/icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/FabinhoruDagger/IMDLDagger.prefab",
                pickupToken = "ITEM_CRIPPLEONHIT_PICKUP",
                tags = crippleOnHitTags,
                tier = ItemTier.Tier2,
                unlockableName = ""
            });
            ItemTag[] cloakOnInteractionTags = new ItemTag[2]
            {
                ItemTag.AIBlacklist,
                ItemTag.Utility
            };
            //string unlockAble = Main.unlockAll.Value ? "CLOUDBURST_GRABORDIE_REWARD_ID" : "";
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_CLOAKBUFFONINTERACTION_DESC",
                hidden = false,
                loreToken = "",
                name = "CloakOnInteraction",
                nameToken = "ITEM_CLOAKBUFFONINTERACTION_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/Cloak/JapeIcon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/Cloak/IMDLCloak.prefab",
                pickupToken = "ITEM_CLOAKBUFFONINTERACTION_PICKUP",
                tags = cloakOnInteractionTags,
                tier = ItemTier.Tier2,
                unlockableName = AchievementCore.GetUnlockableString("GrabOrDie")
            });
            ItemTag[] itemOnLevelUpTags = new ItemTag[2]
            {
                ItemTag.AIBlacklist,
                ItemTag.Utility
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_ITEMONLEVELUP_DESC",
                hidden = false,
                loreToken = "",
                name = "ItemOnLevelUp2",
                nameToken = "ITEM_ITEMONLEVELUP_NAME",
                //pickupIconPath = pickUpIconPath,
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/CarePackageRequester/IMDLCarePackageRequester.prefab",
                pickupToken = "ITEM_ITEMONLEVELUP_PICKUP",
                tags = itemOnLevelUpTags,
                tier = ItemTier.Tier3,
                unlockableName = ""
            });
            ItemTag[] randomDebuffOnHitTags = new ItemTag[2]
            {
                ItemTag.Utility,
                ItemTag.Damage
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_RANDOMDEBUFFONHIT_DESC",
                hidden = false,
                loreToken = "",
                name = "RandomDebuffOnHit",
                nameToken = "ITEM_RANDOMDEBUFFONHIT_NAME",
                //pickupIconPath = pickUpIconPath,
                //pickupModelPath = pickUpModelPath,
                pickupToken = "ITEM_RANDOMDEBUFFONHIT_PICKUP",
                tags = randomDebuffOnHitTags,
                tier = ItemTier.Tier3,
                unlockableName = ""
            });
            ItemTag[] moneyOnInteractionTags = new ItemTag[2]
            {
                ItemTag.Utility,
                ItemTag.AIBlacklist
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_MONEYONINTERACTION_DESC",
                hidden = false,
                loreToken = "",
                name = "MoneyOnInteraction",
                nameToken = "ITEM_MONEYONINTERACTION_NAME",
                //pickupIconPath = pickUpIconPath,
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/UESKeycard/IMDLKeycard.prefab",
                pickupToken = "ITEM_MONEYONINTERACTION_PICKUP",
                tags = moneyOnInteractionTags,
                tier = ItemTier.Tier1,
                unlockableName = ""
            });
            ItemTag[] barrierOnCritTags = new ItemTag[2]
            {
                    ItemTag.Utility,
                    ItemTag.Healing
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_BARRIERONCRIT_DESC",
                hidden = false,
                loreToken = "",
                name = "BarrierOnCrit",
                nameToken = "ITEM_BARRIERONCRIT_NAME",
                //pickupIconPath = pickUpIconPath,
                //pickupModelPath = pickUpModelPath,
                pickupToken = "ITEM_BARRIERONCRIT_PICKUP",
                tags = barrierOnCritTags,
                tier = ItemTier.Tier2,
                unlockableName = ""
            });
            ItemTag[] barrierOnLevelTags = new ItemTag[2]
            {
                    ItemTag.Utility,
                    ItemTag.Healing
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_BARRIERONLEVEL_DESC",
                hidden = false,
                loreToken = "",
                name = "BarrierOnLevel",
                nameToken = "ITEM_BARRIERONLEVEL_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/BrokenBodyArmor/brokenarmoricon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/BrokenBodyArmor/IMDLBrokenBodyArmor.prefab",
                pickupToken = "ITEM_BARRIERONLEVEL_PICKUP",
                tags = barrierOnLevelTags,
                tier = ItemTier.Tier1,
                unlockableName = ""
            });
            ItemTag[] experienceOnHitTags = new ItemTag[2]
            {
                    ItemTag.Utility,
                    ItemTag.AIBlacklist
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_EXPERIENCEONHIT_DESC",
                hidden = false,
                loreToken = "",
                name = "ExperienceOnHit",
                nameToken = "ITEM_EXPERIENCEONHIT_NAME",
                //pickupIconPath = pickUpIconPath,
                //pickupModelPath = pickUpModelPath,
                pickupToken = "ITEM_EXPERIENCEONHIT_PICKUP",
                tags = experienceOnHitTags,
                tier = ItemTier.Tier1,
                unlockableName = AchievementCore.GetUnlockableString("HitLevelCap")
            });
            ItemTag[] betterCritsOnLowHealthTags = new ItemTag[2]
            {
                    ItemTag.Damage,
                    ItemTag.AIBlacklist
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_LEMDOG_DESC",
                hidden = false,
                loreToken = "",
                name = "Lemdog",
                nameToken = "ITEM_LEMDOG_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/Lemdog/LemDog_TexIcon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/Lemdog/IMDLLemDog.prefab",
                pickupToken = "ITEM_LEMDOG_PICKUP",
                tags = betterCritsOnLowHealthTags,
                tier = ItemTier.Tier3,
                unlockableName = ""
            });
            ItemTag[] extendEnemyBuffDurationTags = new ItemTag[2]
            {
                    ItemTag.Utility,
                    ItemTag.AIBlacklist
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_EXTENDEDENEMYBUFFDURATIOM_DESC",
                hidden = false, 
                loreToken = "",
                name = "ExtendedEnemyBuffDuration",
                nameToken = "ITEM_EXTENDEDENEMYBUFFDURATIOM_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/Earrings/magicicon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/Earrings/IMDLEarrings.prefab",
                pickupToken = "ITEM_EXTENDEDENEMYBUFFDURATIOM_PICKUP",
                tags = extendEnemyBuffDurationTags,
                tier = ItemTier.Tier2,
                unlockableName = ""
            });
            /*ItemTag[] walkmanTags = new ItemTag[4]
            {
                    ItemTag.Utility,
                    ItemTag.WorldUnique,
                    ItemTag.AIBlacklist,
                    ItemTag.BrotherBlacklist,
            };
            RegisterItem(new ItemDef
            {
                canRemove = false,
                hidden = false,
                name = "WyattWalkman",
                tags = walkmanTags,
                tier = ItemTier.Tier3,
            });*/

            
            //chipIndex
            Hook();
        }

        protected internal void RegisterTokens()
        {
            LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_NAME", "Extractor"); //model
            LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_DESC", "<style=cIsUtility>25% chance <style=cStack>(+10% per stack)</style> for bosses to drop a random boss item when killed.</style>");
            LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_PICKUP", "Chance for bosses to drop items when killed");

            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_NAME", "Mechanical Trinket"); //model
            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_DESC", "<style=cIsUtility>Increase teleporter radius by 3m</style> <style=cStack>(+3m per stack)</style>.");
            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_PICKUP", "Increase teleporter radius.");

            LanguageAPI.Add("ITEM_CRIPPLEONHIT_NAME", "Fabinhoru's Dagger");
            LanguageAPI.Add("ITEM_CRIPPLEONHIT_DESC", "20% Chance to <style=cIsDamage>cripple enemies</style> for 3 <style=cStack>(+3 seconds per stack)</style> seconds.");
            LanguageAPI.Add("ITEM_CRIPPLEONHIT_PICKUP", "Chance to cripple enemies on hit.");

            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_NAME", "Jape's Cloak");
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_DESC", "Become <style=cIsUtility>cloaked for 2 seconds</style> <style=cStack>(+2 seconds per stack)</style> when picking up an item.");
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_PICKUP", "Become cloaked when picking up an item.");

            LanguageAPI.Add("ITEM_ITEMONLEVELUP_NAME", "Care Package Requester");
            LanguageAPI.Add("ITEM_ITEMONLEVELUP_DESC", "<style=cIsUtility>75% Chance to gain an item on level up</style>. <style=cStack>+5% chance to gain a green item per stack</style>");
            LanguageAPI.Add("ITEM_ITEMONLEVELUP_PICKUP", "Chance to gain an item on level up");

            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_NAME", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_DESC", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_PICKUP", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_LORE", "Order: \u201C[REDACTED]\u201D\r\nTracking Number: [REDACTED]\r\nEstimated Delivery: [REDACTED]\r\nShipping Method: [REDACTED]\r\nShipping Address: [REDACTED], [REDACTED]\r\nShipping Details:\r\n\r\nSecure, contain, protect.\r\n");

            LanguageAPI.Add("ITEM_MONEYONINTERACTION_NAME", "UES Keycard");
            LanguageAPI.Add("ITEM_MONEYONINTERACTION_DESC", "Gain <style=cIsUtility>10 gold</style> <style=cStack>(+5 gold per stack)</style> upon opening a chest. ");
            LanguageAPI.Add("ITEM_MONEYONINTERACTION_PICKUP", "Activating an interactable gives gold");

            LanguageAPI.Add("ITEM_BARRIERONCRIT_NAME", "Onyx Optics");
            LanguageAPI.Add("ITEM_BARRIERONCRIT_DESC", "Gain a <style=cIsHealing>temporary barrier</style> on critical hits for <style=cIsHealing>5 health</style> <style=cStack>(+3 per stack)</style>.");
            LanguageAPI.Add("ITEM_BARRIERONCRIT_PICKUP", "Gain barrier on critical hits");

            LanguageAPI.Add("ITEM_BARRIERONLEVEL_NAME", "Broken Body Armor");
            LanguageAPI.Add("ITEM_BARRIERONLEVEL_DESC", "Gain 10 <style=cStack>(+5 per stack)</style> <style=cIsUtility>armor</style> when hurt.");
            LanguageAPI.Add("ITEM_BARRIERONLEVEL_PICKUP", "Gain armor when hurt");

            LanguageAPI.Add("ITEM_EXPERIENCEONHIT_NAME", "Glass Harvester");
            LanguageAPI.Add("ITEM_EXPERIENCEONHIT_DESC", "Gain 0.5 <style=cStack>(+0.4 per stack)</style> <style=cIsUtility>experience</style> on hit.");
            LanguageAPI.Add("ITEM_EXPERIENCEONHIT_PICKUP", "Gain experience on hit");

            LanguageAPI.Add("ITEM_LEMDOG_NAME", "Lemdog");
            LanguageAPI.Add("ITEM_LEMDOG_DESC", "25% <style=cStack>(+2.5% per stack)</style> chance for <style=cIsUtility>applied debuffs to become beneficial buffs</style>");
            LanguageAPI.Add("ITEM_LEMDOG_PICKUP", "Chance for debuffs to become beneficial buffs when applied");

            LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_NAME", "Magician's Earrings");
            LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_DESC", "Extend <style=cIsUtility>positive buff duration</style> by 2<style=cStack>(+1 per stack)</style> seconds.");
            LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_PICKUP", "Extend positive buff duration");

            LanguageAPI.Add("ITEM_WYATTWALKMAN_NAME", "Walkman");
            LanguageAPI.Add("ITEM_WYATTWALKMAN_DESC", "Gain <style=cIsUtility>+x% speed</style> and <style=cIsHealing>+x% regen</style> while in combat every 3 seconds. Maximum 10 stacks <style=cStack>(+1 per stack)</style>");
            LanguageAPI.Add("ITEM_WYATTWALKMAN_PICKUP", "Gain speed and regeneration while in combat");
        }

        protected internal void RegisterItem(ItemDef itemDef)
        {
            var internalName = itemDef.name;
            var upperName = internalName.ToUpper(CultureInfo.InvariantCulture);
            if (itemDef.nameToken == null)
            {
                itemDef.nameToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_NAME", upperName);
            }
            if (itemDef.descriptionToken == null)
            {
                itemDef.descriptionToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_DESC", upperName);
            }
            if (itemDef.pickupToken == null)
            {
                itemDef.pickupToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_PICKUP", upperName);
            }
            if (itemDef.loreToken == null)
            {
                itemDef.loreToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_LORE", upperName);
            }
            var customItem = new CustomItem(itemDef, new ItemDisplayRule[0]);



            switch (itemDef.name)
            {
                case "ItemChampionOnKill":
                    itemChampionOnKillIndex = ItemAPI.Add(customItem);
                    
                    break;
                case "LargerTeleporterRadius":
                    customItem.ItemDisplayRules = GenerateItemDisplayRulesMT();
                    largerTeleporterRadiusIndex = ItemAPI.Add(customItem);
                    break;
                case "CrippleOnHit":
                    customItem.ItemDisplayRules = GenerateItemDisplayRulesDagger();
                    crippleOnHitIndex = ItemAPI.Add(customItem);
                    break;
                case "CloakOnInteraction":  
                    cloakOnInteractionIndex = ItemAPI.Add(customItem);
                    break;
                case "ItemOnLevelUp2":
                    itemOnLevelUpIndex = ItemAPI.Add(customItem);
                    break;
                case "RandomDebuffOnHit":
                    randomDebuffOnHitIndex = ItemAPI.Add(customItem);
                    break;
                case "MoneyOnInteraction":
                    //customItem.ItemDisplayRules = GenerateItemDisplayRulesKeyCard();
                    moneyOnInteractionIndex = ItemAPI.Add(customItem);
                    
                    break;
                case "BarrierOnCrit":
                    barrierOnCritIndex = ItemAPI.Add(customItem);
                    break;
                case "BarrierOnLevel":
                    customItem.ItemDisplayRules = GenerateItemDisplayRulesBBA(); 
                    barrierOnLevelIndex = ItemAPI.Add(customItem);
                    break;
                case "ExperienceOnHit":
                    experienceOnHitIndex = ItemAPI.Add(customItem);
                    break;
                case "Lemdog":
                    customItem.ItemDisplayRules = GenerateItemDisplayRulesLD();
                    lemdogIndex = ItemAPI.Add(customItem);
                    break;
                case "ExtendedEnemyBuffDuration":
                    extendEnemyBuffDurationIndex = ItemAPI.Add(customItem);
                    break;
                case "WyattWalkman":
                    throw new System.Exception("Custodian's walkman should not be registered.");
                    //wyattWalkmanIndex = ItemAPI.Add(customItem);
                    //break;
                default:
                    LogCore.LogF(itemDef.name + " is unregistered! Item will not spawn!");
                    break;
                    /*case "DamageOnDamage":
                        damageOnDamagedIndex = ItemAPI.Add(customItem);
                        break;
                    case "FreezeEnemiesOnHit":
                        freezeEnemiesOnHitIndex = ItemAPI.Add(customItem);
                        break;
                    case "GeigerCounter":
                        geigerCounterIndex = ItemAPI.Add(customItem);
                        break;
                    case "Heart":
                        heartIndex = ItemAPI.Add(customItem);
                        break;
                    case "Chip":
                        chipIndex = ItemAPI.Add(customItem);
                        break;
                    case "Skin":
                        skinIndex = ItemAPI.Add(customItem);
                        break;*/
                    //how will moonfall ever recover
            }
        }

        private ItemDisplayRuleDict GenerateItemDisplayRulesDagger()
        {

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = keyCardMDL,
                    childName = "Pelvis",
                    localPos = new Vector3(-0.004f, 0.09f, 0.06f),
                    localAngles = new Vector3(-10.305f, 0.057f, -0.626f),
                    localScale = new Vector3(100, 100, 100)

        }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = keyCardMDL,
                    childName = "Pelvis",
                    localPos = new Vector3(0.008f, 0.036f, -0.034f),
                    localAngles = new Vector3(3.176f, -92.262f, -86.522f),
                    localScale = new Vector3(50, 50, 50)
        }
});

            return rules;
        }
        private ItemDisplayRuleDict GenerateItemDisplayRulesBBA()
        {

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = bbaMDL,
                    childName = "Chest",
                    localPos = new Vector3(-0.001f, -0.5F, -0.004f),
                    localAngles = Vector3.zero,
                    localScale = new Vector3(0.5f   ,  0.5f, 0.8f)

        }
            });
            return rules;
        }
        private ItemDisplayRuleDict GenerateItemDisplayRulesMT()
        {

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = mechanicalTrinketMDL,
                    childName = "Chest",
                    localPos = new Vector3(-0.0012f, 0.2745f, 0.2034f),
                    localAngles = new Vector3(-18.398f, 0.02f, -0.125f),
                    localScale = new Vector3(5,5, 5)

        }
            });
            return rules;
        }


        private ItemDisplayRuleDict GenerateItemDisplayRulesLD()
        {

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0, 0.4f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)

        }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "HeadCenter",
                    localPos = new Vector3(0.08f, 0.899f, 1.84f),
                    localAngles = new Vector3(0,-90,0),
                    localScale = new Vector3(5, 5, 5)
                }
});

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "HeadCenter",
                    localPos = new Vector3(0, 0.3f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0, 0.4f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0, 0.4f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "FlowerBase",
                    localPos = new Vector3(0, 2, 0),
                    localAngles = new Vector3(90,90,180),
                    localScale = new Vector3(2,  2, 2)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0, 0.4f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0.48f, 5f, -0.01f),
                    localAngles = new Vector3(15.79083f, 90,106.9842f),
                    localScale = new Vector3(5, 5, 5)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = lemdogMDL,
                    childName = "Head",
                    localPos = new Vector3(0, 0.4f, 0),
                    localAngles = new Vector3(0,90,0),
                    localScale = new Vector3(1   ,  1, 1)
                }
            });

            return rules;
        }




        protected internal void Hook()
        {
            On.RoR2.GlobalEventManager.OnCrit += GlobalEventManager_OnCrit;
            On.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;
            On.RoR2.GlobalEventManager.OnTeamLevelUp += GlobalEventManager_OnTeamLevelUp;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteractionOnTeleporterBeginChargingGlobal;
            On.RoR2.GenericPickupController.GrantItem += GrantItem;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            if (self)
            {
                if (self.inventory)
                {
                    var inv = self.inventory;
                    var earringsCount = inv.GetItemCount(extendEnemyBuffDurationIndex);
                    var lemCount = inv.GetItemCount(lemdogIndex);
                    var def = BuffCatalog.GetBuffDef(buffType);

                    if (lemCount > 0)
                    {
                        if (def.isDebuff && Util.CheckRoll(25 + (lemCount * 2.5f), self.master))
                        {
                            var random = Random.Range(0, lemdogList.Count);
                            var buff = lemdogList[random];
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
            orig(self, buffType, duration);
        }



        public void GrantItem(On.RoR2.GenericPickupController.orig_GrantItem orig, GenericPickupController self, CharacterBody body, Inventory inventory)
        {
            orig(self, body, inventory);
            int cloakOnInteractionCount = inventory.GetItemCount(cloakOnInteractionIndex);
            if (self && inventory && cloakOnInteractionCount > 0)
            {
                body.AddTimedBuff(BuffIndex.Cloak, 2 * cloakOnInteractionCount);
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker && self)
            {
                GameObject attacker = damageInfo.attacker;
                if (damageInfo.attacker.HasComponent<CharacterBody>())
                {
                    CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                    if (attackerBody.inventory)
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
                            count = inv2.GetItemCount(barrierOnLevelIndex);
                        }

                        int experienceOnHitCount = inv.GetItemCount(experienceOnHitIndex);

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
                }
            }
            var combinedHealthb4Damage = self.combinedHealth;
            orig(self, damageInfo);

        }
        #region GlobalEventManager
        public void GlobalEventManager_OnInteractionBegin(On.RoR2.GlobalEventManager.orig_OnInteractionBegin orig, GlobalEventManager self, Interactor interactor, IInteractable interactable, UnityEngine.GameObject interactableObject)
        {
            orig(self, interactor, interactable, interactableObject);
            CharacterBody characterBody = interactor.GetComponent<CharacterBody>();
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory && !interactableObject.HasComponent<GenericPickupController>())
                {
                    int moneyOnInteractionCount = inventory.GetItemCount(moneyOnInteractionIndex);

                    if (moneyOnInteractionCount > 0)
                    {
                        TeamManager.instance.GiveTeamMoney(characterBody.teamComponent.teamIndex, 5U + (uint)(moneyOnInteractionCount * 5));
                        //characterBody.master.GiveMoney(5U + (uint)(moneyOnInteractionCount * 5));
                    }
                }
            }
        }

        public void GlobalEventManager_OnCrit(On.RoR2.GlobalEventManager.orig_OnCrit orig, GlobalEventManager self, CharacterBody body, CharacterMaster master, float procCoefficient, ProcChainMask procChainMask)
        {
            if (body && procCoefficient > 0f && body && master && master.inventory)
            {
                Inventory masterInventory = master.inventory;
                HealthComponent healthComponent = body.healthComponent;
                int topazLense = masterInventory.GetItemCount(barrierOnCritIndex);
                //int lightningEye = masterInventory.GetItemCount(lightningOnCritIndex);
                if (topazLense > 0 && healthComponent)
                {
                    //oddly, this doesn't null reference
                    healthComponent.AddBarrier(5f + (topazLense * 3f));
                }
            }
            orig(self, body, master, procCoefficient, procChainMask);
        }

        public void GlobalEventManagerOnOnCharacterDeath(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody || !damageReport.attacker || !damageReport.victim || !damageReport.attackerMaster || !damageReport.victimMaster || damageReport == null)
                return;
            CharacterBody attackerBody = damageReport.attackerBody;
            var victimBody = damageReport.victimBody;
            CharacterMaster attackerMaster = damageReport.attackerMaster;
            Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;

            int itemChampionOnKillCount = attackerInventory.GetItemCount(itemChampionOnKillIndex);

            if (itemChampionOnKillCount > 0 && damageReport.victimIsBoss && Util.CheckRoll(15 + (itemChampionOnKillCount * 5), attackerMaster)) {
                Util.PlaySound("ui_obj_casinoChest_open", attackerBody.gameObject);
                EffectData data = new EffectData
                {
                    scale = 1000,
                    origin = victimBody.transform.position,
                };
                LogCore.LogI("YOOOOOOOOOOO!");
                EffectManager.SpawnEffect(EntityStates.Toolbot.ToolbotDash.knockbackEffectPrefab, data, true);
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(GetRandomItem(bossitemList)), victimBody.transform.position, new Vector3(0, 50, 0));

            }
        }

        public void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody || !damageReport.attacker || !damageReport.victim || !damageReport.attackerMaster || !damageReport.victimMaster || damageReport == null)
                return;
            if (damageReport.attackerBody && damageReport != null)
            {
                //CharacterBody attackerBody = damageReport.attackerBody;
                CharacterBody victimBody = damageReport.victimBody;
                CharacterBody attackerBody = damageReport.attackerBody;
                CharacterMaster attackerMaster = damageReport.attackerMaster;
                CharacterMaster victimMaster = damageReport.victimMaster;
                Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
                //Inventory victimInventory = victimMaster ? victimMaster.inventory : null;

                int scpRandom = UnityEngine.Random.Range(0, scpBuffList.Count);
                int rootCount = attackerInventory.GetItemCount(crippleOnHitIndex);
                int scpCount = attackerInventory.GetItemCount(randomDebuffOnHitIndex);
                //int freezeCount = attackerInventory.GetItemCount(freezeEnemiesOnHitIndex);


                if (rootCount > 0 && Util.CheckRoll(20, attackerMaster) && attackerMaster && damageReport.victimBody)
                {
                    victimBody.AddTimedBuff(BuffIndex.Cripple, 3);
                }

                if (scpCount > 0 && damageReport.victimBody && victimMaster)
                {
                    victimBody.AddTimedBuff(scpBuffList[scpRandom], (scpCount * 2));
                }

            }
        }

        public void GlobalEventManager_OnTeamLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
            for (int i = 0; i < teamMembers.Count; i++)
            {
                TeamComponent teamComponent = teamMembers[i];
                if (teamComponent)
                {
                    CharacterBody characterBody = teamComponent.GetComponent<CharacterBody>();
                    if (characterBody)
                    {
                        if (NetworkServer.active)
                        {
                            CharacterMaster master = characterBody.master;
                            if (master)
                            {
                                int itemOnLevelUpCount = master.inventory.GetItemCount(itemOnLevelUpIndex);
                                int barrierOnLevelUpCount = master.inventory.GetItemCount(barrierOnLevelIndex);

                                if (itemOnLevelUpCount > 0 && Util.CheckRoll(50, master))
                                {
                                    DropShipCall(characterBody.transform, itemOnLevelUpCount);
                                    
                                }
                                /*if (barrierOnLevelUpCount > 0)
                                {
                                    if (characterBody.healthComponent)
                                    {
                                        characterBody.healthComponent.AddBarrier(25 + (5 * barrierOnLevelUpCount));
                                    }
                                }*/
                            }
                        }
                    }
                }
            }
            orig(teamIndex);
        }
        #endregion

        public void TeleporterInteractionOnTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            int count = Util.GetItemCountForTeam(TeamIndex.Player, largerTeleporterRadiusIndex, false, true);
            if (count > 0)
            {
                obj.holdoutZoneController.baseRadius = obj.holdoutZoneController.baseRadius + (count * 3);
            }
        }

    }
    public class WyattItemWalkmanBehavior : CharacterBody.ItemBehavior
    {
        //VICE VS YOU
        //YOUUUUU
        private float timer;
        private WyattWalkmanBehavior behavior;

        public void Start() {
            behavior = body.gameObject.GetComponent<WyattWalkmanBehavior>();
        }

        public void FixedUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 3)
            {
                if (body.hasAuthority)
                {
                }
                timer = 0;
            }
            /*else
            {
                if (body.hasAuthority)
                {
                    behavior.UntriggerAuthority();
                }
                timer = 0;
            }*/
        }
    }
    public class DormantFungusBehavior : CharacterBody.ItemBehavior
    {
        //VICE VS YOU
        //YOUUUUU
        private float timer;
        public void FixedUpdate()
        {
            if (body.isSprinting)
            {
                timer += Time.deltaTime;
                if (timer >= 5)
                {
                    body.healthComponent.HealFraction(.1f * stack, default);
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }
    public class DairyBehavior : CharacterBody.ItemBehavior
    {
        //VICE VS YOU
        //YOUUUUU
        private float timer;
        private bool fatalLogged;
        public void FixedUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 3)
            {
                if (body.teamComponent)
                {
                    TeamManager.instance.GiveTeamExperience(body.teamComponent.teamIndex, (ulong)(1 * stack));
                }
                else
                {
                    if (!fatalLogged)
                    {
                        fatalLogged = true;
                        LogCore.LogF(Language.GetString(body.baseNameToken) + " doesn't have a team component!");
                    }
                }
                timer = 0;
            }
        }
    }
    public class GeigerCounterBehavior : CharacterBody.ItemBehavior
    {
        private float counter;
        public void Start()
        {
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            if (self.characterBody.Equals(body))
            {
                if (!self.characterBody.skillLocator.primary.Equals(self))
                {
                    counter += 1;
                }
            }
            orig(self);
        }

        public void FixedUpdate()
        {
            if (counter == 10)
            {
                //LogCore.LogD("Fired!");

                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/LightningStakeNova"), new EffectData()
                {
                    scale = 25,
                    origin = transform.position,
                    rotation = Quaternion.identity
                }, true);

                BlastAttack attack = new BlastAttack()
                {
                    attacker = base.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = (2.5f + (2.5f * stack)) * body.damage,
                    baseForce = 2500,
                    bonusForce = new Vector3(0, 0, 0),
                    crit = body.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.BlightOnHit,
                    falloffModel = BlastAttack.FalloffModel.None,
                    impactEffect = EffectIndex.Invalid,
                    inflictor = base.gameObject,
                    losType = BlastAttack.LoSType.NearestHit,
                    position = base.transform.position,
                    procChainMask = default,
                    procCoefficient = 0.5f,
                    radius = 25,
                    teamIndex = body.teamComponent.teamIndex,
                };
                attack.Fire();
                counter = 0;
            }
        }
    }
    public class SkinBehavior : CharacterBody.ItemBehavior
    {
        private float timer;

        public void FixedUpdate()
        {
            timer += Time.deltaTime;
            //LogCore.LogI(timer);

            if (timer >= 1)
            {
                var count = body.GetBuffCount(BuffCore.instance.skinIndex);
                for (int i = 0; i < count; i++)
                {
                    body.RemoveBuff(BuffCore.instance.skinIndex);
                }

                var cap = 20 + stack;

                foreach (var collision in Physics.OverlapSphere(transform.position, 25))
                {
                    var cb = collision.GetComponentInParent<CharacterBody>();
                    if (cb && !cb.Equals(body) && body.GetBuffCount(BuffCore.instance.skinIndex) < cap)
                    {
                        body.AddBuff(BuffCore.instance.skinIndex);
                        //LogCore.LogI(Language.GetString(cb.baseNameToken));
                    }
                }
                timer = 0;
            }
        }
    }
}