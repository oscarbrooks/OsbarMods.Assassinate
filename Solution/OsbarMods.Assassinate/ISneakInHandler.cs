using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface ISneakInHandler
    {
        void AttemptSneakIn(Settlement settlement, Hero assassinationTarget);

        float GetSneakInChance(Hero assassin);
    }
}
