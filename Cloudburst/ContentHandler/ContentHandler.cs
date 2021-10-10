using EntityStates;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Cloudburst.Content
{

    internal abstract class Module<T> : Module where T : Module<T>
    {
        public static T instance { get; private set; }

        public Module()
        {
            if (instance != null) throw new InvalidOperationException("Singleton public class \"" + typeof(T).Name + "\" was instantiated twice!");
            instance = this as T;
        }
    }

    /// <summary>
    /// Base module.
    /// </summary>
    public abstract class Module
    {
        internal virtual void Load()
        {
            CloudburstPlugin.start += Start;
        }

        internal virtual void Start()
        {
        }
    }

    public class ContentHandler
    {

        internal static ObservableCollection<Module> modules = new ObservableCollection<Module>();

        /// <summary>
        /// Helper class for registering projectiles to the projectile catalog.
        /// </summary>
        public class Projectiles : Module
        {
            internal static ObservableCollection<GameObject> ProjectileDefinitions = new ObservableCollection<GameObject>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
            }

            /// <summary>
            /// Adds a GameObject to the projectile catalog.
            /// GameObject cannot be null and must have a ProjectileController component.
            /// </summary>
            /// <param name="projectile">The projectile to register to the projectile catalog.</param>
            /// <returns></returns>
            public static void RegisterProjectile(GameObject projectile)
            {
                //Check if the SurvivorDef has already been registered.
                if (ProjectileDefinitions.Contains(projectile) || !projectile || !projectile.GetComponent<ProjectileController>())
                {
                    string error = projectile + " has already been registered, please do not register the same projectile twice.";
                    if (!projectile.GetComponent<ProjectileController>())
                    {
                        error += " And/Or, the projectile does not have a projectile controller component.";
                    }
                    LogCore.LogE(error);
                }
                //If not, add it to our SurvivorDefinitions
                ProjectileDefinitions.Add(projectile);
            }

            internal static GameObject[] DumpContent()
            {
                //Make a list of survivor defs (we'll be converting it to an array later)
                List<GameObject> defs = new List<GameObject>();
                //Add everything from SurvivorDefinitions to it.
                foreach (GameObject def in ProjectileDefinitions)
                {
                    defs.Add(def);
                }
                //Convert the list into an array and give it to the ContentPack.
                return defs.ToArray();
            }
        }
        /// <summary>
        /// Helper class for registering masters to the MasterCatalog
        /// </summary>
        public class Masters : Module
        {
            internal static ObservableCollection<GameObject> MasterDefinitions = new ObservableCollection<GameObject>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
            }

            /// <summary>
            /// Adds a GameObject to the master catalog.
            /// GameObject cannot be null and have a CharacterMaster component.
            /// </summary>
            /// <param name="master">The master GameObject to register to the master catalog.</param>
            /// <returns></returns>
            public static void RegisterMaster(GameObject master)
            {
                //Check if the SurvivorDef has already been registered.
                if (MasterDefinitions.Contains(master) || !master.GetComponent<CharacterMaster>())
                {
                    string error = master + " has already been registered, please do not register the same master twice.";
                    if (!master.GetComponent<CharacterMaster>())
                    {
                        error += " And/Or, the master does not have a character master component.";
                    }
                    LogCore.LogE(error);
                }
                //If not, add it to our SurvivorDefinitions
                MasterDefinitions.Add(master);
            }

            internal static GameObject[] DumpContent()
            {


                //Make a list of survivor defs (we'll be converting it to an array later)
                List<GameObject> defs = new List<GameObject>();
                //Add everything from SurvivorDefinitions to it.
                foreach (GameObject def in MasterDefinitions)
                {
                    defs.Add(def);
                }
                //Convert the list into an array and give it to the ContentPack.
                return defs.ToArray();
            }
        }
        /// <summary>
        /// Helper class for adding entity states, skill families, skill defs, survivor defs, skins, and entity state configurations.
        /// </summary>
        public class Loadouts : Module
        {
            internal static ObservableCollection<SkillFamily> SkillFamilyDefinitions = new ObservableCollection<SkillFamily>();
            internal static ObservableCollection<Type> EntityStateDefinitions = new ObservableCollection<Type>();
            internal static ObservableCollection<SkillDef> SkillDefDefinitions = new ObservableCollection<SkillDef>();
            internal static ObservableCollection<SurvivorDef> SurvivorDefinitions = new ObservableCollection<SurvivorDef>();
            internal static ObservableCollection<EntityStateConfiguration> EntityStateConfigurationDefinitions = new ObservableCollection<EntityStateConfiguration>();
            internal static readonly HashSet<SkinDef> AddedSkins = new HashSet<SkinDef>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
            }




            public static bool RegisterEntityStateConfig(EntityStateConfiguration entityStateConfiguration)
            {
                //Check if the EntityStateConfiguration has already been registered.
                if (EntityStateConfigurationDefinitions.Contains(entityStateConfiguration))
                {
                    string error = entityStateConfiguration.name + " has already been registered, please do not register the same EntityStateConfiguration twice.";
                    if (entityStateConfiguration.targetType == default)
                    {
                        error = error + " And/Or, the target type has not been set. Please make sure your target has been set before creating your EntityStateConfiguration.";
                    }
                    LogCore.LogE(error);
                    return false;
                }
                //If not, add it to our EntityStateConfigurationDefinitions
                EntityStateConfigurationDefinitions.Add(entityStateConfiguration);
                return true;
            }

            /// <summary>
            /// Registers a SkillFamily to the SkillCatalog.
            /// Must be called before Catalog init (during Awake() or OnEnable())
            /// </summary>
            /// <param name="skillFamily">The SkillDef to add</param>
            /// <returns>True if the event was registered</returns>
            public static bool RegisterSkillFamily(SkillFamily skillFamily)
            {
                //Check if the SurvivorDef has already been registered.
                if (SkillFamilyDefinitions.Contains(skillFamily))
                {
                    LogCore.LogE(skillFamily + " has already been registered to the SkillFamily Catalog, please do not register the same SkillFamily twice.");
                    return false;
                }
                //If not, add it to our SurvivorDefinitions
                SkillFamilyDefinitions.Add(skillFamily);
                return true;
            }

            /// <summary>
            /// Adds the type of an EntityState to the EntityStateCatalog.
            /// State must derive from EntityStates.EntityState.
            /// Note that SkillDefs and SkillFamiles must also be added seperately.
            /// </summary>
            /// <param name="entityState">The type to add</param>
            /// <returns>True if succesfully added</returns>
            public static bool RegisterEntityState(Type entityState)
            {
                //Check if the entity state has already been registered, is abstract, or is not a subclass of the base EntityState
                if (EntityStateDefinitions.Contains(entityState) || !entityState.IsSubclassOf(typeof(EntityState)) || entityState.IsAbstract)
                {
                    LogCore.LogE(entityState.AssemblyQualifiedName + " is either abstract, not a subclass of an entity state, or has already been registered.");
                    LogCore.LogI("Is Abstract: " + entityState.IsAbstract + " Is not Subclass: " + !entityState.IsSubclassOf(typeof(EntityState)) + " Is already added: " + EntityStateDefinitions.Contains(entityState));
                    return false;
                }
                //If not, add it to our EntityStateDefinitions
                EntityStateDefinitions.Add(entityState);
                return true;
            }

            /// <summary>
            /// Registers a SkillDef to the SkillCatalog.
            /// Must be called before Catalog init (during Awake() or OnEnable())
            /// </summary>
            /// <param name="skillDef">The SkillDef to add</param>
            /// <returns>True if the event was registered</returns>
            public static bool RegisterSkillDef(SkillDef skillDef)
            {
                //Check if the SurvivorDef has already been registered.
                if (SkillDefDefinitions.Contains(skillDef))
                {
                    LogCore.LogE(skillDef + " has already been registered to the SkillDef Catalog, please do not register the same SkillDef twice.");
                    return false;
                }
                //If not, add it to our SurvivorDefinitions
                SkillDefDefinitions.Add(skillDef);
                return true;
            }

            /// <summary>
            /// Add a SurvivorDef to the list of available survivors.
            /// This must be called before the SurvivorCatalog inits, so before plugin.Start().
            /// If this is called after the SurvivorCatalog inits then this will return false and ignore the survivor.        /// The survivor prefab must be non-null
            /// </summary>
            /// <param name="survivorDef">The survivor to add.</param>
            /// <returns>true if survivor will be added</returns>
            public static bool RegisterSurvivorDef(SurvivorDef survivorDef)
            {
                //Check if the SurvivorDef has already been registered.
                if (SurvivorDefinitions.Contains(survivorDef) || !survivorDef.bodyPrefab)
                {
                    string error = Language.GetString(survivorDef.displayNameToken) + " has already been registered, please do not register the same SurvivorDef twice.";
                    if (!survivorDef.bodyPrefab)
                    {
                        error = error + " And/Or, the body prefab is null. Please make sure your body prefab is not null before creating your SurvivorDef.";
                    }
                    LogCore.LogE(error);

                    return false;
                }
                //If not, add it to our SurvivorDefinitions
                SurvivorDefinitions.Add(survivorDef);
                return true;
            }
            internal static SkillFamily[] DumpContentSkillFamilies()
            {

                List<SkillFamily> skillFamilies = new List<SkillFamily>();
                //Add everything from SkillFamilyDefinitions to it.
                foreach (SkillFamily def in SkillFamilyDefinitions)
                {
                    skillFamilies.Add(def);
                }
                return skillFamilies.ToArray();
            }

            internal static Type[] DumpEntityStates()
            {
                List<Type> entityStates = new List<Type>();
                //Add everything from EntityStateDefinitions to it.
                foreach (Type def in EntityStateDefinitions)
                {
                    entityStates.Add(def);
                }
                return entityStates.ToArray();
            }

            internal static SurvivorDef[] DumpSurvivorDefs()
            {

                List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
                //Add everything from SurvivorDefinitions to it.
                foreach (SurvivorDef def in SurvivorDefinitions)
                {
                    survivorDefs.Add(def);
                }
                return survivorDefs.ToArray();
            }

            internal static EntityStateConfiguration[] DumpConfigs()
            {

                List<EntityStateConfiguration> entityStateConfigs = new List<EntityStateConfiguration>();
                //Add everything from SurvivorDefinitions to it.
                foreach (EntityStateConfiguration def in EntityStateConfigurationDefinitions)
                {
                    entityStateConfigs.Add(def);
                }
                return entityStateConfigs.ToArray();
            }

            internal static SkillDef[] DumpContentSkillDefs()
            {

                //Make a lists full of added content

                List<SkillDef> skillDefs = new List<SkillDef>();
                //Add everything from SkillDefDefinitions to it.
                foreach (SkillDef def in SkillDefDefinitions)
                {
                    skillDefs.Add(def);
                }
                return skillDefs.ToArray();
            }
            #region Adding Skins
            /// <summary>
            /// Creates a skin icon sprite styled after the ones already in the game.
            /// </summary>
            /// <param name="top">The color of the top portion</param>
            /// <param name="right">The color of the right portion</param>
            /// <param name="bottom">The color of the bottom portion</param>
            /// <param name="left">The color of the left portion</param>
            /// <returns>The icon sprite</returns>
            public static Sprite CreateSkinIcon(Color top, Color right, Color bottom, Color left)
            {
                return CreateSkinIcon(top, right, bottom, left, new Color(0.6f, 0.6f, 0.6f));
            }
            /// <summary>
            /// Creates a skin icon sprite styled after the ones already in the game.
            /// </summary>
            /// <param name="top">The color of the top portion</param>
            /// <param name="right">The color of the right portion</param>
            /// <param name="bottom">The color of the bottom portion</param>
            /// <param name="left">The color of the left portion</param>
            /// <param name="line">The color of the dividing lines</param>
            /// <returns></returns>
            public static Sprite CreateSkinIcon(Color top, Color right, Color bottom, Color left, Color line)
            {
                var tex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                new IconTexJob
                {
                    Top = top,
                    Bottom = bottom,
                    Right = right,
                    Left = left,
                    Line = line,
                    TexOutput = tex.GetRawTextureData<Color32>()
                }.Schedule(16384, 1).Complete();
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            }
            private struct IconTexJob : IJobParallelFor
            {
                [ReadOnly]
                public Color32 Top;
                [ReadOnly]
                public Color32 Right;
                [ReadOnly]
                public Color32 Bottom;
                [ReadOnly]
                public Color32 Left;
                [ReadOnly]
                public Color32 Line;
                public NativeArray<Color32> TexOutput;
                public void Execute(int index)
                {
                    int x = index % 128 - 64;
                    int y = index / 128 - 64;

                    if (Math.Abs(Math.Abs(y) - Math.Abs(x)) <= 2)
                    {
                        TexOutput[index] = Line;
                        return;
                    }
                    if (y > x && y > -x)
                    {
                        TexOutput[index] = Top;
                        return;
                    }
                    if (y < x && y < -x)
                    {
                        TexOutput[index] = Bottom;
                        return;
                    }
                    if (y > x && y < -x)
                    {
                        TexOutput[index] = Left;
                        return;
                    }
                    if (y < x && y > -x)
                    {
                        TexOutput[index] = Right;
                    }
                }
            }

            /// <summary>
            /// A container struct for all SkinDef parameters.
            /// Use this to set skinDef values, then call CreateNewSkinDef().
            /// </summary>
            public struct SkinDefInfo
            {
#pragma warning disable
                public SkinDef[] BaseSkins;
                public Sprite Icon;
                public string NameToken;

                [Obsolete("Use SkinDefInfo.UnlockableDef instead")]
                public string UnlockableName;

                public UnlockableDef UnlockableDef;

                public GameObject RootObject;
                public CharacterModel.RendererInfo[] RendererInfos;
                public SkinDef.MeshReplacement[] MeshReplacements;
                public SkinDef.GameObjectActivation[] GameObjectActivations;
                public SkinDef.ProjectileGhostReplacement[] ProjectileGhostReplacements;
                public SkinDef.MinionSkinReplacement[] MinionSkinReplacements;
                public string Name;
            }

            /// <summary>
            /// Creates a new SkinDef from a SkinDefInfo.
            /// Note that this prevents null-refs by disabling SkinDef awake while the SkinDef is being created.
            /// The things that occur during awake are performed when first applied to a character instead.
            /// </summary>
            /// <param name="skin"></param>
            /// <returns></returns>
            public static SkinDef CreateNewSkinDef(SkinDefInfo skin)
            {
                On.RoR2.SkinDef.Awake += DoNothing;

                var newSkin = ScriptableObject.CreateInstance<SkinDef>();

                newSkin.baseSkins = skin.BaseSkins;
                newSkin.icon = skin.Icon;
                newSkin.unlockableDef = skin.UnlockableDef;
                newSkin.unlockableName = skin.UnlockableName;

                newSkin.rootObject = skin.RootObject;
                newSkin.rendererInfos = skin.RendererInfos;
                newSkin.gameObjectActivations = skin.GameObjectActivations;
                newSkin.meshReplacements = skin.MeshReplacements;
                newSkin.projectileGhostReplacements = skin.ProjectileGhostReplacements;
                newSkin.minionSkinReplacements = skin.MinionSkinReplacements;
                newSkin.nameToken = skin.NameToken;
                newSkin.name = skin.Name;

                On.RoR2.SkinDef.Awake -= DoNothing;

                AddedSkins.Add(newSkin);
                return newSkin;
            }

            /// <summary>
            /// Adds a skin to the body prefab for a character.
            /// Will attempt to create a default skin if one is not present.
            /// Must be called during plugin Awake or OnEnable. If called afterwards the new skins must be added to bodycatalog manually.
            /// </summary>
            /// <param name="bodyPrefab">The body to add the skin to</param>
            /// <param name="skin">The SkinDefInfo for the skin to add</param>
            /// <returns>True if successful</returns>
            public static bool AddSkinToCharacter(GameObject bodyPrefab, SkinDefInfo skin)
            {
                var skinDef = CreateNewSkinDef(skin);
                return AddSkinToCharacter(bodyPrefab, skinDef);
            }

            /// <summary>
            /// Adds a skin to the body prefab for a character.
            /// Will attempt to create a default skin if one is not present.
            /// Must be called during plugin Awake or OnEnable. If called afterwards the new skins must be added to bodycatalog manually.
            /// </summary>
            /// <param name="bodyPrefab">The body to add the skin to</param>
            /// <param name="skin">The SkinDef to add</param>
            /// <returns>True if successful</returns>
            public static bool AddSkinToCharacter(GameObject bodyPrefab, SkinDef skin)
            {
                if (bodyPrefab == null)
                {
                    LogCore.LogE("Tried to add skin to null body prefab.");
                    return false;
                }

                if (skin == null)
                {
                    LogCore.LogE("Tried to add invalid skin.");
                    return false;
                }
                AddedSkins.Add(skin);

                var modelLocator = bodyPrefab.GetComponent<ModelLocator>();
                if (modelLocator == null)
                {
                    LogCore.LogE("Tried to add skin to invalid body prefab (No ModelLocator).");
                    return false;
                }

                var model = modelLocator.modelTransform;
                if (model == null)
                {
                    LogCore.LogE("Tried to add skin to body prefab with no modelTransform.");
                    return false;
                }

                if (skin.rootObject != model.gameObject)
                {
                    LogCore.LogE("Tried to add skin with improper root object set.");
                    return false;
                }

                var modelSkins = model.GetComponent<ModelSkinController>();

                if (modelSkins == null)
                {
                    LogCore.LogW(bodyPrefab.name + " does not have a modelSkinController.\nAdding a new one and attempting to populate the default skin.\nHighly recommended you set the controller up manually.");
                    var charModel = model.GetComponent<CharacterModel>();
                    if (charModel == null)
                    {
                        LogCore.LogE("Unable to locate CharacterModel, default skin creation aborted.");
                        return false;
                    }

                    var skinnedRenderer = charModel.GetFieldValue<SkinnedMeshRenderer>("mainSkinnedMeshRenderer");
                    if (skinnedRenderer == null)
                    {
                        LogCore.LogE("CharacterModel did not contain a main SkinnedMeshRenderer, default skin creation aborted.");
                        return false;
                    }

                    var baseRenderInfos = charModel.baseRendererInfos;
                    if (baseRenderInfos == null || baseRenderInfos.Length == 0)
                    {
                        LogCore.LogE("CharacterModel rendererInfos are invalid, default skin creation aborted.");
                        return false;
                    }

                    modelSkins = model.gameObject.AddComponent<ModelSkinController>();

                    var skinDefInfo = new SkinDefInfo
                    {
                        BaseSkins = Array.Empty<SkinDef>(),
                        GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>(),
                        Icon = CreateDefaultSkinIcon(),
                        Name = "skin" + bodyPrefab.name + "Default",
                        NameToken = bodyPrefab.name.ToUpper() + "_DEFAULT_SKIN_NAME",
                        RootObject = model.gameObject,
                        UnlockableName = "",
                        UnlockableDef = null,
                        MeshReplacements = new[]
                        {
                        new SkinDef.MeshReplacement {
                            renderer = skinnedRenderer,
                            mesh = skinnedRenderer.sharedMesh
                        }
                    },
                        RendererInfos = charModel.baseRendererInfos,
                        ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>(),
                        MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>()
                    };

                    var defaultSkinDef = CreateNewSkinDef(skinDefInfo);

                    modelSkins.skins = new[] {
                    defaultSkinDef
                };
                }

                //Array.Resize(ref modelSkins.skins, modelSkins.skins.Length + 1);
                //modelSkins.skins[modelSkins.skins.Length - 1] = skin;

                Array.Resize(ref modelSkins.skins, modelSkins.skins.Length + 1);
                modelSkins.skins[modelSkins.skins.Length - 1] = skin;
                return true;
            }

            private static Sprite CreateDefaultSkinIcon()
            {
                return CreateSkinIcon(Color.red, Color.green, Color.blue, Color.black);
            }

            private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, SkinDef self)
            {
                //Intentionally do nothing
            }
            #endregion
        }
        /// <summary>
        /// Helper class for registering EffectDefs to the EffectCatalog
        /// </summary>
        public class Effects : Module
        {
            internal static ObservableCollection<EffectDef> EffectDefDefinitions = new ObservableCollection<EffectDef>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
            }

            /// <summary>
            /// Creates an EffectDef from a prefab and adds it to the EffectCatalog.
            /// The prefab must have an the following components: EffectComponent, VFXAttributes
            /// For more control over the EffectDef, use RegisterEffect(EffectDef)
            /// </summary>
            /// <param name="effect">The prefab of the effect to be added</param>
            public static void RegisterGenericEffect(GameObject effect)
            {
                if (!effect)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" is null", effect.name));


                }

                var effectComp = effect.GetComponent<EffectComponent>();
                if (effectComp == null)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" does not have an EffectComponent.", effect.name));

                }

                var vfxAttrib = effect.GetComponent<VFXAttributes>();
                if (vfxAttrib == null)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" does not have a VFXAttributes component.", effect.name));

                }

                var def = new EffectDef
                {
                    prefab = effect,
                    //cullMethod = new Func<EffectData, bool>()
                };
                RegisterEffect(def);
            }

            /// <summary>
            /// Creates an EffectDef from a prefab.
            /// The prefab must have an the following components: EffectComponent, VFXAttributes
            /// </summary>
            /// <param name="effect">The prefab of the effect to be added</param>
            /// <returns>The newly created EffectDef</returns>
            public static EffectDef CreateGenericEffectDef(GameObject effect)
            {
                if (!effect)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" is null", effect.name));

                    return null;
                }

                var effectComp = effect.GetComponent<EffectComponent>();
                if (effectComp == null)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" does not have an EffectComponent.", effect.name));
                    return null;
                }

                var vfxAttrib = effect.GetComponent<VFXAttributes>();
                if (vfxAttrib == null)
                {
                    LogCore.LogE(string.Format("Effect prefab: \"{0}\" does not have a VFXAttributes component.", effect.name));
                    return null;
                }

                var def = new EffectDef
                {
                    prefab = effect,
                    //cullMethod = new Func<EffectData, bool>()
                };
                return def;
            }


            /// <summary>
            /// Adds an EffectDef to the EffectCatalog.
            /// </summary>
            /// <param name="effectDef">The EffectDef to add</param>
            public static void RegisterEffect(EffectDef effectDef)
            {
                //Check if the SurvivorDef has already been registered.
                if (EffectDefDefinitions.Contains(effectDef) || !effectDef.prefab)
                {
                    LogCore.LogE(effectDef + " has already been registered to the EffectDef Catalog, please do not register the same EffectDef twice. Or, the EffectDef does not have a prefab.");
                    return;
                }
                //If not, add it to our SurvivorDefinitions
                EffectDefDefinitions.Add(effectDef);
            }

            internal static EffectDef[] DumpContent()
            {

                //Make a list of survivor defs (we'll be converting it to an array later)
                List<EffectDef> defs = new List<EffectDef>();
                //Add everything from SurvivorDefinitions to it.
                foreach (EffectDef def in EffectDefDefinitions)
                {
                    defs.Add(def);
                }
                //Convert the list into an array and give it to the ContentPack.
                return defs.ToArray();
            }
        }
        /// <summary>
        /// Helper class for adding custom buffs to the game. 
        /// </summary>
        public class Buffs : Module
        {
            internal static ObservableCollection<BuffDef> BuffDefDefinitions = new ObservableCollection<BuffDef>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
                //IL.RoR2.BuffCatalog.Init += FixBuffCatalog;
            }

            /*Credits to Aaron on the RoR2 modding discord.
            internal static void FixBuffCatalog(ILContext il)
            {
                ILCursor c = new ILCursor(il);

                if (!c.Next.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.buffDefs)))
                {
                    LogCore.LogW("Another mod has already fixed BuffCatalog or the game has updated, skipping...");
                    return;
                }

                c.Remove();
                c.Emit(OpCodes.Ldsfld, typeof(ContentManager).GetField(nameof(ContentManager.buffDefs)));
            }*/

            /// <summary>
            /// Registers a buff def to the buff catalog
            /// </summary>
            /// <param name="BuffDef">The buff def you want to register.</param>
            public static void RegisterBuff(BuffDef BuffDef)
            {
                //Check if the SurvivorDef has already been registered.
                if (BuffDefDefinitions.Contains(BuffDef))
                {
                    LogCore.LogE(BuffDef.name + " has already been registered, please do not register the same BuffDef twice.");
                    return;
                }
                //If not, add it to our SurvivorDefinitions
                BuffDefDefinitions.Add(BuffDef);
            }

            internal static BuffDef[] DumpContent()
            {

                //Make a list of survivor defs (we'll be converting it to an array later)
                List<BuffDef> defs = new List<BuffDef>();
                //Add everything from SurvivorDefinitions to it.
                foreach (BuffDef def in BuffDefDefinitions)
                {
                    defs.Add(def);
                }
                //Convert the list into an array and give it to the ContentPack.
                return defs.ToArray();
            }
        }
        /// <summary>
        /// Helper class for registering bodies to the BodyCatalog.
        /// </summary>
        public class Bodies : Module
        {
            internal static ObservableCollection<GameObject> BodyDefinitions = new ObservableCollection<GameObject>();
            internal override void Load()
            {
                base.Load();
                //Meow (Waiting for something to happen?)
            }


            /// <summary>
            /// Registers a body prefab to the BodyCatalog
            /// </summary>
            /// <param name="body">The body prefab to register.</param>
            public static void RegisterBody(GameObject body)
            {
                //Check if the SurvivorDef has already been registered.
                if (BodyDefinitions.Contains(body) || !body)
                {
                    LogCore.LogE(body + " has already been registered, please do not register the same body prefab twice. Or, the body prefab is null.");
                    return;
                }
                //If not, add it to our SurvivorDefinitions
                BodyDefinitions.Add(body);
            }

            internal static GameObject[] DumpContent()
            {


                //Make a list of survivor defs (we'll be converting it to an array later)
                List<GameObject> defs = new List<GameObject>();
                //Add everything from SurvivorDefinitions to it.
                foreach (GameObject def in BodyDefinitions)
                {
                    defs.Add(def);
                }
                //Convert the list into an array and give it to the ContentPack.
                return defs.ToArray();
            }
        }
        public static void Load()
        {
            List<Type> gatheredModules = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Module))).ToList();
            foreach (Type module in gatheredModules)
            {
                //Create instance.
                Module item = (Module)Activator.CreateInstance(module);
                //Log
                LogCore.LogI("Enabling module: " + item);
                //Fire
                item.Load();
                //Add to collection
                modules.Add(item);

            }
            CloudburstPlugin.postLoad += AddContentPack;
        }

        private static void AddContentPack()
        {
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }
        private static void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new CloudburstContent());
        }
    }

    class CloudburstContent : IContentPackProvider
    {

        private ContentPack contentPack = new ContentPack();
        public string identifier => "Cloudburst.CloudburstContent";

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            //we do a large amount of trolling
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            //we do a CONSIDERABLE amount of trolling.
            contentPack.bodyPrefabs.Add(Cloudburst.Content.ContentHandler.Bodies.DumpContent());
            //args.ReportProgress(1f);
            contentPack.buffDefs.Add(Cloudburst.Content.ContentHandler.Buffs.DumpContent());
            //args.ReportProgress(1f);
            contentPack.effectDefs.Add(Cloudburst.Content.ContentHandler.Effects.DumpContent());
            contentPack.entityStateConfigurations.Add(Cloudburst.Content.ContentHandler.Loadouts.DumpConfigs());
            //args.ReportProgress(1f);
            contentPack.entityStateTypes.Add(Cloudburst.Content.ContentHandler.Loadouts.DumpEntityStates());
            //args.ReportProgress(1f);
            contentPack.skillFamilies.Add(Cloudburst.Content.ContentHandler.Loadouts.DumpContentSkillFamilies());
            //args.ReportProgress(1f);
            contentPack.skillDefs.Add(Cloudburst.Content.ContentHandler.Loadouts.DumpContentSkillDefs());
            //args.ReportProgress(1f);
            contentPack.survivorDefs.Add(Cloudburst.Content.ContentHandler.Loadouts.DumpSurvivorDefs());
            //args.ReportProgress(1f);
            contentPack.masterPrefabs.Add(Cloudburst.Content.ContentHandler.Masters.DumpContent());
            contentPack.projectilePrefabs.Add(Cloudburst.Content.ContentHandler.Projectiles.DumpContent());
            args.ReportProgress(1f);

            yield break;
        }
    }
}
