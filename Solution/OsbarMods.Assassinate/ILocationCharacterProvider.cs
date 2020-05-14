using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface ILocationCharacterProvider
    {
        LocationCharacter GetFromCharacterObject(CharacterObject characterObject, bool useCivillianEquipment);

        LocationCharacter GetRandomFromTroopTree(CharacterObject basicTroop, int minTier, int maxTier);
    }
}
