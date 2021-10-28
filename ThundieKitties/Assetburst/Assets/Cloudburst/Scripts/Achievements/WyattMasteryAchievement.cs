using RoR2;
using System;
using UnityEngine;
using R2API;

namespace Cloudburst.Achievements
{
    public class WyattMasteryAchievement : ModdedUnlockable
    {
        public override String AchievementIdentifier { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "";
        public override String AchievementNameToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite Sprite { get; } = null;
       // public override bool ForceDisable { get; } = false;

        public override Func<string> GetHowToUnlock => throw new NotImplementedException();

        public override Func<string> GetUnlocked => throw new NotImplementedException();

        public override BodyIndex LookUpRequiredBodyIndex()
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
