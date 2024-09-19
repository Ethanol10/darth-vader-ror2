using DarthVaderMod.Modules.Survivors;
using RoR2;
using RoR2.Achievements;

namespace DarthVaderMod.Modules.Achievements
{
    [RegisterAchievement(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_ACHIEVEMENT_ID",
           DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_ANYDIFFICULTY_UNLOCKABLE_ID", null, 0)]
    internal class AnyDifficultyAchievement : BaseAchievement
    {
        private DarthVaderController darthCon;

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(Modules.Survivors.DarthVader.instance.fullBodyName);
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
        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }

            }
        }

    }
}