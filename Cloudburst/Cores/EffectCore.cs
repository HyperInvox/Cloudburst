using UnityEngine;

using RoR2;
using TMPro;
using RoR2.UI;
using EnigmaticThunder.Modules;

namespace Cloudburst.Cores
{
    class EffectCore
    {
        public static EffectCore instance;

        public static GameObject unknownEffect;
        public static GameObject orbitalImpact;
        public static GameObject HANDRetrivalOrb;
        public static GameObject coinImpactEffect;
        public static GameObject wyattSwingTrail;
        public static GameObject wyatt2SwingTrail;

        public static GameObject magicArmor;
        public static GameObject magicRegen;
        public static GameObject magicAttackSpeed;

        public static GameObject coolEffect;
        public static GameObject reallyCoolEffect;
        public static GameObject trulyCoolEffect;

        public static GameObject maidCleanseEffect;


        public static GameObject blackHoleIncisionEffect;

        public static GameObject lumpkinEffect;
        public static GameObject willIsNotPoggers;

        public EffectCore() => DoEffects();


        protected void DoEffects() {
            LogCore.LogI("Initializing Core: " + base.ToString());
            instance = this;

            CreateNewEffects();
            Repair();


            blackHoleIncisionEffect = CreateAsset("UniversalIncison", false, false, true, "", false, VFXAttributes.VFXIntensity.Medium, VFXAttributes.VFXPriority.Always);

        }

        private void CreateMAIDCleanseEffect()
        {
            maidCleanseEffect = CreateAsset("MAIDCleanEffect", false, false, true, "", false, VFXAttributes.VFXIntensity.Medium, VFXAttributes.VFXPriority.Always);
        }

        private void WillIsStillNotPoggers() {
            willIsNotPoggers = CreateAsset("WillIsNotPoggers", false, false, true, "", false, VFXAttributes.VFXIntensity.Low, VFXAttributes.VFXPriority.Always);
            var unfortunatelyWillIsStillNotPoggers = willIsNotPoggers.AddComponent<ShakeEmitter>();

            unfortunatelyWillIsStillNotPoggers.wave = new Wave()
            {
                amplitude = 0.5f,
                cycleOffset = 0,
                frequency = 100,
            };
            unfortunatelyWillIsStillNotPoggers.duration = 0.5f;
            unfortunatelyWillIsStillNotPoggers.radius = 51;
            unfortunatelyWillIsStillNotPoggers.amplitudeTimeDecay = true;
        }

        private void CreateLumpkinEffect()
        {
            lumpkinEffect = CreateAsset("LumpkinScreamEffect", false, false, true, "", false, VFXAttributes.VFXIntensity.Medium, VFXAttributes.VFXPriority.Always);
            var component = lumpkinEffect.AddComponent<ShakeEmitter>();
            var component2 = lumpkinEffect.AddComponent<DestroyOnTimer>();

            component.wave = new Wave()
            {
                amplitude = 0.7f,
                cycleOffset = 0,
                frequency = 100,
            };
            component.duration = 2;
            component.radius = 15;
            component.amplitudeTimeDecay = true;
            component2.duration = 2;
        }

        protected void CreateMagiciansEarringsEffects()
        {
            coolEffect = CreateAsset("HANDHeal", false, false, true, "", false, VFXAttributes.VFXIntensity.Low, VFXAttributes.VFXPriority.Medium);
            reallyCoolEffect = CreateAsset("HANDArmor", false, false, true, "", false, VFXAttributes.VFXIntensity.Medium, VFXAttributes.VFXPriority.Medium);
            trulyCoolEffect = CreateAsset("HANDAttackSpeed", false, false, true, "", false, VFXAttributes.VFXIntensity.Medium, VFXAttributes.VFXPriority.Medium);

        }

        protected void Repair() {
            RepairAncientWispPillar();
            RepairHANDSwingEffect();
            RepairWyattEPICSwingEffect();
            RepairAncientWispSwingEffect();
        }

