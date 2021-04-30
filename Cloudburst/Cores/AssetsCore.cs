
using HG;
using RoR2.ContentManagement;
using System.Linq;
using System.Reflection;
using ThreeEyedGames;
using UnityEngine;

namespace Cloudburst.Cores
{
    class AssetsCore
    {
        //yeah no instance 
        public static AssetBundle mainAssetBundle = null;
        //public static AssetBundleResourcesProvider provider;

        public static AssetBundle HANDAssetBundle = null;
        //public static AssetBundleResourcesProvider HANDProvider;

        public static AssetBundle bombAssetBundle = null;
        //public static AssetBundleResourcesProvider bombProvider;

        // icons
        public static GameObject grinderModel;

        #region HAN-D
        public static Sprite winchIcon = null;
        public static Texture2D portraitIcon = null;
        public static Sprite droneIcon = null;
        public static Sprite forcedReassemblyIcon = null;
        public static Sprite overclockBuffIcon = null;
        public static Sprite passiveIcon = null;
        public static Sprite hurtIcon = null;
        public static Sprite overclockIcon = null;
        public static Sprite explungeIcon = null;
        public static Sprite passiveDroneBuffIcon = null;
        public static Sprite unethicalReassemblyIcon = null;
        public static Sprite passive = null;
        #endregion

        public static Sprite rexHarvester;
        public static Sprite engiImpact;
        public static Sprite engiFlameTurrets;

        public static Sprite wyattPassive;
        public static Sprite wyattPrimary;
        public static Sprite wyattSecondary;
        public static Sprite wyattUtility;
        public static Sprite wyattUtilityAlt;
        public static Sprite wyattSpecial;
        public static Sprite wyattSpecial2;

        public static Material brickWall;
        public AssetsCore()
        {
            CloudburstPlugin.postLoad += CloudburstPlugin_postLoad;
            CloudburstPlugin.start += CloudburstPlugin_start;
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManager_onContentPacksAssigned;
            LogCore.LogI("Initializing Core: " + base.ToString());

            // populate ASSETS
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudburst.assetburst"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    //provider = new AssetBundleResourcesProvider("@Cloudburst", mainAssetBundle);
                    //ResourcesAPI.AddProvider(provider);
                }
            }

            if (bombAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudburst.examplesurvivorbundle"))
                {
                    bombAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    //bombProvider = new AssetBundleResourcesProvider("@Bombardier", bombAssetBundle);
                    //ResourcesAPI.AddProvider(bombProvider);
                }
            }
            // gather assets
            grinderModel = mainAssetBundle.LoadAsset<GameObject>("Assets/Items/Grinder/MDLGrinder.prefab");

            //skill icons
            rexHarvester = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Icons/REX/texTreebotHarvester.png");

            engiImpact = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Icons/Engineer/Impact_Grenades.png");
            engiFlameTurrets = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Icons/Engineer/TR75_Immolator_Turret_2.png");

            brickWall = mainAssetBundle.LoadAsset<Material>("Assets/Cloudburst/Materials/brickwall.mat");

            wyattPassive = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/passive.png");
            wyattPrimary = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/primaryicon.png");
            wyattSecondary = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/secondaryicon.png");
            wyattUtility = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/utilityicon.png");
            wyattUtilityAlt = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/utilityicon2.png");
            wyattSpecial = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/specialicon.png");
            wyattSpecial2 = mainAssetBundle.LoadAsset<Sprite>("Assets/Cloudburst/Survivors/Wyatt/specialicon2.png");
        }

        private void ContentManager_onContentPacksAssigned(ReadOnlyArray<ReadOnlyContentPack> obj)
        {
            Material[] source = Resources.FindObjectsOfTypeAll<Material>();
            Material mfMat = Resources.Load<GameObject>("prefabs/temporaryvisualeffects/SlowDownTime").transform.Find("Visual/Mesh").GetComponent<Renderer>().material;

            foreach (GameObject gameObject in mainAssetBundle.LoadAllAssets<GameObject>())
            {
                MaterialSwapper[] quickSwap = gameObject.GetComponentsInChildren<MaterialSwapper>();
                foreach (MaterialSwapper swap in quickSwap)
                {
                    //LogCore.LogI(swap.gameObject.name);
                    //LogCore.LogI(swap.materialName);
                    Material material = (from p in source
                                         where p.name == swap.materialName//"ppLocalUnderwater"
                                         select p).FirstOrDefault<Material>();

                    Decal decal = swap.GetComponent<Decal>();
                    if (!decal)
                    {
                        Renderer renderer = swap.gameObject.GetComponent<Renderer>();
                        if (renderer)
                        {
                            if (swap.materialName == "matBaubleEffect")
                            {
                                material = mfMat;
                            }
                            if (material)
                            {
                                renderer.material = material;
                                renderer.sharedMaterial = material;
                            }


                            else
                            {
                                LogCore.LogE(swap.materialName + $" could not be found! Attempting alternative loading method! GameObject: {swap.gameObject}");
                                Material newMat = Resources.Load<Material>("Material/" + swap.materialName);

                                if (newMat)
                                {

                                    renderer.material = newMat;
                                    renderer.sharedMaterial = newMat;
                                }
                                else
                                {
                                    LogCore.LogE(swap.materialName + " cannot be found! Alternative loading method failed!");

                                }
                            }
                        }
                    }
                    else {
                        decal.Material = material;
                        LogCore.LogI(decal.gameObject);
                    }
                    CloudburstPlugin.Destroy(swap);
                }
                /*if (quicKSwap)
                {
                    LogCore.LogI(gameObject.name + " has a MaterialSwapper component attached.");

                    LogCore.LogI(quicKSwap.materialName);
                }
                else {
                    LogCore.LogI(gameObject.name + " does not have a MaterialSwapper component attached.");
                }*/
            }
        }

        private void CloudburstPlugin_start()
        {

        }

        private void CloudburstPlugin_postLoad()
        {
            /*Shader[] source = Resources.FindObjectsOfTypeAll<Shader>();
            foreach (GameObject gameObject in mainAssetBundle.LoadAllAssets<GameObject>())
            {
                MaterialSwapper[] quickSwap = gameObject.GetComponentsInChildren<MaterialSwapper>();
                foreach (MaterialSwapper swap in quickSwap)
                {
                    LogCore.LogI(swap.gameObject.name);
                    LogCore.LogI(swap.materialName);

                    Renderer renderer = swap.gameObject.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.material.shader = (from p in source
                                             where p.name == swap.materialName//"ppLocalUnderwater"
                                             select p).FirstOrDefault<Shader>();
                    }
                }
                /*if (quicKSwap)
                {
                    LogCore.LogI(gameObject.name + " has a MaterialSwapper component attached.");

                    LogCore.LogI(quicKSwap.materialName);
                }
                else {
                    LogCore.LogI(gameObject.name + " does not have a MaterialSwapper component attached.");
                }*/
            
        }
    }
}