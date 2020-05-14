using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface IEncounterProvider
    {
        /// <summary>
        /// Returns the LocationEncounter for the given settlement.
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        EncounterConfig GetSneakInEncounter(Settlement settlement);

        EncounterConfig GetCapturedEncounter(Settlement settlement);
    }
}
