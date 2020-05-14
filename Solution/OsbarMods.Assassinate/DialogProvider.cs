using OsbarMods.Assassinate.Missions;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace OsbarMods.Assassinate
{
    public class DialogProvider : IDialogProvider
    {
        public DialogFlow GetAssassinationDialogFlow(Hero player, Hero victim, Action onFightStart, Action onLeave)
        {
            var dialogFlow = DialogFlow
                .CreateDialogFlow("start", int.MaxValue)
                .NpcLine(GetIntroText(player, victim), null, null)
                .BeginPlayerOptions()
                .PlayerOption("I am here to kill you!", null)
                .Consequence(() => onFightStart())
                .CloseDialog()
                .PlayerOption("I must apologise, I have gotten lost. I'll be on my way. (Leave)", null)
                .NpcLine("Hmm, very well then. I shall be having a word with my guards about their attentiveness.")
                .CloseDialog()
                .Consequence(() => onLeave())
                .EndPlayerOptions();

            return dialogFlow;
        }

        public DialogFlow GetCaughtByGuardsDialogFlow(Hero victim, bool canBribe, Action onFightStart, Action onGuardsBribed)
        {
            var bribeAmount = AssassinationCaughtByGuardsLogic.GuardsBribeAmount;

            var dialogFlow = DialogFlow
                .CreateDialogFlow("start", int.MaxValue)
                .NpcLine("Halt! Who are you? It is forbidden to bring weapons into the keep.", null, null)
                .BeginPlayerOptions()
                .PlayerOption($"I am here to put an end to {victim.Name}. Move aside or die!", null)
                .Consequence(() => onFightStart())
                .CloseDialog()
                .PlayerOption($"Perhaps we can come to some sort of agreement... (Bribe {string.Format("{0:n0}", bribeAmount)} gold)", null)
                .Condition(() => canBribe)
                .ClickableCondition((out TextObject explanation) => BribeGuardsClickableCondition(out explanation, bribeAmount))
                .NpcLine("That's more like it. Make it quick, before I change my mind.")
                .CloseDialog()
                .Consequence(() => onGuardsBribed())
                .EndPlayerOptions();

            return dialogFlow;
        }

        public DialogFlow GetCapturedDialogFlow(Hero player, Hero assassinationTarget)
        {
            var dialogFlow = DialogFlow
                .CreateDialogFlow("start", int.MaxValue)
                .NpcLine("You filthy rat. Did you think you would get away with it? Everyone shall know of your cowardice and failure.", null, null)
                .NpcLine("You should thank me for my mercy, for I'm only taking half of your wealth... Not that you'll need it when you're locked away in the dungeons.", null, null);

            return dialogFlow;
        }

        private string GetIntroText(Hero player, Hero victim)
        {
            return victim.HasMet
                ? $"{player.FirstName}, I wasn't expecting to see you."
                : "Who are you? I'm not expecting anyone right now.";
        }

        private bool BribeGuardsClickableCondition(out TextObject explanation, int bribeAmount)
        {
            var hasRequiredGold = Hero.MainHero.Gold >= bribeAmount;

            explanation = hasRequiredGold ? null : new TextObject("You do not have enough gold to bribe the guards.");
            return hasRequiredGold;
        }
    }
}
