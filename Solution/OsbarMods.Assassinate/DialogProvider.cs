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
                .NpcLine(GetIntroText(player, victim))
                .BeginPlayerOptions()
                .PlayerOption(new TextObject("{=UlByid6B}I am here to kill you!"))
                .Consequence(() => onFightStart())
                .CloseDialog()
                .PlayerOption(new TextObject("{=urBkJcfs}I must apologise, I have gotten lost. I'll be on my way. (Leave)"))
                .NpcLine(new TextObject("{=d6Y8pYjf}Hmm, very well then. I shall be having a word with my guards about their attentiveness."))
                .CloseDialog()
                .Consequence(() => onLeave())
                .EndPlayerOptions();

            return dialogFlow;
        }

        public DialogFlow GetCaughtByGuardsDialogFlow(Hero victim, bool canBribe, Action onFightStart, Action onGuardsBribed)
        {
            var bribeAmount = AssassinationCaughtByGuardsLogic.GuardsBribeAmount;

            var startFightOption = new TextObject("{=aaC7DDfK}I am here to put an end to {VICTIM_NAME}. Move aside or die!");
            startFightOption.SetTextVariable("VICTIM_NAME", victim.FirstName);

            var bribeGuardsOption = new TextObject("{=3N7FUr35}Perhaps we can come to some sort of agreement... (Bribe {BRIBE_AMOUNT} gold)");
            bribeGuardsOption.SetTextVariable("BRIBE_AMOUNT", string.Format("{0:n0}", bribeAmount));

            var dialogFlow = DialogFlow
                .CreateDialogFlow("start", int.MaxValue)
                .NpcLine(new TextObject("{=cIL4bcTL}Halt! Who are you? It is forbidden to bring weapons into the keep."))
                .BeginPlayerOptions()
                .PlayerOption(startFightOption)
                .Consequence(() => onFightStart())
                .CloseDialog()
                .PlayerOption(bribeGuardsOption)
                .Condition(() => canBribe)
                .ClickableCondition((out TextObject explanation) => BribeGuardsClickableCondition(out explanation, bribeAmount))
                .NpcLine(new TextObject("{=a1tXDKmk}That's more like it. Make it quick, before I change my mind."))
                .CloseDialog()
                .Consequence(() => onGuardsBribed())
                .EndPlayerOptions();

            return dialogFlow;
        }

        public DialogFlow GetCapturedDialogFlow(Hero player, Hero assassinationTarget)
        {
            var dialogFlow = DialogFlow
                .CreateDialogFlow("start", int.MaxValue)
                .NpcLine(new TextObject("{=h1HEeHEk}You filthy rat. Did you really think you would get away with it? Everyone shall know of your cowardice and failure."))
                .NpcLine(new TextObject("{=2qCq9SXN}You should thank me for my mercy, for I'm only taking half of your wealth... Not that you'll need it when you're locked away in the dungeons."));

            return dialogFlow;
        }

        private TextObject GetIntroText(Hero player, Hero victim)
        {
            if(victim.HasMet)
            {
                var text = new TextObject("{=pUuxJPsj}{PLAYER_NAME}, I wasn't expecting to see you.");

                text.SetTextVariable("PLAYER_NAME", player.FirstName);

                return text;
            }

            return new TextObject("{=VSNb0Fng}Who are you? I'm not expecting anyone right now.");
        }

        private bool BribeGuardsClickableCondition(out TextObject explanation, int bribeAmount)
        {
            var hasRequiredGold = Hero.MainHero.Gold >= bribeAmount;

            explanation = hasRequiredGold ? null : new TextObject("{=L6Lg32mJ}You do not have enough gold to bribe the guards.");
            return hasRequiredGold;
        }
    }
}
