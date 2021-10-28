using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

class CommonAssets
{
    public static GameObject lunarStarburst;
    public static GameObject smallChestStarburst;
    public static GameObject goldChestStarburst;
    public static GameObject lunarWispMinigunHitspark;
    public static GameObject burstIntoFlames;

    public static Material lemurianSwingEffectMaterial;
    public static Material ancientWispPillarEffect;
    public static Material parentSwingEffectMaterial;

    public static GameObject goldAffix;

    public static List<PostProcessProfile> profiles = new List<PostProcessProfile>();

    public static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

    public static void Load() {
        LogCore.LogI("Loading common assets...");

        profiles = Resources.FindObjectsOfTypeAll<PostProcessProfile>().ToList();

        lunarStarburst = Resources.Load<GameObject>("prefabs/LunarChestStarburst");
        smallChestStarburst = Resources.Load<GameObject>("prefabs/Chest1Starburst");
        goldChestStarburst = Resources.Load<GameObject>("prefabs/GoldChestStarburst");
        lunarWispMinigunHitspark = Resources.Load<GameObject>("prefabs/effects/impacteffects/LunarWispMinigunHitspark");
        burstIntoFlames = Resources.Load<GameObject>("prefabs/effects/impacteffects/IgniteExplosionVFX");

        goldAffix = Resources.Load<GameObject>("Prefabs/GoldAffixEffect");

        lemurianSwingEffectMaterial = Resources.Load<GameObject>("prefabs/effects/LemurianBiteTrail").transform.Find("SwingTrail").GetComponent<Renderer>().material;
        parentSwingEffectMaterial = Resources.Load<GameObject>("prefabs/effects/ParentSlamTrail").transform.Find("SwingTrail").GetComponent<Renderer>().material;
        //parentSwingEffectMaterial = Resources.Load<GameObject>("prefabs/effects/GrandparentGroundSwipeTrailEffect").transform.Find("EnergyParticles/SwipeTrail").GetComponent<Renderer>().material;   
        ancientWispPillarEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/AncientWispPillar").transform.Find("Particles/Flames, Tube, CenterHuge").GetComponent<Renderer>().material;

        Cloudburst.CloudburstPlugin.start += CloudburstPlugin_start;
        LogCore.LogI("Common assets loading finished!");

    }

    private static void CloudburstPlugin_start()
    {
        PopulateDisplays();    }

    public static GameObject LoadDisplay(string name)
    {
        //LogCore.LogI("HI1121");

        if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
        {
            //LogCore.LogI("HI112");
            if (itemDisplayPrefabs[name.ToLower()]) /*LogCore.LogI("HI1123");*/ return itemDisplayPrefabs[name.ToLower()];
        }
        return null;
    }


    internal static void PopulateDisplays()
    {
        ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

        ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

        for (int i = 0; i < item.Length; i++)
        {
            ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

            for (int j = 0; j < rules.Length; j++)
            {
                GameObject followerPrefab = rules[j].followerPrefab;
                if (followerPrefab)
                {
                    string name = followerPrefab.name;
                    string key = (name != null) ? name.ToLower() : null;
                    if (!itemDisplayPrefabs.ContainsKey(key))
                    {
                        itemDisplayPrefabs[key] = followerPrefab;
                    }
                }
            }
        }
    }
}