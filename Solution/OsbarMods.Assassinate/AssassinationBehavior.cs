using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using OsbarMods.Assassinate.Extensions;
using TaleWorlds.Localization;
using System.Linq;

namespace OsbarMods.Assassinate
{
    public class AssassinationBehavior : CampaignBehaviorBase
    {
        private readonly IAssassinationHistoryService _assassinationHistoryService;
        private readonly ISneakInHandler _sneakInHandler;

        private const string AssassinateOutsideCastleOptionId = "osbar.assassinate.castle_outside";
        private const string AssassinateCastleOptionId = "osbar.assassinate.castle";
        private const string AssassinateTownKeepBribeOptionId = "osbar.assassinate.town_keep_bribe";
        private const string AssassinateTownKeepOptionId = "osbar.assassinate.town_keep";

        public AssassinationBehavior(IAssassinationHistoryService assassinationHistoryService, ISneakInHandler sneakInHandler)
        {
            _assassinationHistoryService = assassinationHistoryService;
            _sneakInHandler = sneakInHandler;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this,
                new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            CreateAssassinationMenus(campaignGameStarter);
        }

        private void CreateAssassinationMenus(CampaignGameStarter campaignGameStarter)
        {
            AddAssassinateAtSettlementOption(campaignGameStarter);
        }

        private void AddAssassinateAtSettlementOption(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                "castle_outside",
                AssassinateOutsideCastleOptionId,
                "Assassinate...",
                MenuCondition,
                MenuConsequence,
                false,
                1
            );

            campaignGameStarter.AddGameMenuOption(
                "castle",
                AssassinateCastleOptionId,
                "Assassinate...",
                MenuCondition,
                MenuConsequence,
                false,
                1
            );

            campaignGameStarter.AddGameMenuOption(
                "town_keep_bribe",
                AssassinateTownKeepBribeOptionId,
                "Assassinate...",
                MenuCondition,
                MenuConsequence,
                false,
                1
            );

            campaignGameStarter.AddGameMenuOption(
                "town_keep",
                AssassinateTownKeepOptionId,
                "Assassinate...",
                MenuCondition,
                MenuConsequence,
                false,
                1
            );
        }

        private bool MenuCondition(MenuCallbackArgs args)
        {
            var currentSettlement = Settlement.CurrentSettlement;

            if (_assassinationHistoryService.TryGetSettlementAssassinationHistory(currentSettlement, out var settlementAssassinationHistory))
            {
                var isLockedDown = settlementAssassinationHistory.IsInLockdown;

                args.IsEnabled = !isLockedDown;

                if (isLockedDown)
                {
                    var lockdownReason = settlementAssassinationHistory.LastAssassinationSuccessful ? "a recent assassination" : "a recently attempted assassination";

                    args.Tooltip = new TextObject($"The keep is on lockdown due to {lockdownReason}.");
                }
            }

            args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
            return currentSettlement.GetViableAssassinationTargetsInLordsHall(Hero.MainHero).Any();
        }

        private void MenuConsequence(MenuCallbackArgs args)
        {
            var currentSettlement = Settlement.CurrentSettlement;

            ScreenManager.PushScreen(new SettlementAssassinateScreen(_sneakInHandler, currentSettlement));
        }
    }
}
