using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class BombardierPassiveController : MonoBehaviour
    {
        public GenericSkill passiveSkillSlot;
        private CharacterBody body;
        public enum RocketType
        {
            Invalid = -1,
            Shocking,
            Concussive,
            GODENDORSESTHEPAINTRAINONLY
        };
        public void Awake()
        {
            body = base.GetComponent<CharacterBody>();
        }
        public RocketType GetBonus()
        {
            if (this.passiveSkillSlot)
            { 
                switch (passiveSkillSlot.skillDef.skillName) {
                    case "Shocking":
                        return RocketType.Shocking;
                    case "Concussive":
                        return RocketType.Concussive;
                    case "GODENDORSESTHEPAINTRAINONLY":
                        return RocketType.GODENDORSESTHEPAINTRAINONLY;
                }
            }
            return RocketType.Invalid;
        }

    }
}
