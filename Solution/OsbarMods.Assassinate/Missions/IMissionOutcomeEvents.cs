using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Missions
{
    public interface IMissionOutcomeEvents
    {
        event PlayerAssassinationFailedDelegate PlayerAssassinationFailed;

        void ClearListeners();

        void OnPlayerAssassinationFailed(Settlement settlement, Hero assassinationTarget);
    }
}
