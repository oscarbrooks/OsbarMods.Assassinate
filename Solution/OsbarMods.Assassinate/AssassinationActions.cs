using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace OsbarMods.Assassinate
{
    public class AssassinationActions : IAssassinationActions
    {
        private readonly IAssassinationHistoryService _assassinationHistoryService;

        public AssassinationActions(IAssassinationHistoryService assassinationHistoryService)
        {
            _assassinationHistoryService = assassinationHistoryService;
        }

        public void ApplyAssassinationFailedConsequences(Settlement settlement, Hero assassin, Hero victim)
        {
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(assassin, victim, -100, true);

            DeclareWarBetweenHeros(assassin, victim, out var declareWarMessage);

            TakeGoldFromHero(assassin, victim.Clan, out var goldLossMessage);

            TakeHeroCaptive(settlement, assassin, out var takeCaptiveMessage);

            var message = goldLossMessage
                + declareWarMessage
                + takeCaptiveMessage;

            var inquiry = new InquiryData(
                "Captured",
                message,
                true,
                false,
                "Continue",
                string.Empty,
                null,
                null
            );

            InformationManager.ShowInquiry(inquiry);

            _assassinationHistoryService.AddAssassinationEvent(new AssassinationEvent() {
                Settlement = settlement,
                Assassin = assassin,
                Victim = victim,
                Succeeded = false,
                CampaignTime = CampaignTime.Now
            });
        }

        public void ApplySuccessfulAssassination(Settlement settlement, Hero assassin, Hero victim)
        {
            var party = victim.OwnedParties.Where(p => p.LeaderHero == victim).FirstOrDefault();

            if (party != null) party.MobileParty.RemoveParty();

            KillCharacterAction.ApplyByMurder(victim);
            InformationManager.AddQuickInformation(new TextObject($"{victim.Name} has been assassinated."));

            _assassinationHistoryService.AddAssassinationEvent(new AssassinationEvent()
            {
                Settlement = settlement,
                Assassin = assassin,
                Victim = victim,
                Succeeded = true,
                CampaignTime = CampaignTime.Now
            });
        }

        private static void DeclareWarBetweenHeros(Hero assassin, Hero victim, out string declareWarMessage)
        {
            declareWarMessage = string.Empty;

            if (victim.MapFaction.IsAtWarWith(assassin.MapFaction)) return;

            if (assassin.Clan == victim.Clan)
            {
                throw new NotImplementedException("Assassinating members of the same clan is not supported.");
            }

            if (assassin.Clan.Kingdom == victim.Clan.Kingdom)
            {
                if(assassin.IsFactionLeader)
                {
                    throw new NotImplementedException("Assassinating members of the kingdom you own is not supported.");
                }

                ChangeKingdomAction.ApplyByLeaveKingdom(assassin.Clan);
            }

            DeclareWarAction.ApplyDeclareWarOverProvocation(victim.MapFaction, assassin.MapFaction);

            GameTexts.SetVariable("FACTION1_NAME", victim.MapFaction.Name);
            GameTexts.SetVariable("FACTION2_NAME", assassin.MapFaction.Name);

            declareWarMessage = "\n" + GameTexts.FindText("str_factions_declare_war_news", null).ToString();
        }

        private static void TakeGoldFromHero(Hero hero, Clan captorClan, out string goldLossMessage)
        {
            var goldAmount = hero.Gold / 2;

            GiveGoldAction.ApplyBetweenCharacters(hero, captorClan.Leader, goldAmount, true);

            GameTexts.SetVariable("AMOUNT", goldAmount);

            goldLossMessage = GameTexts.FindText("str_you_have_lost_AMOUNT_denars", null).ToString();

            InformationManager.DisplayMessage(new InformationMessage(goldLossMessage));
        }

        private static void TakeHeroCaptive(Settlement settlement, Hero hero, out string takeCaptiveMessage)
        {
            GameMenu.ExitToLast();
            PartyBase.MainParty.AddElementToMemberRoster(hero.CharacterObject, -1, true);
            TakePrisonerAction.Apply(settlement.Party, hero);

            takeCaptiveMessage = $"\n{hero.FirstName} is being held prisoner at {settlement.Name}";
        }
    }
}
