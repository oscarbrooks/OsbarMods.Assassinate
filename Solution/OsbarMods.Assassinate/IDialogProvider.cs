using System;
using TaleWorlds.CampaignSystem;

namespace OsbarMods.Assassinate
{
    public interface IDialogProvider
    {
        DialogFlow GetAssassinationDialogFlow(Hero player, Hero victim, Action onFightStart, Action onLeave);

        DialogFlow GetCaughtByGuardsDialogFlow(Hero victim, bool canBribe, Action onFightStart, Action onGuardsBribed);

        DialogFlow GetCapturedDialogFlow(Hero player, Hero assassinationTarget);
    }
}