        protected void CreateNewEffects() {
            MakeHANDOrbEffect();
            CreateCoinImpactEffect();
            CreateMAIDCleanseEffect();
            CreateOrbitalImpact();
            CreateMagicEffects();
            WillIsStillNotPoggers();
            CreateMagiciansEarringsEffects();
            CreateLumpkinEffect();
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

            Effects.RegisterEffect(Effects.CreateGenericEffectDef(orbitalImpact));
        }

        private void CreateUnknownEliteEffect()
        {
            unknownEffect = Resources.Load<GameObject>("prefabs/PoisonAffixEffect").InstantiateClone("AAAAA", false);
            var fire = Resources.Load<GameObject>("prefabs/projectileghosts/RedAffixMissileGhost");
            var flames = Object.Instantiate<GameObject>(fire.transform.Find("Particles (1)/Flames").gameObject);

            //LogCore.LogI(flames);
            //flames.transform.SetParent(fire.transform);
            flames.transform.SetParent(unknownEffect.transform);

        }

        private void CreateMagicEffects()
        {
            Armor();
            Regen();
            AttackSpeed();
            void Armor() {
                magicArmor = Resources.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone("MagicEffectArmor", false);
                LogCore.LogI("hi1");
                var tmp = magicArmor.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<TextMeshPro>();
                LogCore.LogI("hi2");
                var langMeshController = magicArmor.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<LanguageTextMeshController>();
                LogCore.LogI("hi3");
                magicArmor.transform.Find("Fluff").gameObject.SetActive(false);
                LogCore.LogI("hi4");
               // Languages.Add("MAGIC_ARMOR_EFFECT", "+Armor!");
                LogCore.LogI("hi5");

                tmp.text = "+Armor!";
                LogCore.LogI("hi6");
                langMeshController.token = "MAGIC_ARMOR_EFFECT";
                LogCore.LogI("hi7");

                EnigmaticThunder.Modules.Effects.RegisterEffect(new EffectDef()
                {

                    prefab = magicArmor,
                    prefabEffectComponent = magicArmor.GetComponent<EffectComponent>(),
                    prefabVfxAttributes = magicArmor.GetComponent<VFXAttributes>(),
                    prefabName = magicArmor.name,
                });
            }
            void Regen() {
                magicRegen = Resources.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone("MagicEffectRegen", false);
                var tmp = magicRegen.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<TextMeshPro>();
                var langMeshController = magicRegen.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<LanguageTextMeshController>();

                magicRegen.transform.Find("Fluff").gameObject.SetActive(false);

                Languages.Add("MAGIC_REGEN_EFFECT", "+Regeneration!");

                tmp.text = "+Regeneration!";
                langMeshController.token = "MAGIC_REGEN_EFFECT";

                EnigmaticThunder.Modules.Effects.RegisterEffect(new EffectDef()
                {

                    prefab = magicRegen,
                    prefabEffectComponent = magicRegen.GetComponent<EffectComponent>(),
                    prefabVfxAttributes = magicRegen.GetComponent<VFXAttributes>(),
                    prefabName = magicRegen.name,
                });
            }
            void AttackSpeed() {
                magicAttackSpeed = Resources.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone("MagicEffectAttackSpeed", false);
                var tmp = magicAttackSpeed.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<TextMeshPro>();
                var langMeshController = magicAttackSpeed.transform.Find("TextCamScaler/TextRiser/TextMeshPro").GetComponent<LanguageTextMeshController>();

                magicAttackSpeed.transform.Find("Fluff").gameObject.SetActive(false);

                Languages.Add("MAGIC_ATKSPEED_EFFECT", "+Attack Speed!");

                tmp.text = "+Attack Speed!";
                langMeshController.token = "MAGIC_ATKSPEED_EFFECT";

                EnigmaticThunder.Modules.Effects.RegisterEffect(new EffectDef()
                {

                    prefab = magicAttackSpeed,
                    prefabEffectComponent = magicAttackSpeed.GetComponent<EffectComponent>(),
                    prefabVfxAttributes = magicAttackSpeed.GetComponent<VFXAttributes>(),
                    prefabName = magicAttackSpeed.name,
                });
            }
        }

