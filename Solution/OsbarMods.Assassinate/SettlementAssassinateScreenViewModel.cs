using OsbarMods.Assassinate.Extensions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace OsbarMods.Assassinate
{
    public class SettlementAssassinateScreenViewModel : ViewModel
    {
        private readonly ISneakInHandler _sneakInHandler;

        private readonly Settlement _settlement;

        private MBBindingList<AssassinationCharacterPanel> _assassinationTargets;

        public SettlementAssassinateScreenViewModel(ISneakInHandler sneakInHandler, Settlement settlement)
        {
            _sneakInHandler = sneakInHandler;
            _settlement = settlement;

            var targetsList = new MBBindingList<AssassinationCharacterPanel>();

            foreach (var hero in settlement.GetViableAssassinationTargetsInLordsHall(Hero.MainHero))
            {
                targetsList.Add(new AssassinationCharacterPanel(hero, OnSneakIn));
            }

            AssassinationTargets = targetsList;
        }

        [DataSourceProperty]
        public MBBindingList<AssassinationCharacterPanel> AssassinationTargets
        {
            get
            {
                return _assassinationTargets;
            }
            set
            {
                if (value != _assassinationTargets)
                {
                    _assassinationTargets = value;
                    base.OnPropertyChanged("AssassinationTargets");
                }
            }
        }

        [DataSourceProperty]
        public string SneakInPercentageText {
            get
            {
                var text = new TextObject("{=VCTZvhpm}Sneak success chance: {SNEAK_SUCCESS_PERCENT}%");

                text.SetTextVariable("SNEAK_SUCCESS_PERCENT", (int)Math.Round(_sneakInHandler.GetSneakInChance(Hero.MainHero) * 100));

                return text.ToString();
            }
        }

        [DataSourceProperty]
        public string LeaveText => new TextObject("{=3sRdGQou}Leave").ToString();

        private void OnSneakIn(Hero assassinationTarget)
        {
            OnCloseMenu();

            _sneakInHandler.AttemptSneakIn(_settlement, assassinationTarget);
        }

        private void OnCloseMenu()
        {
            ScreenManager.PopScreen();
        }
    }
}
