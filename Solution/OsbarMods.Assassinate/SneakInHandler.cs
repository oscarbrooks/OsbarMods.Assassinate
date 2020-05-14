using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace OsbarMods.Assassinate
{
    public class SneakInHandler : ISneakInHandler
    {
        private readonly IMissionLoader _missionLoader;

        private readonly Random _random = new Random();

        public SneakInHandler(IMissionLoader missionLoader)
        {
            _missionLoader = missionLoader;
        }

        public void AttemptSneakIn(Settlement settlement, Hero assassinationTarget)
        {
            var successChance = GetSneakInChance(Hero.MainHero);

            var sneakInSucceeded = _random.NextDouble() < successChance;

            _missionLoader.LoadCastleAssassination(settlement, assassinationTarget, sneakInSucceeded);
        }

        public float GetSneakInChance(Hero assassin)
        {
            var roguerySkill = assassin.CharacterObject.GetSkillValue(DefaultSkills.Roguery);

            return roguerySkill / 100f;
        }
    }
}
