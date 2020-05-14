using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;

namespace OsbarMods.Assassinate
{
    public class SettlementAssassinateScreen : ScreenBase
    {
        private ISneakInHandler _sneakInHandler;

        private SettlementAssassinateScreenViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private Settlement _settlement;

        public SettlementAssassinateScreen(ISneakInHandler sneakInHandler, Settlement settlement)
        {
            _sneakInHandler = sneakInHandler;
            _settlement = settlement;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _dataSource = new SettlementAssassinateScreenViewModel(_sneakInHandler, _settlement);

            _gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };

            AddLayer(_gauntletLayer);

            _gauntletLayer.InputRestrictions.SetInputRestrictions();

            _movie = _gauntletLayer.LoadMovie("AssassinateAtSettlementScreen", _dataSource);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_gauntletLayer);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(_gauntletLayer);
            _dataSource = null;
            _gauntletLayer = null;
        }
    }
}
