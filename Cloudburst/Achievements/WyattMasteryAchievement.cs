using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudburst.Achievements
{
    public class WyattMasteryAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("NotARealPATH");

        public override int LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("WyattBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && difficultyDef.countsAsHardMode)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }

}
