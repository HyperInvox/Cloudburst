using R2API;
using System.Reflection;
using UnityEngine;

namespace Cloudburst.Cores
{
    class AssetsCore
    {
        //yeah no instance 
        public static AssetBundle mainAssetBundle = null;
        public static AssetBundleResourcesProvider provider;

        public static AssetBundle HANDAssetBundle = null;
        public static AssetBundleResourcesProvider HANDProvider;

        public static AssetBundle bombAssetBundle = null;
        public static AssetBundleResourcesProvider bombProvider;

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
            LogCore.LogI("Initializing Core: " + base.ToString());

            // populate ASSETS
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudburst.assetburst"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    provider = new AssetBundleResourcesProvider("@Cloudburst", mainAssetBundle);
                    ResourcesAPI.AddProvider(provider);
                }
            }

            if (bombAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudburst.examplesurvivorbundle"))
                {
                    bombAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    bombProvider = new AssetBundleResourcesProvider("@Bombardier", bombAssetBundle);
                    ResourcesAPI.AddProvider(bombProvider);
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
    }
}