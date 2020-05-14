using OsbarMods.Assassinate.Missions;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class MissionLoader : IMissionLoader
    {
        private readonly IMissionOutcomeEvents _missionOutcomeHandler;
        private readonly IAssassinationActions _assassinationActions;
        private readonly IEncounterProvider _encounterProvider;
        private readonly IDialogProvider _dialogProvider;
        private readonly ILocationCharacterProvider _locationCharacterProvider;

        public MissionLoader(IMissionOutcomeEvents missionOutcomeHandler,
            IAssassinationActions assassinationActions,
            IEncounterProvider encounterProvider,
            IDialogProvider dialogProvider,
            ILocationCharacterProvider locationCharacterProvider)
        {
            _missionOutcomeHandler = missionOutcomeHandler;
            _assassinationActions = assassinationActions;
            _encounterProvider = encounterProvider;
            _dialogProvider = dialogProvider;
            _locationCharacterProvider = locationCharacterProvider;
        }

        public void LoadCastleAssassination(Settlement settlement, Hero assassinationTarget, bool sneakInSuccessful)
        {
            AssassinationMissions.OpenCastleAssassination(
                _encounterProvider,
                _locationCharacterProvider,
                _dialogProvider,
                _missionOutcomeHandler,
                _assassinationActions,
                settlement,
                assassinationTarget,
                sneakInSuccessful
            );
        }
    }
}
