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

        private const int MaximumGoldPenalty = 200000;

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

            var message = goldLossMessage.ToString()
                + $"\n{declareWarMessage}"
                + $"\n{takeCaptiveMessage}";

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

            PrintAssassinatedText(victim);

            _assassinationHistoryService.AddAssassinationEvent(new AssassinationEvent()
            {
                Settlement = settlement,
                Assassin = assassin,
                Victim = victim,
                Succeeded = true,
                CampaignTime = CampaignTime.Now
            });
        }

        private static int CalculateGoldLoss(int initialGold, int maximumGoldLoss)
        {
            var rawGoldLoss = initialGold / 2;

            return Math.Min(rawGoldLoss, maximumGoldLoss);
        }

        private static void PrintAssassinatedText(Hero victim)
        {
            var assassinatedText = new TextObject("{=5vbPC4NI}{VICTIM_NAME} has been assassinated.");

            assassinatedText.SetTextVariable("VICTIM_NAME", victim.Name.ToString());

            InformationManager.AddQuickInformation(assassinatedText);
        }

        private static void DeclareWarBetweenHeros(Hero assassin, Hero victim, out string declareWarMessage)
        {
            declareWarMessage = string.Empty;

            if (victim.MapFaction.IsAtWarWith(assassin.MapFaction)) return;

            if (assassin.Clan == victim.Clan)
            {
                throw new NotImplementedException("Assassinating members of the same clan is not supported.");
            }

            if (assassin.Clan.Kingdom != null && assassin.Clan.Kingdom == victim.Clan.Kingdom)
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

            declareWarMessage = $"\n{GameTexts.FindText("str_factions_declare_war_news", null)}.";
        }

        private static void TakeGoldFromHero(Hero hero, Clan captorClan, out string goldLossMessage)
        {
            var goldAmount = CalculateGoldLoss(hero.Gold, MaximumGoldPenalty);

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

            var message = new TextObject("{=AqBsz5xT}{CAPTIVE_HERO_NAME} is being held prisoner at {SETTLEMENT_NAME}.");

            message.SetTextVariable("CAPTIVE_HERO_NAME", hero.FirstName);
            message.SetTextVariable("SETTLEMENT_NAME", settlement.Name);

            takeCaptiveMessage = message.ToString();
        }
    }
}
