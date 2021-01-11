using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cloudburst.Cores
{
    class PrefabCore
    {
        public static GameObject pillar;
        public PrefabCore() => CreatePrefabs();


        public void CreatePrefabs()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            CloudburstPlugin.start += CreateMushrumPillarEffect;
        }

        public void CreateMushrumPillarEffect()
        {
            pillar = EntityStates.AncientWispMonster.ChannelRain.delayPrefab; //.InstantiateClone("AncientWispPillarExplosion", true);

            var centralRing = pillar.transform.Find("Particles/Ring, Center");

            //centralRing.gameObject.GetComponent<Renderer>().material = Resources.Load<GameObject>("prefabs/projectileghosts/AncientWispCannonGhost").transform.Find("Particles/FireSphere").GetComponent<Renderer>().material;
            //centralRing.GetComponent<Renderer>().material = Resources.Load<GameObject>("prefabs/effects/impacteffects/AncientWispPillar").transform.Find("Particles/Flames, Tube, CenterHuge").GetComponent<Renderer>().material;
            centralRing.GetComponent<Renderer>().material = Resources.Load<GameObject>("prefabs/effects/ParentPodHatchEffect").transform.Find("Particles/EnergyInitialParticle (1)").GetComponent<Renderer>().material;
        }
    }
}
