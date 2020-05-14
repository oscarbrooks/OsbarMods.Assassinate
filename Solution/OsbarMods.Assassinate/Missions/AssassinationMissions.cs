using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Missions.Handlers;
using SandBox.Source.Missions.Handlers.Logic;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace OsbarMods.Assassinate.Missions
{
    [MissionManager]
    public static class AssassinationMissions
    {
        [MissionMethod]
        public static Mission OpenCastleAssassination(
            IEncounterProvider encounterProvider,
            ILocationCharacterProvider locationCharacterProvider,
            IDialogProvider dialogProvider,
            IMissionOutcomeEvents missionOutcomeHandler,
            IAssassinationActions assassinationActions,
            Settlement settlement,
            Hero assassinationTarget,
            bool sneakInSuccessful
        )
        {
            var encounterConfig = encounterProvider.GetSneakInEncounter(settlement);

            var locationToOpen = settlement.LocationComplex.GetLocationWithId(encounterConfig.LocationId);

            PlayerEncounter.Current.AddLocationEncounter(encounterConfig.LocationEncounter);

            var sceneName = locationToOpen.GetSceneName(encounterConfig.WallLevel);

            var missionRecord = SandBoxMissions.CreateSandBoxMissionInitializerRecord(sceneName, string.Empty, true);

            var missionBehaviors = new List<MissionBehaviour> {
                new MissionOptionsComponent(),
                new CampaignMissionComponent(),
                new MissionBasicTeamLogic(),
                new BasicLeaveMissionLogic(),// maybe implement own
                new LeaveMissionLogic(),//maybe implement own
                new MissionAgentLookHandler(),
                new MissionConversationHandler(),
                new MissionAgentHandler(locationToOpen, null),
                new HeroSkillHandler(),
                new MissionFightHandler(),
                new BattleAgentLogic(),
                new AgentTownAILogic(),//not sure, maybe implement my own version
                new MissionCrimeHandler(),// maybe keep
                new MissionFacialAnimationHandler(),
                new MissionDebugHandler(),
                new LocationItemSpawnHandler(),// remove?
                new VisualTrackerMissionBehavior(),
                new CastleAssassinationController(locationCharacterProvider, assassinationTarget, sneakInSuccessful),
                new AssassinationOutcomeLogic(missionOutcomeHandler, assassinationActions, settlement, assassinationTarget, sneakInSuccessful),
                new AssassinationConversationLogic(dialogProvider, Campaign.Current.ConversationManager, assassinationTarget.CharacterObject, sneakInSuccessful)
            };

            if (!sneakInSuccessful)
            {
                missionBehaviors.Add(new AssassinationCaughtByGuardsLogic(locationCharacterProvider, Campaign.Current.ConversationManager, settlement, assassinationTarget));
            }

            missionOutcomeHandler.PlayerAssassinationFailed += (settl, ass) => {
                OpenCapturedConversation(
                    encounterProvider,
                    dialogProvider,
                    assassinationActions,
                    settl,
                    ass
                );
            };

            return MissionState.OpenNew("CastleAssassination", missionRecord, (Mission mission) => missionBehaviors, true, true, true);
        }

        [MissionMethod]
        public static Mission OpenCapturedConversation(
            IEncounterProvider encounterProvider,
            IDialogProvider dialogProvider,
            IAssassinationActions assassinationActions,
            Settlement settlement,
            Hero assassinationTarget
        )
        {
            var encounterConfig = encounterProvider.GetCapturedEncounter(settlement);

            var locationToOpen = settlement.LocationComplex.GetLocationWithId(encounterConfig.LocationId);

            PlayerEncounter.Current.AddLocationEncounter(encounterConfig.LocationEncounter);

            var sceneName = locationToOpen.GetSceneName(encounterConfig.WallLevel);

            var dialogFlow = dialogProvider.GetCapturedDialogFlow(Hero.MainHero, assassinationTarget);

            var conversationManager = Campaign.Current.ConversationManager;

            var listenerHandle = new object();

            conversationManager.AddDialogFlow(dialogFlow, listenerHandle);

            CampaignEvents.ConversationEnded.AddNonSerializedListener(listenerHandle, character =>
            {
                CampaignEvents.ConversationEnded.ClearListeners(listenerHandle);

                conversationManager.RemoveRelatedLines(listenerHandle);

                assassinationActions.ApplyAssassinationFailedConsequences(settlement, Hero.MainHero, assassinationTarget);

                CampaignEvents.GameMenuOpened.AddNonSerializedListener(listenerHandle, args => {
                    CampaignEvents.GameMenuOpened.ClearListeners(listenerHandle);

                    GameMenu.SwitchToMenu("settlement_wait");
                });
            });

            return SandBoxMissions.OpenConversationMission(
                new ConversationCharacterData(Hero.MainHero.CharacterObject) {
                    IsCivilianEquipmentRequired = true,
                    NoHorse = true,
                    NoWeapon = true
                },
                new ConversationCharacterData(assassinationTarget.CharacterObject) {
                    IsCivilianEquipmentRequired = true,
                    NoWeapon = true,
                    NoHorse = true
                },
                sceneName
            );
        }
    }
}
