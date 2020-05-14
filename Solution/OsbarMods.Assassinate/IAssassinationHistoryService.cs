using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface IAssassinationHistoryService
    {
        void AddAssassinationEvent(AssassinationEvent assassinationEvent);

        bool TryGetSettlementAssassinationHistory(Settlement settlement, out SettlementAssassinationHistory settlementAssassinationHistory);
    }
}