        public static GameObject CreateAsset(string name, bool positionAtReferencedTransform, bool parentToReferencedTransform, bool applyScale, string soundName, bool disregardZScale, VFXAttributes.VFXIntensity intensity, VFXAttributes.VFXPriority priority)  {
            GameObject obj = AssetsCore.mainAssetBundle.LoadAsset<GameObject>(name);
            EffectComponent effectComponent = obj.AddComponent<EffectComponent>();
            VFXAttributes attributes = obj.AddComponent<VFXAttributes>();
            DestroyOnParticleEnd destroyOnEnd = obj.AddComponent<DestroyOnParticleEnd>();

            effectComponent.effectIndex = EffectIndex.Invalid;
            effectComponent.positionAtReferencedTransform = positionAtReferencedTransform;
            effectComponent.parentToReferencedTransform = parentToReferencedTransform;
            effectComponent.applyScale = applyScale;
            effectComponent.soundName = soundName;
            effectComponent.disregardZScale = disregardZScale;

            attributes.vfxIntensity = intensity;
            attributes.vfxPriority = priority;


            EnigmaticThunder.Modules.Effects.RegisterEffect(new EffectDef()
            {

                prefab = obj,
                prefabEffectComponent = obj.GetComponent<EffectComponent>(),
                prefabVfxAttributes = obj.GetComponent<VFXAttributes>(),
                prefabName = obj.name,
            });
            return obj;
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

            //EnigmaticThunder.Modules.EffectDefs.Add(coinImpactEffect);

            //EnigmaticThunder.Modules.EffectDefs.Add(coinImpactEffect);
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

            //EnigmaticThunder.Modules.EffectDefs.Add(HANDRetrivalOrb);

            //var core = HANDRetrivalOrb.transform.Find("VFX/Core");
            //var coreRenderer = core.GetComponent<Renderer>();

            //coreRenderer.material = Resources.Load<Material>("materials/matGrandparentMeleeSwipe");


            //var trail = orbEffect.transform.Find("TrailParent/Trail");
            //var trailRenderer = trail.GetComponent<Renderer>();
            //trailRenderer.material = Resources.Load<Material>("materials/matGrandparentMeleeSwipe");
            //EnigmaticThunder.Modules.EffectDefs.Add(orbEffect);
        }


        private void RepairHANDSwingEffect()
        {
            wyattSwingTrail = Resources.Load<GameObject>("prefabs/effects/handslamtrail").InstantiateClone("WyattSwingTrail", false);
            GameObject healEffect = Resources.Load<GameObject>("prefabs/effects/HANDheal");

            Transform HANDHealRingEffectTransform = healEffect.transform.Find("SwingTrail");
            Transform HANDSwingTrailTransform = wyattSwingTrail.transform.Find("SlamTrail");

            var ringRenderer = HANDHealRingEffectTransform.GetComponent<Renderer>();
            var HANDrenderer = HANDSwingTrailTransform.GetComponent<Renderer>();

            if (HANDrenderer)
            {
                HANDrenderer.material = CommonAssets.parentSwingEffectMaterial;
            }
            if (ringRenderer)
            {
                ringRenderer.material = CommonAssets.parentSwingEffectMaterial;
            }

            Effects.RegisterEffect(Effects.CreateGenericEffectDef(wyattSwingTrail));
        }
        private void RepairWyattEPICSwingEffect()
        {
            wyatt2SwingTrail = Resources.Load<GameObject>("prefabs/effects/handslamtrail").InstantiateClone("WyattSwingTrail2", false);

            Transform HANDSwingTrailTransform = wyatt2SwingTrail.transform.Find("SlamTrail");

            var HANDrenderer = HANDSwingTrailTransform.GetComponent<Renderer>();

            if (HANDrenderer)
            {
                HANDrenderer.material = CommonAssets.lemurianSwingEffectMaterial;
            }

            Effects.RegisterEffect(Effects.CreateGenericEffectDef(wyatt2SwingTrail));
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
