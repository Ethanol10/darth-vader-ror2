using DarthVaderMod.Modules.Survivors;
using RoR2;
using RoR2.Achievements;
using RoR2.Stats;
using System;

namespace DarthVaderMod.Modules.Achievements
{
    [RegisterAchievement(DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_ACHIEVEMENT_ID",
           DarthVaderPlugin.DEVELOPER_PREFIX + "_DARTHVADER_BODY_STAGE_UNLOCKABLE_ID", null, 0)]
    internal class StageAchievement : BaseAchievement
    {
        private bool listeningForStats;
        private UserProfile subscribedProfile;
        private MemoizedGetComponent<PlayerStatsComponent> playerStatsComponentGetter;

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(Modules.Survivors.DarthVader.instance.fullBodyName);
        }

        public override void OnInstall()
        {
            base.OnInstall();
            Run.onRunStartGlobal += this.OnRunStart;
        }
        private void OnRunStart(Run run)
        {
            this.SetListeningForStats(true);
        }
        public override void OnUninstall()
        {
            Run.onRunStartGlobal -= this.OnRunStart;
            this.SetListeningForStats(false);
            base.OnUninstall();
        }
        private void SetListeningForStats(bool shouldListen)
        {
            if (this.listeningForStats == shouldListen)
            {
                return;
            }
            this.listeningForStats = shouldListen;
            if (this.listeningForStats)
            {
                this.subscribedProfile = base.localUser.userProfile;
                UserProfile userProfile = this.subscribedProfile;
                userProfile.onStatsReceived = (Action)Delegate.Combine(userProfile.onStatsReceived, new Action(this.OnStatsReceived));
                return;
            }
            UserProfile userProfile2 = this.subscribedProfile;
            userProfile2.onStatsReceived = (Action)Delegate.Remove(userProfile2.onStatsReceived, new Action(this.OnStatsReceived));
            this.subscribedProfile = null;
        }

        private void OnStatsReceived()
        {
            PlayerStatsComponent playerStatsComponent = this.playerStatsComponentGetter.Get(base.localUser.cachedMasterObject);
            if (playerStatsComponent && playerStatsComponent.currentStats.GetStatValueULong(StatDef.highestStagesCompleted) >= 20UL)
            {
                base.Grant();
            }
        }

    }
}