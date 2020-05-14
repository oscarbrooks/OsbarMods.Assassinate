using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class AssassinationHistoryService : IAssassinationHistoryService
    {
        private readonly Dictionary<Settlement, SettlementAssassinationHistory> _settlementHistories = new Dictionary<Settlement, SettlementAssassinationHistory>();

        public void AddAssassinationEvent(AssassinationEvent assassinationEvent)
        {
            if(!_settlementHistories.ContainsKey(assassinationEvent.Settlement))
            {
                _settlementHistories.Add(assassinationEvent.Settlement, new SettlementAssassinationHistory() {
                    Settlement = assassinationEvent.Settlement
                });
            }

            _settlementHistories[assassinationEvent.Settlement].AssassinationEvents.Add(assassinationEvent);
        }

        public bool TryGetSettlementAssassinationHistory(Settlement settlement, out SettlementAssassinationHistory settlementAssassinationHistory)
        {
            _settlementHistories.TryGetValue(settlement, out settlementAssassinationHistory);

            return _settlementHistories.ContainsKey(settlement);            
        }
    }
}
