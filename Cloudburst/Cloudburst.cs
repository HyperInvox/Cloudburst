using BepInEx;
using BepInEx.Configuration;
using Cloudburst.Cores;
using Cloudburst.Cores.Components;
using Cloudburst.Cores.HAND;
using EnigmaticThunder;
using EnigmaticThunder.Modules;
using RoR2;
using RoR2.Audio;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cloudburst
{
    [BepInDependency(EnigmaticThunderPlugin.guid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(guid, modName, version)]

    public class CloudburstPlugin : BaseUnityPlugin
    {
        //be unique, though you're the same here.

        public const string guid = "com.TeamCloudburst.Cloudburst";
        public const string modName = "Cloudburst";
        public const string version = "0.1.8    ";

        public static CloudburstPlugin instance;

        /// <summary>
        /// Called after the cores are loaded. Do not reboot a core or call its constructor.
        /// </summary>
        public static event Action postLoad;

        /// <summary>
        /// Called BEFORE the first frame of the game.
        /// </summary>
        public static event Action awake;
        /// <summary>
        /// Called on the first frame of the game.
        /// </summary>
        public static event Action start;
        /// <summary>
        /// Called after the first frame of the game.
        /// </summary>
        public static event Action postStart;
        /// <summary>
        /// Called when the mod is enabled
        /// </summary>
        public static event Action onEnable;
        /// <summary>
        /// Called when the mod is disabled
        /// </summary>
        public static event Action onDisable;
        /// <summary>
        /// Called on the mod's FixedUpdate
        /// </summary>
        public static event Action onFixedUpdate;

        public ConfigFile GetConfig()
        {
            return Config;
        }
        public bool ValidateItem(ItemBuilder item, List<ItemBuilder> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        public bool ValidateEquipment(EquipmentBuilder equipment, List<EquipmentBuilder> equipmentList)
        {
            if (Config.Bind<bool>("Equipment: " + equipment.EquipmentName, "Enable Equipment?", true, "Should this equipment appear in runs?").Value)
            {
                equipmentList.Add(equipment);
                return true;
            }
            return false;
        }


        public ConfigFile configFile
        {
            get {
                return this.Config;
            }
            private set { }
        }

        #region Cores
        //Support cores; Cannot be disabled.
        private AssetsCore assetCore;
        private OrbCore orbCore;
        private BuffCore buffCore;
        private DoTCore dotCore;
        private EffectCore effectCore;
        //private DebuggingCore debugCore;
        private PrefabCore prefabCore;
        //private ShitpostCore lolCore;

        //Content Cores; Can be disabled
        private ItemCore itemCore;
        private EquipmentCore equipCore;
        private EliteCore eliteCore;
        //private MegaMushrum mushrum;
        //private WyattCore han_dCore;
        private EngineerCore engineerCore;
        private QOLCore qol;
        private CommandoCore commandoCore;
        //private AncientWispCore ancientWisp;
        //private ClayMenCore clayCore;
        private SipSipCore sipsipCore;
        //private A rchaicWispCore archwispCore;
        private RexCore rexCore;
        private HuntressCore wipCore;
        //private BombardierCore bombManCore;
        private AchievementCore achiveCore;
        private ProjectileCore projectileCore;
        private DamageTypeCore damageTypeCore;
        //shut up vs
        //private ArtifactsCore artifactCore;
        #endregion

        #region Config
        public static ConfigEntry<bool> EnableWyatt;
        public static ConfigEntry<bool> EnableSipSip;
        public static ConfigEntry<bool> EnableElites;
        public static ConfigEntry<bool> EnableCommando;
        public static ConfigEntry<bool> EnableEngineer;
        public static ConfigEntry<bool> EnableRex;
        public static ConfigEntry<bool> EnableItems;
        public static ConfigEntry<bool> EnableEquipment;

        public static ConfigEntry<bool> EnableWyattFreeFlight;

        public static ConfigEntry<bool> EnableVoid;
        public static ConfigEntry<bool> EnableNewt;

        public static ConfigEntry<bool> EnableWIP;
        public static ConfigEntry<bool> EnableUnlockAll;
        public static ConfigEntry<bool> Enabled;
        #endregion

        private static int vanillaErrors = 0;
        private static int modErrors = 0;

        public CloudburstPlugin()
        {   
            LogCore.logger = Logger;
            BepInEx.Logging.Logger.Listeners.Add(new ErrorListener());

            SingletonHelper.Assign<CloudburstPlugin>(ref CloudburstPlugin.instance, this);
            LogCore.LogI("Cloudburst instance assigned.");
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };

            /*var state = Resources.Load<GameObject>("prefabs/characterbodies/LoaderBody").GetComponent<SetStateOnHurt>();
            for (int i = 0; i < state.idleStateMachine.Length; i++)
            {
                LogCore.LogI("SetStateOnHurt: " + state.idleStateMachine[i].customName);
            }
            var machines = Resources.Load<GameObject>("prefabs/characterbodies/LoaderBody").GetComponent<NetworkStateMachine>().stateMachines;
            for (int i = 0; i < machines.Length; i++)
            {

                LogCore.LogI("NetworkStateMachine: " + machines[i].customName);
            }*/
            //important!!
            ErrorListener.vanillaErrors.addition += VanillaErrors_addition;
            ErrorListener.modErrors.addition += ModErrors_addition;
            //not super important!!

            DefineConfig();
            InitializeCores();

            LogCore.LogM("Cloudburst loaded!");
        }


        private void ModErrors_addition(ErrorListener.LogMessage objectRemoved)
        {
            modErrors++;
        }

        private void VanillaErrors_addition(ErrorListener.LogMessage msg)
        {
            vanillaErrors++;
        }

        private void DefineConfig()
        {
            //learn a smile
            //as every good day goes on by
            EnableNewt = Config.Bind("Cloudburst :: Newt", "Enabled", true, "Enables Newt speaking in-game. Set to false to seal his mouth shut.");
            EnableVoid = Config.Bind("Cloudburst :: Nilthnix", "Enabled", true, "Enables the cloudburst event in-game. Set to false to it completely (aka the alternate boss battle)");
            EnableWyattFreeFlight = Config.Bind("Cloudburst :: Custodian", "Free Flight", true, "Enables Custodian's free flight mechanic. Set to false to disable it.");
            EnableWyatt = Config.Bind("Cloudburst :: Custodian", "Enabled", true, "Enables Custodian as a survivor. Set to false to disable Custodian.");
            EnableSipSip = Config.Bind("Cloudburst :: SipSip", "Enabled", true, "Enables SipSip as a potential 5th lunar scavenger. Set to false to disable SipSip.");
            EnableElites = Config.Bind("Cloudburst :: Elites", "Enabled", true, "Enables custom elites. Set to false to disable Cloudburst's elites.");
            EnableRex = Config.Bind("Cloudburst :: REX", "Enabled", true, "Enables Cloudburst's modifications to REX. Set to false to disable Cloudburst's modifications to REX.");
            EnableCommando = Config.Bind("Cloudburst :: Commando", "Enabled", true, "Enables Cloudburst's modifications to Commando. Set to false to disable Cloudburst's modifications to Commando.");
            EnableEngineer = Config.Bind("Cloudburst :: Engineer", "Enabled", true, "Enables Cloudburst's modifications to Engineer. Set to false to disable Cloudburst's modifications to Engineer.");
            EnableItems = Config.Bind("Cloudburst :: Items", "Enabled", true, "Enables Cloudburst's items. Set to false to disable  's items.");
            EnableEquipment = Config.Bind("Cloudburst :: Equipment", "Enabled", false, "Enables Cloudburst's equipment. Set to false to disable Cloudburst's equipment.");
            EnableWIP = Config.Bind("Cloudburst :: WIP", "Enabled", false, "[WARNING]: CONTENT ADDED BY THIS MODULE MAY NOT BE STABLE. ENABLE AT YOUR OWN RISK! Enables Cloudburst's WIP (work in progress) content. Set to false to disable Cloudburst's WIP content.");
            EnableUnlockAll = Config.Bind("Cloudburst :: Achievements", "Enabled", false, "Enables Cloudburst's unlocks for unlockable content. Set to false to unlock all of Cloudburst's unlockable content.");
            Enabled = Config.Bind("Cloudburst", "Enabled", true, "THIS WILL NOT MAKE YOUR GAME COMPATIBLE WITH UNMODDED CLEINTS. Enables the mod. Set to false to disable the mod entirely. THIS WILL NOT MAKE YOUR GAME COMPATIBLE WITH UNMODDED CLEINTS.");
        }

        private void InitializeCores()
        {
            if (Enabled.Value)
            {
                //if i allow users to disable these cores they're gonna be stupid
                //so i can't do that
                //plus most cores rely on these
                assetCore = new AssetsCore();
                GlobalHooks.Init();
                CommonAssets.Load();


                RoR2.SplashScreenController.cvSplashSkip.defaultValue = "1";

                var materials = AssetsCore.mainAssetBundle.LoadAllAssets<Material>();
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].shader.name == "Standard")
                    {
                        materials[i].shader = Resources.Load<Shader>("shaders/deferred/hgstandard");
                    }
                    if (materials[i].name.Contains("GLASS"))
                    {
                        materials[i].shader = Resources.Load<Shader>("shaders/fx/hgintersectioncloudremap");
                    }
                    switch (materials[i].shader.name)
                    {

                        case "Hopoo Games/FX/Cloud Remap Proxy":
                            LogCore.LogI("material");
                            materials[i].shader = Resources.Load<Shader>("shaders/fx/hgcloudremap");
                            LogCore.LogI(materials[i].shader.name);
                            break;
                    }
                }

                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

                damageTypeCore = new DamageTypeCore();
                orbCore = new OrbCore();
                buffCore = new BuffCore();
                effectCore = new EffectCore();
                dotCore = new DoTCore();
                prefabCore = new PrefabCore();
                projectileCore = new ProjectileCore();
                achiveCore = new AchievementCore();
                //new CaptainCore();

                //got this feeling
                //i'm losing you
                //it's got me reeling
                //i need a clue
                if (EnableItems.Value)
                {
                    itemCore = new ItemCore();
                }
                if (EnableEquipment.Value)
                {
                    equipCore = new EquipmentCore();
                }

    

                if (EnableElites.Value)
                {
                    eliteCore = new EliteCore();
                }
                if (EnableEngineer.Value)
                {
                    engineerCore = new EngineerCore();
                }
                if (EnableCommando.Value)
                {
                    commandoCore = new CommandoCore();
                }
                if (EnableSipSip.Value)
                {
                    sipsipCore = new SipSipCore();
                }
                if (EnableWyatt.Value)
                {
                    new Custodian().Init(Config);
                    //wyattCore = new WyattCore();
                }
                if (EnableVoid.Value)
                {
                    new VoidCore();
                }
                if (EnableRex.Value)
                {
                    rexCore = new RexCore();
                }
                if (EnableWIP.Value)
                {
                    wipCore = new HuntressCore();
                    new EventsCore();
                }

                new EventsCore();

                //On.RoR2.CharacterBody.AddTimedBuff += GlobalHooks.CharacterBody_AddTimedBuff;
                //On.RoR2.HealthComponent.TakeDamage += GlobalHooks.HealthComponent_TakeDamage;

                new NewtCore();
                //and thus
                //we reach the end
                qol = new QOLCore();
                // mushrum = new MegaMushrum();
                // ancientWisp = new AncientWispCore();
                // bombManCore = new BombardierCore();
                // new DebuggingCore();
                postLoad?.Invoke();

                EnigmaticThunder.Modules.CommandHelper.AddToConsoleWhenReady();

                //i'll follow you home when the night comes
#if DEBUG
                //unlock!
                debugCore = new DebuggingCore();
#endif
            }
            else
            {
                LogCore.LogW("You have disabled ALL of Cloudburst. If this was not desired, you can re-enable it in Cloudburst's config file.");
            }
        }

        [ConCommand(commandName = "gmc", flags = ConVarFlags.ExecuteOnServer, helpText = "Good morning chat.")]
        private static void GMC(ConCommandArgs arg)
        {
            //how the fuck does this crash the game?
            NetworkManager.singleton.ServerChangeScene("goolake");
            //arg.TryGetSenderBody().inventory.GiveItem(Cores.Items.Red.REDACTED.instance.Index);
            /*foreach (ItemDef i in RoR2.RoR2Content.Items.itemDefs)
            {
                if (i.tier != ItemTier.NoTier && i.tier != ItemTier.Lunar)
                {
                    arg.TryGetSenderBody().inventory.GiveItem(i);
                }
            }*/

        }


        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            //var def = SceneCatalog.GetSceneDefFromScene(arg1);
            //onSceneLoaded(arg0, arg1, def, SceneInfo.instance);
            if (arg1 != default && arg1.name == "title")
            {
                var menu = GameObject.Find("MainMenu");
                //LogCore.LogI(menu.name)
                var title = menu.transform.Find("MENU: Title/TitleMenu/SafeZone/ImagePanel (JUICED)/LogoImage");
                var indicator = menu.transform.Find("MENU: Title/TitleMenu/MiscInfo/Copyright/Copyright (1)");

                var build = indicator.GetComponent<HGTextMeshProUGUI>();

                build.fontSize += 4;
                build.text = build.text + Environment.NewLine + $"Cloudburst Version: {version}";

                title.GetComponent<Image>().sprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/cloudburstlogo.png");
                //LogCore.LogI(title.name);

                //var rock = GameObject.Find("HOLDER: Title Background/Rocks/RockModular2(3)");
                var broom = Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("Broom"));

                broom.transform.Find("BroomRig/Handle/GyroBall").AddComponent<Spinner>();
                broom.transform.Find("BroomRig/Handle/GyroRing").AddComponent<Spinner>();

                var transform = broom.transform;
                transform.position = new Vector3(81, 4, 62);
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                transform.rotation = Quaternion.Euler(new Vector3(340, 340, 70));
            }
        }

        public void Awake()
        {
            Action awake = CloudburstPlugin.awake;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F)) {
                LogCore.LogI(LocalUserManager.localUsersList[0].cachedBody.transform.position);
            }
            
                Action fixedUpdate = CloudburstPlugin.onFixedUpdate;    
            if (fixedUpdate == null)
            {
                return;
            }
            fixedUpdate();
        }

        public void Start()
        {
            CloudburstPlugin.start();
            CloudburstPlugin.postStart();
        }

        public void OnDisable()
        {
            SingletonHelper.Unassign<CloudburstPlugin>(CloudburstPlugin.instance, this);
            LogCore.LogI("Cloudburst instance unassigned.");
            Action awake = CloudburstPlugin.onDisable;
            if (awake == null)
            {
                return;
            }
            awake();
        }
    }
}