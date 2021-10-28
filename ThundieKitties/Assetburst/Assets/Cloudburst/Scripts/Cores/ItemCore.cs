using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cloudburst.Cores
{

    class ItemCore
    {
        public static ItemCore instance;

        public List<ItemDef> bossitemList = new List<ItemDef>{
            RoR2Content.Items.SiphonOnLowHealth,
            RoR2Content.Items.NovaOnLowHealth,
            RoR2Content.Items.Knurl,
            RoR2Content.Items.BeetleGland,
            RoR2Content.Items.TitanGoldDuringTP,
            RoR2Content.Items.SprintWisp,
            RoR2Content.Items.BleedOnHitAndExplode,
            RoR2Content.Items.FireballsOnHit 
            //Excluding pearls because those aren't boss Item, they come from the Cleansing Pool 
        };

        public List<BuffDef> scpBuffList = new List<BuffDef>{
            RoR2Content.Buffs.NullifyStack,
            RoR2Content.Buffs.ClayGoo,
            RoR2Content.Buffs.BeetleJuice,
            RoR2Content.Buffs.HealingDisabled
        };
        public List<BuffDef> eliteBuffList = new List<BuffDef>{
            RoR2Content.Buffs.AffixPoison,
            RoR2Content.Buffs.AffixRed,
            RoR2Content.Buffs.AffixBlue,
            RoR2Content.Buffs.AffixWhite
        };
        public List<BuffDef> lemdogList = new List<BuffDef>{
            RoR2Content.Buffs.Warbanner,
            //RoR2Content.Buffs.Cloak,
            RoR2Content.Buffs.CloakSpeed,
            //RoR2Content.Buffs.EngiShield,
            RoR2Content.Buffs.MeatRegenBoost,
            RoR2Content.Buffs.WhipBoost,
            RoR2Content.Buffs.TeamWarCry,
        };

        public List<ItemBuilder> Items = new List<ItemBuilder>();

        public ItemCore() => RegisterItems();


        protected internal void RegisterItems()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBuilder)));
            foreach (var mdl in AssetsCore.mainAssetBundle.LoadAllAssets<GameObject>())
            {
                if (mdl.name.Contains("IMDL"))
                {
                    var display = mdl.AddComponent<ItemDisplay>();
                    display.rendererInfos = CloudUtils.GatherRenderInfos(mdl);
                }
            }
            foreach (var itemType in ItemTypes)
            {
                ItemBuilder item = (ItemBuilder)System.Activator.CreateInstance(itemType);

                LogCore.LogI(item.ItemName);
                if (CloudburstPlugin.instance.ValidateItem(item, Items))
                {
                    item.Init(CloudburstPlugin.instance.GetConfig());
                }
            }
            instance = this;
        }


    }
}
/*foreach (var item in AssetsCore.mainAssetBundle.LoadAllAssets<GameObject>()) {
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

var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemCreator)));

foreach (var itemType in ItemTypes)
{
    ItemCreator item = (ItemCreator)System.Activator.CreateInstance(itemType);
    item.Init(null);
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
    pickupIconPath = "Assets/Cloudburst/Items/Grinder/icon.png", 
    pickupModelPath = "Assets/Cloudburst/Items/Grinder/IMDLGrinder.prefab",
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
    pickupIconPath = "Assets/Cloudburst/Items/MechanicalTrinket/Icon.png",
    pickupModelPath = "Assets/Cloudburst/Items/MechanicalTrinket/IMDLMechanicalTrinket.prefab",
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
    pickupIconPath = "Assets/Cloudburst/Items/FabinhoruDagger/icon.png",
    pickupModelPath = "Assets/Cloudburst/Items/FabinhoruDagger/IMDLDagger.prefab",
    pickupToken = "ITEM_CRIPPLEONHIT_PICKUP",
    tags = crippleOnHitTags,
    tier = ItemTier.Tier2,
    unlockableName = ""
});
ItemTag[] cloakOnInteractionTags = new ItemTag[2]
{

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
    pickupIconPath = "Assets/Cloudburst/Items/Cloak/JapeIcon.png",
    pickupModelPath = "Assets/Cloudburst/Items/Cloak/IMDLCloak.prefab",
    pickupToken = "ITEM_CLOAKBUFFONINTERACTION_PICKUP",
    tags = cloakOnInteractionTags,
    tier = ItemTier.Tier2,
    unlockableName = AchievementCore.GetUnlockableString("GrabOrDie")
});
ItemTag[] itemOnLevelUpTags = new 
RegisterItem(new ItemDef
{
    canRemove = true,
    descriptionToken = "ITEM_EXPLOSIONGROUND_DESC",
    hidden = false,
    loreToken = "",
    name = "ExplosionGround",
    nameToken = "ITEM_EXPLOSIONGROUND_NAME",
    pickupIconPath = "",
    pickupModelPath = "",
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
    loreToken = "ITEM_RANDOMDEBUFFONHIT_LORE",
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
    pickupIconPath = "Assets/Cloudburst/Items/UESKeycard/icon.png",
    pickupModelPath = "Assets/Cloudburst/Items/UESKeycard/IMDLKeycard.prefab",
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
    pickupIconPath = "Assets/Cloudburst/Items/TopazLense/icon.png",
    pickupModelPath = "Assets/Cloudburst/Items/TopazLense/IMDLRabbitFoot.prefab",
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
    pickupIconPath = "Assets/Cloudburst/Items/BrokenBodyArmor/brokenarmoricon.png",
    pickupModelPath = "Assets/Cloudburst/Items/BrokenBodyArmor/IMDLBrokenBodyArmor.prefab",
    pickupToken = "ITEM_BARRIERONLEVEL_PICKUP",
    tags = barrierOnLevelTags,
    tier = ItemTier.Tier1,
    unlockableName = ""
});
ItemTag[] experienceOnHitTags = 
RegisterItem(new ItemDef
{
    canRemove = true,
    descriptionToken = "ITEM_EXPERIENCEONHIT_DESC",
    hidden = false,
    loreToken = "",
    name = "ExperienceOnHit",
    nameToken = "ITEM_EXPERIENCEONHIT_NAME",
    pickupIconPath = "Assets/Cloudburst/Items/Harvester/icon.png",
    pickupModelPath = "Assets/Cloudburst/Items/Harvester/IMDLHarvester.prefab",
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
    pickupIconPath = "Assets/Cloudburst/Items/Lemdog/LemDog_TexIcon.png",
    pickupModelPath = "Assets/Cloudburst/Items/Lemdog/IMDLLemDog.prefab",
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
    loreToken = "ITEM_EXTENDEDENEMYBUFFDURATIOM_LORE",
    name = "ExtendedEnemyBuffDuration",
    nameToken = "ITEM_EXTENDEDENEMYBUFFDURATIOM_NAME",
    pickupIconPath = "Assets/Cloudburst/Items/Earrings/magicicon.png",
    pickupModelPath = "Assets/Cloudburst/Items/Earrings/IMDLEarrings.prefab",
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
});


//chipIndex
Hook();
}

protected internal void RegisterTokens()
{
R2API.LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_NAME", "Extractor"); //model
R2API.LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_DESC", "On boss death, <style=cIsUtility>25% chance <style=cStack>(+10% per stack)</style> for bosses to drop a random boss item when killed.</style> Nearby projectiles are also destroyed, gain 5 <style=cStack>(+5 per stack)</style> for each destroyed projectile. ");
R2API.LanguageAPI.Add("ITEM_ITEMONCHAMPIONKILL_PICKUP", "On death, bosses have a chance to drop an item. Nearby projectiles are also destroyed, gain barrier for each destroyed projectile.");

R2API.LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_NAME", "Mechanical Trinket"); //model
R2API.LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_DESC", "");
R2API.LanguageAPI.Add("ITEM_LARGERTELEPORTERRADIUS_PICKUP", "Increase teleporter radius.");

R2API.LanguageAPI.Add("ITEM_CRIPPLEONHIT_NAME", "Fabinhoru's Dagger");
R2API.LanguageAPI.Add("ITEM_CRIPPLEONHIT_DESC", "8% Chance to <style=cIsDamage>cripple enemies</style> for 3 <style=cStack>(+3 seconds per stack)</style> seconds.");
R2API.LanguageAPI.Add("ITEM_CRIPPLEONHIT_PICKUP", "Chance to cripple enemies on hit.");


//var nArmor = armor + (0.1f + (count * 0.2f));
//var nRegen = regen + (0.1f + (count * 0.2f));
R2API.LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_NAME", "Jape's Cloak");
R2API.LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_DESC", "Gain a buff that grants you <style=cIsUtility>+5 armor</style> and <style=cIsHealing>30% healing</style> when picking up an item. Maximum cap of 3 buffs <style=cStack>(+2 per stack)</style>.");
R2API.LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_PICKUP", "Gain a buff that grants armor and healing on item pickup.");
R2API.LanguageAPI.Add("ITEM_CLOAKBUFFONINTERACTION_LORE", japesLor3);

//R2API.LanguageAPI.Add("ITEM_ITEMONLEVELUP_NAME", "Opportunists Charm");
//R2API.LanguageAPI.Add("ITEM_ITEMONLEVELUP_DESC", "For each skill on cooldown, recieve an armor buff that gives you <style=cIsUtility>+10 <style=cStack>+10 chance per stack</style> armor</style>. ");
//R2API.LanguageAPI.Add("ITEM_ITEMONLEVELUP_PICKUP", "Recieve armor for each skill on cooldown");

R2API.LanguageAPI.Add("ITEM_EXPLOSIONGROUND_NAME", "Blastboot Shell");
R2API.LanguageAPI.Add("ITEM_EXPLOSIONGROUND_DESC", "On use of your secondary, if in midair, to <style=cIsDamage>spawn an explosion beneath you</style> that does <style=cIsDamage>100%<style=cStack>+250% chance per stack</style> damage</style> and <style=cIsUtility>boosts you upwards</style>");
R2API.LanguageAPI.Add("ITEM_EXPLOSIONGROUND_PICKUP", "");


R2API.LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_NAME", "[REDACTED]");
R2API.LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_DESC", "In combat, nearby low health enemies have a 5% chance to become <style=cIsDamage>Overwhelmed, slowing them and inflicting them with a strong non-lethal Damage-Over-Time that does 200% damage every three seconds and becomes more potent the lower their health is</style> for 8 <style=cStack>(+3 seconds</style> .");
R2API.LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_PICKUP", "Nearby enemies with low health have a chance to become overwhelmed, slowing them and inflicting with with a Damage-Over-Time");
R2API.LanguageAPI.Add("ITEM_RANDOMDEBUFFONHIT_LORE", redactedLore);

R2API.LanguageAPI.Add("ITEM_MONEYONINTERACTION_NAME", "Enigmatic Keycard");
R2API.LanguageAPI.Add("ITEM_MONEYONINTERACTION_DESC", "10% chance on hit to spawn a <style=cIsDamage>void orb</style> that does <style=cIsDamage>100% <style=cStack>(+100% per stack)</style></style>.");
R2API.LanguageAPI.Add("ITEM_MONEYONINTERACTION_PICKUP", "Chance to spawn a void orb on hit.");

R2API.LanguageAPI.Add("ITEM_BARRIERONCRIT_NAME", "Lucky Rabbit Foot");
R2API.LanguageAPI.Add("ITEM_BARRIERONCRIT_DESC", "Gain a <style=cIsHealing>temporary barrier</style> on critical hits for <style=cIsHealing>5 health</style> <style=cStack>(+3 per stack)</style>.");
R2API.LanguageAPI.Add("ITEM_BARRIERONCRIT_PICKUP", "Gain barrier on critical hits");

R2API.LanguageAPI.Add("ITEM_BARRIERONLEVEL_NAME", "Broken Body Armor");
R2API.LanguageAPI.Add("ITEM_BARRIERONLEVEL_DESC", "Gain a buff that grants 8 <style=cStack>(+8 per stack)</style> <style=cIsUtility>armor</style> when hurt.");
R2API.LanguageAPI.Add("ITEM_BARRIERONLEVEL_PICKUP", "Gain an armor buff when hurt ");

R2API.LanguageAPI.Add("ITEM_EXPERIENCEONHIT_NAME", "Glass Harvester");
R2API.LanguageAPI.Add("ITEM_EXPERIENCEONHIT_DESC", "Gain 3 <style=cStack>(+2 per stack)</style> <style=cIsUtility>experience</style> on hit.");
R2API.LanguageAPI.Add("ITEM_EXPERIENCEONHIT_PICKUP", "");

R2API.LanguageAPI.Add("ITEM_LEMDOG_NAME", "Lemdog");
R2API.LanguageAPI.Add("ITEM_LEMDOG_DESC", "25% <style=cStack>(+2.5% per stack)</style> chance for <style=cIsUtility>applied debuffs to become beneficial buffs</style>");
R2API.LanguageAPI.Add("ITEM_LEMDOG_PICKUP", "Chance for debuffs to become beneficial buffs when applied");

/*                    if (self.HasBuff(magicArmor))
        {
            self.armor += (8f * magicCount);
        }
        if (self.HasBuff(magicAttackSpeed))
        {
            self.attackSpeed += (0.1f * magicCount);
        }
        if (self.HasBuff(magicRegen))
        {
            self.regen += (0.2f * magicCount);
        }


string desc = "Every 15 seconds, gain a buff which can grant you one of the following:" + Environment.NewLine + Environment.NewLine;
desc = desc + "<style=cIsUtility>+8 <style=cStack>(+8 per stack)</style> armor" + Environment.NewLine + Environment.NewLine;
desc = desc + "<style=cIsHealing>20% <style=cStack>(+20% per stack)</style> regeneration</style>" + Environment.NewLine + Environment.NewLine;
desc = desc + "<style=cIsDamage>10% <style=cStack>(+10% per stack)</style> attack speed</style>" + Environment.NewLine + Environment.NewLine;
desc = desc + "Also extend <style=cIsUtility>positive buff duration</style> by 2 <style=cStack>(+1 per stack)</style> seconds." + Environment.NewLine + Environment.NewLine;

R2API.LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_NAME", "Magician's Earrings");
R2API.LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_DESC", desc);//"Every 15 seconds, gain a buff which can grant you one of the following: <style=cIsUtility>+8 <style=cStack>(+8 per stack)</style> armor, <style=cIsHealing>20% <style=cStack>(+20% per stack)</style> regeneration</style>, and <style=cIsDamage>10% <style=cStack>(+10% per stack)</style> attack speed</style>. Also extend <style=cIsUtility>positive buff duration</style> by 2<style=cStack>(+1 per stack)</style> seconds. ");
R2API.LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_PICKUP", "Every 15 seconds, gain a buff that can grant you armor, or regeneration, or attack speed. Also extend positive buff duration");
R2API.LanguageAPI.Add("ITEM_EXTENDEDENEMYBUFFDURATIOM_LORE", magicLore);



R2API.LanguageAPI.Add("ITEM_WYATTWALKMAN_NAME", "Walkman");
R2API.LanguageAPI.Add("ITEM_WYATTWALKMAN_DESC", "Gain <style=cIsUtility>+x% speed</style> and <style=cIsHealing>+x% regen</style> while in combat every 3 seconds. Maximum 10 stacks <style=cStack>(+1 per stack)</style>");
R2API.LanguageAPI.Add("ITEM_WYATTWALKMAN_PICKUP", "Gain speed and regeneration while in combat");

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
        case "DamageOnDamage":
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
            break;
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
self.AddItemBehavior<RedactedBehavior>(self.inventory.GetItemCount(randomDebuffOnHitIndex));
if (NetworkServer.active)
{
    self.AddItemBehavior<MagiciansEarringsBehavior>(self.inventory.GetItemCount(extendEnemyBuffDurationIndex));
}
}

private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
{
LogCore.LogI(damageInfo.damage);
//onHitEnemy.Invoke(ref damageInfo);
LogCore.LogI(damageInfo.damage);

CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
CharacterMaster attackerMaster = attackerBody ? attackerBody.master : null ;
Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
//Inventory victimInventory = victimMaster ? victimMaster.inventory : null;

if (attackerBody && attackerMaster && attackerInventory)
{

    int scpRandom = UnityEngine.Random.Range(0, scpBuffList.Count);
    int rootCount = attackerInventory.GetItemCount(crippleOnHitIndex);
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
    ProjectileManager.instance.FireProjectile(iinfo);


    if (moneyCount > 0 && Util.CheckRoll(5 * damageInfo.procCoefficient, attackerMaster) && victimBody && !damageInfo.procChainMask.HasProc(ProcType.AACannon)) {
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
takeDamage.Invoke(ref damageInfo, self.gameObject, self);
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
            LogCore.LogF(//Language.GetString(body.baseNameToken) + " doesn't have a team component!");
        }
    }
    timer = 0;
}
}
}
public class MagiciansEarringsBehavior : CharacterBody.ItemBehavior
{
private float timer = 0;
private float oldCount;
private Xoroshiro128Plus rng;

public void Start() {
rng = new Xoroshiro128Plus((ulong)DateTime.UtcNow.Ticks);
}
public void FixedUpdate()
{
if (body)
{
    timer += Time.deltaTime;
    if (timer >= 15) {
        CloudUtils.SafeRemoveBuff(BuffCore.instance.magicArmor, body);
        CloudUtils.SafeRemoveBuff(BuffCore.instance.magicAttackSpeed, body);
        CloudUtils.SafeRemoveBuff(BuffCore.instance.magicRegen, body);

        int count = rng.RangeInt(1, 3);

        //roll me a new number you Worthless Fucking Number Generator 128+
        if (count == oldCount) {
            count = rng.RangeInt(1, 3);
        }
        EffectData effectData = new EffectData
        {
            origin = body.transform.position,
            rotation = Quaternion.identity,
            scale = 20,
        };  
        switch (count) {
            case 1:
                EffectManager.SpawnEffect(EffectCore.magicArmor, effectData, true);
                Util.PlaySound("Play_item_use_gainArmor", base.gameObject);

                body.AddBuff(BuffCore.instance.magicArmor);
                break;
            case 2:
                EffectManager.SpawnEffect(EffectCore.magicAttackSpeed, effectData, true);
                Util.PlaySound("Play_item_proc_crit_attack_speed1", base.gameObject);

                body.AddBuff(BuffCore.instance.magicAttackSpeed);
                break;
            case 3:
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
public class RedactedBehavior : CharacterBody.ItemBehavior
{
private float timer;

private void YOUVEBEENSTRUCKBY(HurtBox box) {
//A HEART ATTACK
if (Util.CheckRoll(30, body.master)) {
    var health = box.healthComponent;
    bool isFullHealth = health.combinedHealthFraction >= 0.8f;
    if (health && !DotController.FindDotController(health.gameObject) && !isFullHealth) {
        EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/HANDheal"), new EffectData() {
            origin = box.transform.position,
            scale = 10,
            color = Color.red,
        }, true); 
        Util.PlaySound("Play_item_proc_armorReduction_shatter", box.gameObject);
        DotController.InflictDot(health.gameObject, body.gameObject, DoTCore.redactedIndex, 8 + (stack *2), 1);
    }
}
}

public void FixedUpdate()
{
timer += Time.deltaTime;
//LogCore.LogI(timer);

if (timer >= 3) {
    BullseyeSearch bullseyeSearch = new BullseyeSearch();
    bullseyeSearch.searchOrigin = transform.position;
    bullseyeSearch.maxDistanceFilter = 120;
    bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(TeamIndex.Player);
    bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
    bullseyeSearch.filterByLoS = true;
    bullseyeSearch.searchDirection = Vector3.up;
    bullseyeSearch.RefreshCandidates();
    bullseyeSearch.FilterOutGameObject(base.gameObject);
    var list = bullseyeSearch.GetResults().ToList();

    for (int i = 0; i < list.Count; i++) {
        YOUVEBEENSTRUCKBY(list[i]);
    }

    /*body.healthComponent.combinedHealthFraction >= 1;
    timer = 0;
}
}
}
}*/
