using UnityEngine;
using R2API;
using RoR2;

namespace Cloudburst.Cores
{
    class EffectCore
    {
        public static EffectCore instance;

        public static GameObject unknownEffect;
        public static GameObject orbitalImpact;
        public static GameObject HANDRetrivalOrb;
        public static GameObject coinImpactEffect;
        public EffectCore() => Effects();

        protected void Effects() {
            LogCore.LogI("Initializing Core: " + base.ToString());
            instance = this;

            CreateNewEffects();
            Repair();
        }
        
        protected void Repair() {
            RepairAncientWispPillar();
            RepairHANDSwingEffect();
            RepairAncientWispSwingEffect();
        }

        protected void CreateNewEffects() {
            MakeHANDOrbEffect();
            CreateCoinImpactEffect();
            CreateOrbitalImpact();
            CreateUnknownEliteEffect();
        }

        private void CreateOrbitalImpact() {
            orbitalImpact = Resources.Load<GameObject>("prefabs/effects/impacteffects/ParentSlamEffect").InstantiateClone("OrbitalImpactEffect", false);

            var particleParent = orbitalImpact.transform.Find("Particles");
            var debris = particleParent.Find("Debris, 3D");
            var debris2 = particleParent.Find("Debris");
            var sphere = particleParent.Find("Nova Sphere");

            debris.gameObject.SetActive(false);
            debris2.gameObject.SetActive(false);
            sphere.gameObject.SetActive(false);

            EffectAPI.AddEffect(orbitalImpact);
        }

        private void CreateUnknownEliteEffect()
        {
            unknownEffect = Resources.Load<GameObject>("prefabs/PoisonAffixEffect").InstantiateClone("AAAAA", false);
            //var fire = Resources.Load<GameObject>("prefabs/projectileghosts/RedAffixMissileGhost");
            //var flames = Object.Instantiate<GameObject>(fire.transform.Find("Particles (1)/Flames").gameObject);

            //LogCore.LogI(flames);
            //flames.transform.SetParent(fire.transform);
            //flames.transform.SetParent(unknownEffect.transform);

        }

        private void CreateCoinImpactEffect() {
            coinImpactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/AncientWispExplosion").InstantiateClone("CoinImpactEffect", false);

            //save performance
            var mat = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;
            var particles = coinImpactEffect.transform.Find("Particles");
            foreach (Transform child in particles) {

                var renderer = child.gameObject.GetComponent<Renderer>();
                var particleSystem = child.gameObject.GetComponent<ParticleSystem>();

                if (renderer && child.name != "Flash")
                {
                    renderer.material = mat;
                }
                if (particleSystem) {
                    var color = new Color(234/ 255, 60 / 255, 83 / 255);

                    var main = particleSystem.main;
                    var colorOverLifetime = particleSystem.colorOverLifetime;
                    var colorBySpeed = particleSystem.colorBySpeed;

                    main.startColor = color;
                    colorOverLifetime.color = color;
                    colorBySpeed.color = color;
                }
            }
            /*var flash = coinImpactEffect.transform.Find("Particles/Flash");
            var sparks = coinImpactEffect.transform.Find("Particles/Sparks");
            var flashWhite = coinImpactEffect.transform.Find("Particles/Flash,White");
            var centerHuge = coinImpactEffect.transform.Find("Particles/Flames, Tube (1)");

            sparks.gameObject.GetComponent<Renderer>().material = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;

            centerHuge.gameObject.GetComponent<Renderer>().material = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;

            flashWhite.gameObject.GetComponent<Renderer>().material = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;

            flash.gameObject.GetComponent<Renderer>().material = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;*/


            //var smoke = coinImpactEffect.transform.Find("ImpactStreaks_Ps");
            //smoke.gameObject.GetComponent<Renderer>().material = CommonAssets.burstIntoFlames.transform.Find("Flames").GetComponent<Renderer>().material;
            /*var flames = coinImpactEffect.transform.Find("Flames");
            flames.gameObject.SetActive(false);*/

            //EffectAPI.AddEffect(coinImpactEffect);

            EffectAPI.AddEffect(coinImpactEffect);
        }

            #region HAN-D
        private void MakeHANDOrbEffect()
        {
            //all that glitters dies here

            HANDRetrivalOrb = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/TreePoisonDartOrbEffect").InstantiateClone("HANDDroneOrbEffect", false);

            var trail = HANDRetrivalOrb.transform.Find("TrailParent/Trail");
            if (trail)
            {
                trail.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/matLunarGolemShield");
            }

            EffectAPI.AddEffect(HANDRetrivalOrb);

            //var core = HANDRetrivalOrb.transform.Find("VFX/Core");
            //var coreRenderer = core.GetComponent<Renderer>();

            //coreRenderer.material = Resources.Load<Material>("materials/matGrandparentMeleeSwipe");


            //var trail = orbEffect.transform.Find("TrailParent/Trail");
            //var trailRenderer = trail.GetComponent<Renderer>();
            //trailRenderer.material = Resources.Load<Material>("materials/matGrandparentMeleeSwipe");
            //EffectAPI.AddEffect(orbEffect);
        }


        private void RepairHANDSwingEffect()
        {
            GameObject HANDSwingTrail = Resources.Load<GameObject>("prefabs/effects/handslamtrail");
            GameObject healEffect = Resources.Load<GameObject>("prefabs/effects/HANDheal");

            Transform HANDHealRingEffectTransform = healEffect.transform.Find("SwingTrail");
            Transform HANDSwingTrailTransform = HANDSwingTrail.transform.Find("SlamTrail");

            var ringRenderer = HANDHealRingEffectTransform.GetComponent<Renderer>();
            var HANDrenderer = HANDSwingTrailTransform.GetComponent<Renderer>();

            if (HANDrenderer)
            {
                HANDrenderer.material = CommonAssets.lemurianSwingEffectMaterial;
            }
            if (ringRenderer) {
                ringRenderer.material = CommonAssets.lemurianSwingEffectMaterial;
            }
        }
        #endregion
        #region AncientWisp

        protected void RepairAncientWispSwingEffect()
        {
            var swingEffect = Resources.Load<GameObject>("prefabs/effects/AncientWispSwingTrail");
            var swing = swingEffect.transform.Find("SwingTrail");

            var renderer = swing.GetComponent<Renderer>();

            renderer.material = Resources.Load<GameObject>("prefabs/projectileghosts/AncientWispCannonGhost").transform.Find("Particles/FireSphere").GetComponent<Renderer>().material;
            //Resources.Load<Material>("materials/matGrandparentMeleeSwipe");
        }

        private void RepairAncientWispPillar() {
            var pillarEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/AncientWispPillar");

            var sparks = pillarEffect.transform.Find("Particles/Sparks");
            var centerHuge = pillarEffect.transform.Find("Particles/Flames, Tube (1)");
            var flashWhite = pillarEffect.transform.Find("Particles/Flash,White");
            var flash = pillarEffect.transform.Find("Particles/Flash");

            sparks.GetComponent<Renderer>().material = CommonAssets.ancientWispPillarEffect;
            centerHuge.GetComponent<Renderer>().material = CommonAssets.ancientWispPillarEffect;
            flashWhite.GetComponent<Renderer>().material = CommonAssets.ancientWispPillarEffect;
            flash.GetComponent<Renderer>().material = CommonAssets.ancientWispPillarEffect;
        }
        #endregion
    }
}
