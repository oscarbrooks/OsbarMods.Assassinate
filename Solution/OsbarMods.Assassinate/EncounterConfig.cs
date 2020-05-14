using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class EncounterConfig
    {
        public LocationEncounter LocationEncounter { get; set; }

        /// <summary>
        /// The locationId within the settlement e.g. lordshall, village_center, etc...
        /// </summary>
        public string LocationId { get; set; }

        public int WallLevel { get; set; }
    }
}
