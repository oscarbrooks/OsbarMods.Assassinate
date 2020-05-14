using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface IMissionLoader
    {
        void LoadCastleAssassination(Settlement settlement, Hero assassinationTarget, bool sneakInSuccessful);
    }
}
