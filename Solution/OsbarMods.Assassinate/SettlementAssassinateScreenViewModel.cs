using OsbarMods.Assassinate.Extensions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace OsbarMods.Assassinate
{
    public class SettlementAssassinateScreenViewModel : ViewModel
    {
        private readonly ISneakInHandler _sneakInHandler;

        private readonly Settlement _settlement;

        private MBBindingList<SimpleCharacterPanelVM> _assassinationTargets;

        public SettlementAssassinateScreenViewModel(ISneakInHandler sneakInHandler, Settlement settlement)
        {
            _sneakInHandler = sneakInHandler;
            _settlement = settlement;

            var targetsList = new MBBindingList<SimpleCharacterPanelVM>();

            foreach (var hero in settlement.GetViableAssassinationTargetsInLordsHall(Hero.MainHero))
            {
                targetsList.Add(new SimpleCharacterPanelVM(hero, OnSneakIn));
            }

            AssassinationTargets = targetsList;
        }

        [DataSourceProperty]
        public MBBindingList<SimpleCharacterPanelVM> AssassinationTargets
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
        public string SneakInPercentageText
        {
            get
            {
                return $"Sneak success chance: {(int)Math.Round(_sneakInHandler.GetSneakInChance(Hero.MainHero) * 100)}%";
            }
        }

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
