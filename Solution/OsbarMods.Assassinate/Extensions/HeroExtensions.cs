using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate.Extensions
{
    public static class HeroExtensions
    {
        public static bool IsClanLeader(this Hero hero) => hero.StringId == hero.Clan.Leader.StringId;
    }
}
