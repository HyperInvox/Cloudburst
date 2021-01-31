using Cloudburst.Cores.Components.Wyatt;
using EntityStates.Scrapper;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
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
        protected internal ItemIndex armorOnCd; //rob if he wants to do texturing
        protected internal ItemIndex randomDebuffOnHitIndex; //model and icon
        protected internal ItemIndex moneyOnInteractionIndex; //rob
        protected internal ItemIndex barrierOnCritIndex; //does not have a model
        protected internal ItemIndex barrierOnLevelIndex; //model and icon
        protected internal ItemIndex experienceOnHitIndex; //has icon
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

        public GameObject projectileBlastPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");

        public const string japesLore = @"""In the cold rain, amongst the remnants of ships long fallen, shouting could be heard. Shrouded in fog, were three figures crowded around the spoils of a blue security chest.""

""I found it first! You keep your hands off of it!""

""I’ve barely gotten any items! You don’t need another one!""

""Barely gotten any items? All I have is this shitty rusty key! Give me that item!""

Their argument abruptly stopped when they realized what they had been fighting over was gone. In it’s place was a single, small holo-note.

""I needed this for my build, I hope you don’t mind.""

";
        public const string japesLor3 = @"""Quartermaster’s log. 17 days after the crash.

It has been more than half a month since we’ve nearly burnt ourselves into smoldering black paste during our unwilling introduction to the atmosphere of this murder-planet.Due to the nature of our arrival, that being escape from a collapsing cargo ship that had been ripped out of warp travel, our supply of necessities has been dwindling from an already dangerously low base count. We have been forced to ration what little food and water we have. To keep in check of this, I have been elected as Quartermaster of our outpost. 
Our supplies are as follows:

-Enough food and water for around 1 week and a half.
-3 and a half cardboard boxes full of salvaged metal and circuitry from destroyed drones and sentries.
-4 bags of medical equipment 
-2 keychains, each with 13 rusted keys? (This must be an error, Juarez says it was brought in alongside the rest of the haul that those three blokes brought in 2 days ago, are they printing these or something?)
-6 boxes full of ammunition (potentially dwindling at an exponential rate)

Something isn’t lining up. The bulletin board we put up to track how much supplies everyone’s been taking isn’t covering all of it. While this may be just simple miscommunication on everyone’s part, the chance of somebody taking more than what they need is steadily increasing with each day. I don’t have any evidence to point towards this exactly, but it is a (very frightening) possibility. 


If it’s just one person taking it all, then at the rates that have been shown... 
They just might be more prepared than the rest of us.

End log.


