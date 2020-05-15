using System;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Missions
{
    public interface IMissionOutcomeHandler
    {
        Action<Settlement, Hero> OnPlayerAssassinationFailed { get; }

        Action OnMissionEnded { get; }

        void Initialise(Action<Settlement, Hero> onPlayerAssassinationFailed, Action onMissionEnded);

        void Reset();
    }
}
