using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SurvivorCreator<T> : SurvivorCreator where T : SurvivorCreator<T>
{
    public static T instance { get; private set; }

    public SurvivorCreator()
    {
        if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" was instantiated twice!");
        instance = this as T;
    }
}
public abstract class SurvivorCreator
{
    /// <summary>
    /// The outro line of the survivor
    /// </summary>
    public abstract string SurvivorOutro { get; }
    /// <summary>
    /// The lore of the survivor
    /// </summary>
    public abstract string SurvivorLore { get; }
    /// <summary>
    /// The survivor's name
    /// </summary>
    public abstract string SurvivorName { get; }
    /// <summary>
    /// The survivor's description that shows up in the survivor menu
    /// </summary>
    public abstract string SurvivorDescription { get; }

    /// <summary>
    /// The survivor's name that's used in language tokens.
    /// </summary>
    public abstract string SurvivorInternalName { get; }
    /// <summary>
    /// The survivor's internal name.
    /// </summary>
    public abstract string BodyName { get; }
    /// <summary>
    /// The survivor's unlock string.
    /// </summary>
    public abstract string UnlockableString { get; }
    /// <summary>
    /// The survivor's mastery unlock string.
    /// </summary>
    public abstract string MasteryUnlockString { get; }
    /// <summary>
    /// The survivor's subtitle.
    /// </summary>
    public abstract string SurvivorSubtitle { get; }
    /// <summary>
    /// The display prefab of the survivor
    /// </summary>
    public abstract GameObject survivorDisplay { get; }
    public abstract GameObject survivorMdl { get; }

    public abstract Color survivorDefColor { get; }
    public abstract Sprite defaultSkinColor { get; }
    public abstract Sprite masterySkinColor { get; }


    public GameObject umbraMaster;
    public SurvivorDef def;
    public PrefabBuilder builder;
    public GameObject survivorUmbra;
    public GameObject survivorBody;

    protected abstract void Initialization();

    /// <summary>
    /// Only override when you know what you are doing, or call base.Init()!
    /// </summary>
    /// <param name="config"></param>
    internal virtual void Init(ConfigFile file)
    {
        CreatePrefab();
        Initialization();
        CreateLang();
        SetupCharacterBody(survivorBody.GetComponent<CharacterBody>());
        AlterStatemachines(survivorBody.GetComponent<SetStateOnHurt>(), survivorBody.GetComponent<NetworkStateMachine>());
        CreateSkills();
        GenerateUmbra();
        Hooks();
        CreateSurvivorDef();
    }

    public virtual void GenerateUmbra() {

    }

    public virtual void AlterStatemachines(SetStateOnHurt hurt, NetworkStateMachine network) {

    }

    public virtual void SetupCharacterBody(CharacterBody characterBody) { }

    public abstract Material GetMasteryMat();
    #region Skills 
    public virtual void CreateSkills()
    {
        CloudUtils.CreateEmptySkills(survivorBody);
        SkillLocator locator = survivorBody.GetComponent<SkillLocator>();
        CreateMainState(survivorBody.GetComponent<EntityStateMachine>());
        CreatePassive(locator);
        CreatePrimary(locator, locator.primary.skillFamily);
        CreateSecondary(locator, locator.secondary.skillFamily);
        CreateUtility(locator, locator.utility.skillFamily);
        CreateSpecial(locator, locator.special.skillFamily);
    }

    public virtual void CreateMainState(EntityStateMachine machine)
    {

    }

    public virtual void CreatePassive(SkillLocator locator)
    {

    }

    public virtual void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    public virtual void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
    {

    }
    #endregion
    #region PrefabBuilder
    protected void CreatePrefab()
    {
        PrefabBuilder builder = new PrefabBuilder();
        builder.prefabName = BodyName + "Body";
        builder.masteryAchievementUnlockable = MasteryUnlockString;
        builder.model = survivorMdl;
        builder.defaultSkinIcon = defaultSkinColor;
        builder.masterySkinIcon = masterySkinColor;
        builder.masterySkinDelegate = GetMasteryMat;
        builder.GetAdditionalRenderInfos += GenerateRenderInfos;
        builder.GetAdditionalItemDisplays += GenerateItemDisplays;
        builder.GetAdditionalEquipmentDisplays += GenerateEquipmentDisplays; ;

        survivorBody = builder.CreatePrefab();
    }
    public virtual void GenerateEquipmentDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
    {
    }
    public virtual void GenerateItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> obj)
    {
    }
    public virtual void GenerateRenderInfos(List<CharacterModel.RendererInfo> arg1, Transform arg2)
    {

    }
    #endregion

    protected virtual void CreateLang()
    {
        LanguageAPI.Add(SurvivorInternalName + "_BODY_NAME", SurvivorName);
        LanguageAPI.Add(SurvivorInternalName + "_DESCRIPTION", SurvivorDescription);
        LanguageAPI.Add(SurvivorInternalName + "_OUTRO_FLAVOR", SurvivorOutro);
        LanguageAPI.Add(SurvivorInternalName + "_BODY_LORE", SurvivorLore);
        LanguageAPI.Add(SurvivorInternalName + "_BODY_SUBTITLE", SurvivorSubtitle);
    }

    protected void CreateSurvivorDef()
    {
        SurvivorDef def = new SurvivorDef()
        {
            bodyPrefab = survivorBody,
            descriptionToken = SurvivorInternalName + "_DESCRIPTION",
            displayNameToken = SurvivorInternalName + "_BODY_NAME",
            displayPrefab = survivorDisplay,
            outroFlavorToken = SurvivorInternalName + "_OUTRO_FLAVOR",
            primaryColor = survivorDefColor,
            unlockableName = UnlockableString,
        };
        SurvivorAPI.AddSurvivor(def);
    }

    public virtual void Hooks() { }
}
