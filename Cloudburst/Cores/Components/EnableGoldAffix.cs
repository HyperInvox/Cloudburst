using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components {
    class EnableGoldAffixEffect : MonoBehaviour
    {
        private GameObject goldAffixEffect;
        private SkinnedMeshRenderer mainSkinnedMeshRenderer;
        private CharacterModel characterModel;

        public bool isGoldEffectEnabled { get; private set; }

        public void Awake() {
            GameObject model = GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            characterModel = model.GetComponent<CharacterModel>();
        }   
        public void EnableGoldAffix() {
            mainSkinnedMeshRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");
            if (!this.goldAffixEffect)
            {
                goldAffixEffect = Instantiate<GameObject>(CommonAssets.goldAffix, base.transform);

                ParticleSystem[] sys = goldAffixEffect.GetComponentsInChildren<ParticleSystem>();

                for (int i = 0; i < sys.Length; i++) {
                    var shap = sys[i].shape;
                    shap.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    shap.skinnedMeshRenderer = mainSkinnedMeshRenderer;
                    return;
                }
                //ParticleSystem.ShapeModule shape = goldAffixEffect.GetComponents<ParticleSystem>().shape;

                if (mainSkinnedMeshRenderer)
                {
                    isGoldEffectEnabled = true;

                }
            }
            else {
                Destroy(this.goldAffixEffect);
                this.goldAffixEffect = null;
            }
        }
        public void OnDisable() {
            if (this.goldAffixEffect) {
                Destroy(this.goldAffixEffect);
                this.goldAffixEffect = null;
            }
        }
    }
}
