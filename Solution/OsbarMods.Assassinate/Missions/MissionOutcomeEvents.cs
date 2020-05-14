using System;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Missions
{
    public class MissionOutcomeEvents : IMissionOutcomeEvents
    {
        // This can probably just be an Action, there is only ever 1 listener at any given time
        public event PlayerAssassinationFailedDelegate PlayerAssassinationFailed;

        public MissionOutcomeEvents()
        {
        }

        public void ClearListeners()
        {
            foreach (Delegate del in PlayerAssassinationFailed.GetInvocationList())
            {
                PlayerAssassinationFailed -= (PlayerAssassinationFailedDelegate)del;
            }
        }

        public void OnPlayerAssassinationFailed(Settlement settlement, Hero assassinationTarget)
        {
            PlayerAssassinationFailed?.Invoke(settlement, assassinationTarget);

            ClearListeners();
        }
    }
}
