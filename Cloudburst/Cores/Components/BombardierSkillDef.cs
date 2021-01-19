using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;

namespace Cloudburst.Cores.Skills
{
    public class IsNotroundedSkillDef : SkillDef
    {

        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new GroundedSkillDef.InstanceData
            {
                characterMotor = skillSlot.GetComponent<CharacterMotor>()
            };
        }

        public bool IsGrounded([NotNull] GenericSkill skillSlot)
        {
            GroundedSkillDef.InstanceData instanceData = (GroundedSkillDef.InstanceData)skillSlot.skillInstanceData;
            return instanceData.characterMotor && !instanceData.characterMotor.isGrounded;
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.HasRequiredStockAndDelay(skillSlot) && this.IsGrounded(skillSlot);
        }


        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public InstanceData()
            {
            }

            public CharacterMotor characterMotor;
        }
    }
}