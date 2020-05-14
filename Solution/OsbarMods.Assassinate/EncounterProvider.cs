using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class EncounterProvider : IEncounterProvider
    {
        private const string LordsHallId = "lordshall";
        private const string PrisonId = "prison";

        public EncounterConfig GetSneakInEncounter(Settlement settlement)
        {
            if(settlement.IsCastle)
            {
                return new EncounterConfig()
                {
                    LocationEncounter = new CastleEncounter(settlement),
                    LocationId = LordsHallId,
                    WallLevel = settlement.GetComponent<Town>().GetWallLevel()
                };
            }

            return new EncounterConfig()
            {
                LocationEncounter = new TownEncounter(settlement) {
                    IsInsideOfASettlement = true,
                },
                LocationId = LordsHallId,
                WallLevel = settlement.GetComponent<Town>().GetWallLevel()
            };
        }

        public EncounterConfig GetCapturedEncounter(Settlement settlement)
        {
            if (settlement.IsCastle)
            {
                return new EncounterConfig()
                {
                    LocationEncounter = new CastleEncounter(settlement),
                    LocationId = PrisonId,
                    WallLevel = settlement.GetComponent<Town>().GetWallLevel()
                };
            }

            return new EncounterConfig()
            {
                LocationEncounter = new TownEncounter(settlement)
                {
                    IsInsideOfASettlement = true
                },
                LocationId = PrisonId,
                WallLevel = settlement.GetComponent<Town>().GetWallLevel()
            };
        }
    }
}
