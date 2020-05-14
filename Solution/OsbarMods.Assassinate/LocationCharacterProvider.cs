using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace OsbarMods.Assassinate
{
    public class LocationCharacterProvider : ILocationCharacterProvider
    {
        public LocationCharacter GetFromCharacterObject(CharacterObject characterObject, bool useCivilianEquipment)
        {
            var origin = new SimpleAgentOrigin(characterObject);

            var agentData = new AgentData(origin);

            var actionSetCode = ActionSetCode.HumanLordActionSet;

            return new LocationCharacter(agentData, args => { }, string.Empty, false, LocationCharacter.CharacterRelations.Neutral, actionSetCode, useCivilianEquipment);
        }

        public LocationCharacter GetRandomFromTroopTree(CharacterObject basicTroop, int minTier, int maxTier)
        {
            minTier = Math.Max(1, minTier);

            var flattenedTroops = Flatten(basicTroop.UpgradeTargets)
                .Concat(new[] { basicTroop })
                .Where(t => t.Tier >= minTier && t.Tier <= maxTier)
                .OrderBy(t => t.Tier);

            return GetFromCharacterObject(flattenedTroops.GetRandomElement(), false);
        }

        private IEnumerable<CharacterObject> Flatten(IEnumerable<CharacterObject> troopUpgrades)
        {
            return troopUpgrades
                .SelectMany(t => Flatten(t.UpgradeTargets ?? Enumerable.Empty<CharacterObject>()))
                .Concat(troopUpgrades);
        }
    }
}
