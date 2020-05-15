using System;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Missions
{
    public class MissionOutcomeHandler : IMissionOutcomeHandler
    {
        private Action<Settlement, Hero> _onPlayerAssassinationFailed;

        private Action _onMissionEnded;

        Action<Settlement, Hero> IMissionOutcomeHandler.OnPlayerAssassinationFailed => _onPlayerAssassinationFailed;

        Action IMissionOutcomeHandler.OnMissionEnded => _onMissionEnded;

        public MissionOutcomeHandler()
        {
        }

        public void Initialise(Action<Settlement, Hero> onPlayerAssassinationFailed, Action onMissionEnded)
        {
            _onPlayerAssassinationFailed = onPlayerAssassinationFailed;
            _onMissionEnded = onMissionEnded;
        }

        public void Reset()
        {
            _onPlayerAssassinationFailed = null;
            _onMissionEnded = null;
        }
    }
}
