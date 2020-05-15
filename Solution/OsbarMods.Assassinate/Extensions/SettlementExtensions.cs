using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Extensions
{
    public static class SettlementExtensions
    {
        public static IEnumerable<Hero> GetHerosInLordsHall(this Settlement settlement)
        {
            if(settlement.IsCastle)
            {
                return settlement.HeroesWithoutParty
                    .Where(h => !h.IsPrisoner);
            }

            return settlement.LocationComplex
                .GetListOfCharactersInLocation("lordshall")
                .Where(c => !c.Character.HeroObject.IsPrisoner)
                .Select(c => c.Character.HeroObject);
        }

        public static IEnumerable<Hero> GetViableAssassinationTargetsInLordsHall(this Settlement settlement, Hero assassin)
        {
            return settlement
                .GetHerosInLordsHall()
                .Where(h => h.Clan.Leader != assassin && (h.Clan.Kingdom == null || h.Clan.Kingdom.Leader != assassin));
        }
    }
}
