using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    public static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

    public static void Load() {
        LogCore.LogI("Loading common assets...");

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

        PopulateDisplays();
        LogCore.LogI("Common assets loading finished!");

    }

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

    private static void PopulateDisplays()
    {
        ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

        //LogCore.LogI("HELLO");

        BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        ItemDisplayRuleSet.NamedRuleGroup[] array = typeof(ItemDisplayRuleSet).GetField("namedItemRuleGroups", bindingAttr).GetValue(itemDisplayRuleSet) as ItemDisplayRuleSet.NamedRuleGroup[];
        ItemDisplayRuleSet.NamedRuleGroup[] array2 = typeof(ItemDisplayRuleSet).GetField("namedEquipmentRuleGroups", bindingAttr).GetValue(itemDisplayRuleSet) as ItemDisplayRuleSet.NamedRuleGroup[];
        ItemDisplayRuleSet.NamedRuleGroup[] array3 = array;

        for (int i = 0; i < array3.Length; i++)
        {
            //LogCore.LogI("HELLO2");

            ItemDisplayRule[] rules = array3[i].displayRuleGroup.rules;
            for (int j = 0; j < rules.Length; j++)
            {
                //LogCore.LogI("HELLO3");

                GameObject followerPrefab = rules[j].followerPrefab;
                if (!(followerPrefab == null))
                {
                    //LogCore.LogI("HELLO4");

                    string name = followerPrefab.name;
                    string key = (name != null) ? name.ToLower() : null;
                    if (!itemDisplayPrefabs.ContainsKey(key))
                    {
                        //LogCore.LogI("HELLO5");

                        itemDisplayPrefabs[key] = followerPrefab;
                    }
                }
            }
        }

        array3 = array2;
        for (int i = 0; i < array3.Length; i++)
        {
            //LogCore.LogI("HELLO6");

            ItemDisplayRule[] rules = array3[i].displayRuleGroup.rules;
            for (int j = 0; j < rules.Length; j++)
            {
                //LogCore.LogI("HELLO7");

                GameObject followerPrefab2 = rules[j].followerPrefab;
                if (!(followerPrefab2 == null))
                {
                    //LogCore.LogI("HELLO8");

                    string name2 = followerPrefab2.name;
                    string key2 = (name2 != null) ? name2.ToLower() : null;
                    if (!itemDisplayPrefabs.ContainsKey(key2))
                    {
                        //LogCore.LogI("HELLO9");

                        itemDisplayPrefabs[key2] = followerPrefab2;
                    }
                }
            }
        }
    }
}