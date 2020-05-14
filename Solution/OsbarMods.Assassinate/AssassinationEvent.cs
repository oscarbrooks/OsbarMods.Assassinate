using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class AssassinationEvent
    {
        public Settlement Settlement { get; set; }

        public Hero Victim { get; set; }

        public Hero Assassin { get; set; }

        public bool Succeeded { get; set; }

        public CampaignTime CampaignTime { get; set; }
    }
}