Addendum: WHO. IN GOD ABOVE’S HOLY NAME. TOOK. MY. CIGARETTES??”""";

        public const string redactedLore = @"""Order: ""Irregularity Classification - [/]-[//]-[//]""
Tracking Number: 89*****
Estimated Delivery: 03/27/2056
Shipping Method:  High Priority/Biological
Shipping Address: [/////////////], District [//], [/////]
Shipping Details:


Following the Incident at branch facility [//]-[///], we have decided to transfer Irregularity [/]-[//]-[//] to our main facility, Facility [/]-[///].

Irregularity [/]-[//]-[//] shall be placed in a Level [TEHOM] containment chamber, and shall be constantly monitored for any changes in behavior. Experiments and Containment upkeep shall be conducted remotely, or by personnel with a Cognitional Endurance grade of no less than Five. 
Under no circumstance should Irregularity [/]-[//]-[//] be removed from its room of containment without proper authorization and obfuscation procedures. Should Irregularity [/]-[//]-[//] be lost in transit and left in a potentially visible location, an Irregularity Recontainment Unit MUST BE DISPATCHED TO RECOVER THE IRREGULARITY IMMEDIATELY.

All research on Irregularity [/]-[//]-[//] shall be focused on preventing it from incubating. Work on improving the anti-perception device implanted on it to alter the perception on more than digital devices will come later.

Hold strong, and keep your heads up.
-[///////]""";

        //string a = "\In the cold rain, amongst the remnants of ships long fallen, shouting could be heard. Shrouded in fog, were three figures crowded around the spoils of a blue security chest.\r\n\r\n\"I found it first! You keep your hands off of it!\"\r\n\r\n\"I’ve barely gotten any items! You don’t need another one!\"\r\n\r\n\"Barely gotten any items? All I have is this shitty rusty key! Give me that item!\"\r\n\r\n\Their argument abruptly stopped when they realized what they had been fighting over was gone. In it’s place was a single, small holo-note.\r\n\r\nI needed this for my build, I hope you don’t mind.\"\r\n\r\n\"";


        public List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
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
                        case "IMDLHarvester":
                            harvesterMDL = item;
                            break;
                        case "IMDLRabbitFoot":
                            topazLensMDL = item;
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
                loreToken = "ITEM_CLOAKBUFFONINTERACTION_LORE",
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
                descriptionToken = "ITEM_EXPLOSIONGROUND_DESC",
                hidden = false,
                loreToken = "",
                name = "ExplosionGround",
                nameToken = "ITEM_EXPLOSIONGROUND_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/CarePackageRequester/icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/CarePackageRequester/IMDLCarePackageRequester.prefab",
                pickupToken = "ITEM_EXPLOSIONGROUND_PICKUP",
                tags = itemOnLevelUpTags,
                tier = ItemTier.Tier2,
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
                pickupIconPath = "textures/miscicons/texMysteryIcon",
                pickupModelPath = "prefabs/pickupmodels/PickupMystery",
                pickupToken = "ITEM_RANDOMDEBUFFONHIT_PICKUP",
                tags = randomDebuffOnHitTags,
                tier = ItemTier.Tier3,
                unlockableName = ""
            });
            ItemTag[] moneyOnInteractionTags = new ItemTag[1]
            {
                ItemTag.Damage,
            };
            RegisterItem(new ItemDef
            {
                canRemove = true,
                descriptionToken = "ITEM_MONEYONINTERACTION_DESC",
                hidden = false,
                loreToken = "",
                name = "MoneyOnInteraction",
                nameToken = "ITEM_MONEYONINTERACTION_NAME",
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/UESKeycard/icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/UESKeycard/IMDLKeycard.prefab",
                pickupToken = "ITEM_MONEYONINTERACTION_PICKUP",
                tags = moneyOnInteractionTags,
                tier = ItemTier.Tier2,
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
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/TopazLense/icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/TopazLense/IMDLRabbitFoot.prefab",
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
                pickupIconPath = "@Cloudburst:Assets/Cloudburst/Items/Harvester/icon.png",
                pickupModelPath = "@Cloudburst:Assets/Cloudburst/Items/Harvester/IMDLHarvester.prefab",
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
            LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_DESC", "On boss death, <style=cIsUtility>25% chance <style=cStack>(+10% per stack)</style> for bosses to drop a random boss item when killed.</style> Nearby projectiles are also destroyed, gain 5 <style=cStack>(+5 per stack)</style> for each destroyed projectile. ");
            LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_PICKUP", "On death, bosses have a chance to drop an item. Nearby projectiles are also destroyed, gain barrier for each destroyed projectile.");

            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_NAME", "Mechanical Trinket"); //model
            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_DESC", "<style=cIsUtility>Increase teleporter radius by 3m</style> <style=cStack>(+3m per stack)</style>.");
            LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_PICKUP", "Increase teleporter radius.");

            LanguageAPI.Add("ITEM_CRIPPLEONHIT_NAME", "Fabinhoru's Dagger");
            LanguageAPI.Add("ITEM_CRIPPLEONHIT_DESC", "8% Chance to <style=cIsDamage>cripple enemies</style> for 3 <style=cStack>(+3 seconds per stack)</style> seconds.");
            LanguageAPI.Add("ITEM_CRIPPLEONHIT_PICKUP", "Chance to cripple enemies on hit.");


            //var nArmor = armor + (0.1f + (count * 0.2f));
            //var nRegen = regen + (0.1f + (count * 0.2f));
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_NAME", "Jape's Cloak");
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_DESC", "Gain a buff that grants you <style=cIsUtility>+5 armor</style> and <style=cIsHealing>30% healing</style> when picking up an item. Maximum cap of 3 buffs <style=cStack>(+2 per stack)</style>.");
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_PICKUP", "Gain a buff that grants armor and healing on item pickup.");
            LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_LORE", japesLor3);

            //LanguageAPI.Add("ITEM_ITEMONLEVELUP_NAME", "Opportunists Charm");
            //LanguageAPI.Add("ITEM_ITEMONLEVELUP_DESC", "For each skill on cooldown, recieve an armor buff that gives you <style=cIsUtility>+10 <style=cStack>+10 chance per stack</style> armor</style>. ");
            //LanguageAPI.Add("ITEM_ITEMONLEVELUP_PICKUP", "Recieve armor for each skill on cooldown");

            LanguageAPI.Add("ITEM_EXPLOSIONGROUND_NAME", "Blastboot Shell");
            LanguageAPI.Add("ITEM_EXPLOSIONGROUND_DESC", "On use of your secondary, if in midair, to <style=cIsDamage>spawn an explosion beneath you</style> that does <style=cIsDamage>100%<style=cStack>+250% chance per stack</style> damage</style> and <style=cIsUtility>boosts you upwards</style>");
            LanguageAPI.Add("ITEM_EXPLOSIONGROUND_PICKUP", "On hit, spawn an explosion beneath you if in midair.");


            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_NAME", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_DESC", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_PICKUP", "[REDACTED]");
            LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_LORE", redactedLore);

            LanguageAPI.Add("ITEM_MONEYONINTERACTION_NAME", "Enigmatic Keycard");
            LanguageAPI.Add("ITEM_MONEYONINTERACTION_DESC", "10% chance on hit to spawn a <style=cIsDamage>void orb</style> that does <style=cIsDamage>100% <style=cStack>(+100% per stack)</style></style>.");
            LanguageAPI.Add("ITEM_MONEYONINTERACTION_PICKUP", "Chance to spawn a void orb on hit.");

            LanguageAPI.Add("ITEM_BARRIERONCRIT_NAME", "Lucky Rabbit Foot");
            LanguageAPI.Add("ITEM_BARRIERONCRIT_DESC", "Gain a <style=cIsHealing>temporary barrier</style> on critical hits for <style=cIsHealing>5 health</style> <style=cStack>(+3 per stack)</style>.");
            LanguageAPI.Add("ITEM_BARRIERONCRIT_PICKUP", "Gain barrier on critical hits");

            LanguageAPI.Add("ITEM_BARRIERONLEVEL_NAME", "Broken Body Armor");
            LanguageAPI.Add("ITEM_BARRIERONLEVEL_DESC", "Gain a buff that grants 8 <style=cStack>(+8 per stack)</style> <style=cIsUtility>armor</style> when hurt.");
            LanguageAPI.Add("ITEM_BARRIERONLEVEL_PICKUP", "Gain an armor buff when hurt ");

            LanguageAPI.Add("ITEM_EXPERIENCEONHIT_NAME", "Glass Harvester");
            LanguageAPI.Add("ITEM_EXPERIENCEONHIT_DESC", "Gain 3 <style=cStack>(+2 per stack)</style> <style=cIsUtility>experience</style> on hit.");
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
                case "ExplosionGround":
                    armorOnCd = ItemAPI.Add(customItem);
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
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(5, 2, 3)

        }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
        }
}); rules.Add("mdlToolbot", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
                }
});

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = fabDagMDL  ,
                    childName = "Chest",
                    localPos = new Vector3(0, 0.2f, 0.3f),
                    localAngles = new Vector3(-180f, 0, -0),
                    localScale = new Vector3(3, 2, 3)
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
                    localScale = new Vector3(10,10, 10)

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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManagerOnOnCharacterDeath;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteractionOnTeleporterBeginChargingGlobal;
            On.RoR2.GenericPickupController.GrantItem += GrantItem;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<BlastBootShell>(self.inventory.GetItemCount(armorOnCd));
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {

            CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            CharacterMaster attackerMaster = attackerBody ? attackerBody.master : null ;
            Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
            //Inventory victimInventory = victimMaster ? victimMaster.inventory : null;

            if (attackerBody && attackerMaster && attackerInventory)
            {

                int scpRandom = UnityEngine.Random.Range(0, scpBuffList.Count);
                int rootCount = attackerInventory.GetItemCount(crippleOnHitIndex);
                int scpCount = attackerInventory.GetItemCount(randomDebuffOnHitIndex);
                int moneyCount = attackerInventory.GetItemCount(moneyOnInteractionIndex);

                /*var ppos = CloudUtils.FindBestPosition(victimBody.mainHurtBox);
                EffectData ddata = new EffectData()
                {
                    rotation = Quaternion.Euler(victimBody.transform.forward),
                    scale = 15,
                    origin = ppos,
                };
                var dddamage = moneyCount * 2.5f;
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), ddata, true);
                FireProjectileInfo iinfo = new FireProjectileInfo()
                {
                    crit = false,
                    damage = dddamage * attackerBody.damage,
                    damageColorIndex = RoR2.DamageColorIndex.Default,
                    damageTypeOverride = DamageType.Generic,
                    force = 0,
                    owner = attackerBody.gameObject,
                    position = ppos,
                    procChainMask = default,
                    projectilePrefab = ProjectileCore.orbitalOrb,   
                    rotation = Util.QuaternionSafeLookRotation(victimBody.transform.position),
                    target = victim,
                    useFuseOverride = false,
                    useSpeedOverride = true,
                    _speedOverride = 100
                };
                ProjectileManager.instance.FireProjectile(iinfo);*/


                if (moneyCount > 0 && Util.CheckRoll(10 * damageInfo.procCoefficient, attackerMaster) && victimBody && !damageInfo.procChainMask.HasProc(ProcType.AACannon)) {
                    var pos = CloudUtils.FindBestPosition(victimBody.mainHurtBox);

                    damageInfo.procChainMask.AddProc(ProcType.AACannon);
                    EffectData data = new EffectData()
                    {
                        rotation = Quaternion.Euler(victimBody.transform.forward),
                        scale = 3,
                        origin = pos,
                    };
                    var ddamage = moneyCount * 1f;
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierSpawnEffect"), data, true);
                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        crit = false,
                        damage = ddamage * attackerBody.damage,
                        damageColorIndex = RoR2.DamageColorIndex.Default,
                        damageTypeOverride = DamageType.Generic,
                        force = 0,
                        owner = attackerBody.gameObject,
                        position = pos,
                        procChainMask = default,
                        projectilePrefab = ProjectileCore.orbitalOrbPlayer,
                        rotation = Util.QuaternionSafeLookRotation(victimBody.transform.position),
                        target = victim,
                        useFuseOverride = false,
                        useSpeedOverride = true,
                        _speedOverride = 100
                    };
                    ProjectileManager.instance.FireProjectile(info);

                }

                

                if (rootCount > 0 && Util.CheckRoll(8 * damageInfo.procCoefficient, attackerMaster) && attackerMaster && victimBody)
                {
                    victimBody.AddTimedBuff(BuffIndex.Cripple, 3);
                }

                if (scpCount > 0 && victimBody)
                {
                    victimBody.AddTimedBuff(scpBuffList[scpRandom], (scpCount * 2));
                }
            }
            orig(self, damageInfo, victim);
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
                        if (def.isDebuff && Util.CheckRoll(25 + (lemCount * 2.5f), self.master) && buffType != BuffIndex.NullSafeZone)
                        {
                            var random = UnityEngine.Random.Range(0, lemdogList.Count);
                            var buff = lemdogList[random];
                            buffType = buff;
                        }
                    }
                    if (earringsCount > 0 && buffType != BuffIndex.MedkitHeal & buffType != BuffIndex.ElementalRingsCooldown && buffType != BuffIndex.HiddenInvincibility && !def.isDebuff && buffType != BuffCore.instance.antiGravFriendlyIndex)
                    {
                        //do thing???
                        duration += 2 + (1 * earringsCount);
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
                if (body && body.GetBuffCount(BuffCore.instance.japesCloak) < 2 + cloakOnInteractionCount)
                {
                    body.AddBuff(BuffCore.instance.japesCloak);
                }
                //body.AddTimedBuff(BuffCore.instance.japesCloak, 30);
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
                            float exp = 1 + (experienceOnHitCount * 2);
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

            if (itemChampionOnKillCount > 0 && damageReport.victimIsBoss && attackerBody && attackerMaster) {
                Util.PlaySound("ui_obj_casinoChest_open", attackerBody.gameObject);
                if (Util.CheckRoll(15 + (itemChampionOnKillCount * 5))) {
                    EffectData data = new EffectData
                    {
                        scale = 1000,
                        origin = victimBody.transform.position,
                    };
                    LogCore.LogI("YOOOOOOOOOOO!");
                    EffectManager.SpawnEffect(EntityStates.Toolbot.ToolbotDash.knockbackEffectPrefab, data, true);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(GetRandomItem(bossitemList)), victimBody.transform.position, new Vector3(0, 50, 0));
                }
                Collider[] array = Physics.OverlapSphere(victimBody.transform.position, 10, LayerIndex.projectile.mask);


                for (int i = 0; i < array.Length; i++)
                {
                    ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                    if (pc)
                    {
                        //if its not a friendly
                        if (pc.teamFilter.teamIndex != attackerBody.teamComponent.teamIndex)
                        {
                            attackerBody.healthComponent.AddBarrier(5 * itemChampionOnKillCount);
                            EffectManager.SpawnEffect(projectileBlastPrefab, new EffectData {
                                origin = pc.transform.position
                            }, true);
                        }
                    }
                }
            }
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

    public class BlastBootShell : CharacterBody.ItemBehavior
    {
        public void Start()
        {
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            orig(self);
            if (body.skillLocator.secondary == self)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(body.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
                {
                    body.characterMotor.ApplyForce(new Vector3(0, body.characterMotor.mass * 5, 0));
                    //stops infinite loops!
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXLemurianBruiserFireballImpact"), new EffectData
                    {
                        scale = 10,
                        rotation = Quaternion.identity,
                        origin = raycastHit.point,
                    }, true);
                    float multiplier = 1;
                    if (stack > 1)
                    {
                        multiplier = 1 + (stack * 2.5f);
                    }
                    new BlastAttack
                    {
                        position = raycastHit.point,
                        //baseForce = 3000,
                        attacker = null,
                        inflictor = null,

                        teamIndex = body.teamComponent.teamIndex,
                        baseDamage = body.damage * multiplier,
                        attackerFiltering = default,
                        //bonusForce = new Vector3(0, -3000, 0),
                        damageType = DamageType.Stun1s, //| DamageTypeCore.spiked,
                        crit = body.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = BlastAttack.FalloffModel.None,
                        //impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/PulverizedEffect").GetComponent<EffectIndex>(),
                        procCoefficient = 0,
                        radius = 10
                    }.Fire();
                }
            }
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

        public void FixedUpdate()
        {
            if (body)
            {
                foreach (GenericSkill i in body.skillLocator.allSkills) {
                    if (i && (i.rechargeStopwatch > 0))
                    {
                        if (NetworkServer.active)
                        {
                            body.AddBuff(BuffCore.instance.charmIndex);
                        }
                    }
                    else
                    {
                        if (NetworkServer.active)
                        {
                            CloudUtils.SafeRemoveBuff(BuffCore.instance.charmIndex, body);
                        }
                    }
                }
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
                foreach (var collision in Physics.OverlapSphere(transform.position, 25))
                {
                    var cb = collision.GetComponentInParent<CharacterBody>();
                    bool isTeam = false;
                    if (TeamComponent.GetObjectTeam(cb.gameObject) == TeamIndex.Player) {
                        isTeam = true;
                    }
                    if (cb && !isTeam)
                    {
                        if (cb.healthComponent && cb.healthComponent.combinedHealthFraction < 0.3f) {
                            
                        }
                        //LogCore.LogI(Language.GetString(cb.baseNameToken));
                    }
                }
                timer = 0;
            }
        }
    }
}