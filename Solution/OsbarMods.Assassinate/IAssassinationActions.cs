using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface IAssassinationActions
    {
        void ApplyAssassinationFailedConsequences(Settlement settlement, Hero assassin, Hero victim);

        void ApplySuccessfulAssassination(Settlement settlement, Hero assassin, Hero victim);
    }
}
