using OsbarMods.Assassinate.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public class SettlementAssassinationHistory
    {
        public Settlement Settlement { get; set; }

        public List<AssassinationEvent> AssassinationEvents { get; set; } = new List<AssassinationEvent>();

        public AssassinationEvent LastAssassinationEvent => AssassinationEvents.OrderByDescending(a => a.CampaignTime).FirstOrDefault();

        public CampaignTime TimeSinceLastAssassinationEvent => LastAssassinationEvent != null ? CampaignTime.Now - LastAssassinationEvent.CampaignTime : Campaign.Current.CampaignStartTime;

        public bool LastAssassinationSuccessful => LastAssassinationEvent != null ? LastAssassinationEvent.Succeeded : false;

        public bool IsInLockdown {
            get {
                if (LastAssassinationEvent == null) return false;

                var victim = LastAssassinationEvent.Victim;

                var minMaxDays = new Tuple<int, int>(7, 10);

                if (victim.IsClanLeader()) minMaxDays = new Tuple<int, int>(10, 14);

                if (victim.IsFactionLeader) minMaxDays = new Tuple<int, int>(14, 18);

                var rand = new Random((int)Math.Round(LastAssassinationEvent.CampaignTime.ToMinutes));

                var lockdownDays = rand.Next(minMaxDays.Item1, minMaxDays.Item2 + 1);

                return TimeSinceLastAssassinationEvent.ToDays < lockdownDays;
            }
        }
    }
}
