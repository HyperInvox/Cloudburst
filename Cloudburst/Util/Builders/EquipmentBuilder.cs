﻿using BepInEx.Configuration;
using Cloudburst.Cores;
using R2API;
using RoR2;
using UnityEngine;

public abstract class EquipmentBuilder
    {
        public abstract string EquipmentName { get; }
        public abstract string EquipmentLangTokenName { get; }
        public abstract string EquipmentPickupDesc { get; }
        public abstract string EquipmentFullDescription { get; }
        public abstract string EquipmentLore { get; }

        public abstract string EquipmentModelPath { get; }
        public abstract string EquipmentIconPath { get; }

        public virtual bool AppearsInSinglePlayer { get; } = true;

        public virtual bool AppearsInMultiPlayer { get; } = true;

        public virtual bool CanDrop { get; } = true;

        public virtual float Cooldown { get; } = 60f;

        public virtual bool EnigmaCompatible { get; } = true;

        public virtual bool IsBoss { get; } = false;

        public virtual bool IsLunar { get; } = false;

        public EquipmentDef Index;

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        protected abstract void Initialization();

        /// <summary>
        /// Take care to call base.Init()!
        /// </summary>
        public virtual void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Initialization();
            Hooks();
        }

        protected virtual void CreateConfig(ConfigFile config) { }


        /// <summary>
        /// Take care to call base.CreateLang()!
        /// </summary>
        protected virtual void CreateLang()
        {
            R2API.LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_NAME", EquipmentName);
            R2API.LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_PICKUP", EquipmentPickupDesc);
            R2API.LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_DESCRIPTION", EquipmentFullDescription);
            R2API.LanguageAPI.Add("EQUIPMENT_" + EquipmentLangTokenName + "_LORE", EquipmentLore);
        }

        protected void CreateEquipment()
        {
        Index = ScriptableObject.CreateInstance<EquipmentDef>();

        Index.name = "EQUIPMENT_" + EquipmentLangTokenName;
        Index.nameToken = "EQUIPMENT_" + EquipmentLangTokenName + "_NAME";
        Index.pickupToken = "EQUIPMENT_" + EquipmentLangTokenName + "_PICKUP";
        Index.descriptionToken = "EQUIPMENT_" + EquipmentLangTokenName + "_DESCRIPTION";
        Index.loreToken = "EQUIPMENT_" + EquipmentLangTokenName + "_LORE";
        Index.pickupModelPrefab = AssetsCore.mainAssetBundle.LoadAsset<GameObject>(EquipmentModelPath);
        Index.pickupIconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>(EquipmentIconPath);
        Index.appearsInSinglePlayer = AppearsInSinglePlayer;
        Index.appearsInMultiPlayer = AppearsInMultiPlayer;
        Index.canDrop = CanDrop;
        Index.cooldown = Cooldown;
        Index.enigmaCompatible = EnigmaCompatible;
        Index.isBoss = IsBoss;
        Index.isLunar = IsLunar;
        var itemDisplayRules = CreateItemDisplayRules();
        R2API.ItemAPI.Add(new R2API.CustomEquipment(Index, itemDisplayRules));

        //EnigmaticThunder.Modules.Pickups.RegisterEquipment(Index);
        On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction; ;
    }

    private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
    {
        if (equipmentDef == Index)
        {
            return ActivateEquipment(self);
        }
        else
        {
            return orig(self, equipmentDef);
        }
    }
        protected abstract bool ActivateEquipment(EquipmentSlot slot);

        public virtual void Hooks() { }
    }
