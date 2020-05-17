using OsbarMods.Assassinate.Extensions;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace OsbarMods.Assassinate.Missions
{
    public class AssassinationCaughtByGuardsLogic : MissionLogic
    {
        private readonly ILocationCharacterProvider _locationCharacterProvider;
        private readonly Settlement _settlement;
        private readonly Hero _assassinationTarget;

        private MissionAgentHandler _missionAgentHandler;
        private readonly ConversationManager _conversationManager;

        private readonly List<Agent> _spawnedGuards = new List<Agent>();

        private bool _conversationStarted = false;

        public const int GuardsBribeAmount = 100000;

        public AssassinationCaughtByGuardsLogic(ILocationCharacterProvider locationCharacterProvider, ConversationManager conversationManager, Settlement settlement, Hero assassinationTarget)
        {
            _locationCharacterProvider = locationCharacterProvider;
            _conversationManager = conversationManager;
            _settlement = settlement;
            _assassinationTarget = assassinationTarget;
        }

        public override void OnBehaviourInitialize()
        {
            _missionAgentHandler = Mission.GetMissionBehaviour<MissionAgentHandler>();
        }

        public override void OnMissionTick(float dt)
        {
            if (_conversationStarted || _conversationManager.Handler == null) return;

            SpawnGuards();
            _conversationStarted = true;
        }

        private void SpawnGuards()
        {
            var troop = _assassinationTarget.Clan.IsMinorFaction ? _assassinationTarget.Clan.BasicTroop : _settlement.Culture.EliteBasicTroop;

            var guardTiers = GetGuardTiers(_assassinationTarget);

            var numberOfGuards = _assassinationTarget.Clan.IsMinorFaction ? 2 : 3;

            var characterList = Enumerable.Range(0, numberOfGuards)
                .Select(i =>
                {
                    var guard = _locationCharacterProvider.GetRandomFromTroopTree(troop, guardTiers.Item1, guardTiers.Item2);

                    guard.CharacterRelation = LocationCharacter.CharacterRelations.Enemy;

                    return guard;
                });

            foreach (var character in characterList)
            {
                if (IsAlreadySpawned(character.AgentOrigin)) continue;

                CampaignMission.Current.Location.AddCharacter(character);

                var spawnedAgent = _missionAgentHandler.SpawnLocationCharacter(character, false);

                _spawnedGuards.Add(spawnedAgent);
            }

            foreach (Agent guard in _spawnedGuards)
            {
                _missionAgentHandler.SimulateAgent(guard);

                var firstGuard = _spawnedGuards.First();

                if (guard == firstGuard)
                {
                    TeleportAgentToPlayer(firstGuard, true);
                }
                else
                {
                    TeleportAgentBehindAgent(guard, firstGuard);
                }
            }

            CampaignEventDispatcher.Instance.LocationCharactersSimulated();

            Mission.GetMissionBehaviour<AssassinationConversationLogic>().StartGuardConversation(_spawnedGuards.First(), CanBribe(_assassinationTarget), DoBribe);
        }

        private bool CanBribe(Hero assassinationTarget)
        {
            return !assassinationTarget.IsFactionLeader || _assassinationTarget.StringId != assassinationTarget.Clan.Leader.StringId;
        }

        private Tuple<int, int> GetGuardTiers(Hero assassinationTarget)
        {
            if (assassinationTarget.IsFactionLeader) return new Tuple<int, int>(5, 6);

            if(assassinationTarget.IsClanLeader()) return new Tuple<int, int>(3, 4);

            return new Tuple<int, int>(2, 3);
        }

        private void DoBribe()
        {
            Hero.MainHero.ChangeHeroGold(-GuardsBribeAmount);

            GameTexts.SetVariable("GOLD_AMOUNT", GuardsBribeAmount);

            var message = new InformationMessage(GameTexts.FindText("str_gold_removed_with_icon", null).ToString(), "event:/ui/notification/coins_negative");

            InformationManager.DisplayMessage(message);

            Mission.GetMissionBehaviour<AssassinationOutcomeLogic>().OnGuardsAcceptedBribe();
        }

        private bool IsAlreadySpawned(IAgentOriginBase agentOrigin)
        {
            return Mission.Current != null && Mission.Current.Agents.Any((Agent x) => x.Origin == agentOrigin);
        }

        private void TeleportAgentToPlayer(Agent agentToTeleport, bool spawnOpposite)
        {
            var referenceAgent = Agent.Main;

            Vec3 vec3_2 = referenceAgent.Position + referenceAgent.LookDirection.NormalizedCopy() * 4f;
            Vec3 vec3_3;

            if (spawnOpposite)
            {
                vec3_3 = vec3_2;
                vec3_3.z = base.Mission.Scene.GetGroundHeightAtPosition(vec3_3, BodyFlags.CommonCollisionExcludeFlags, true);
            }
            else
            {
                vec3_3 = Mission.Current.GetRandomPositionAroundPoint(referenceAgent.Position, 2f, 4f, true);
                vec3_3.z = base.Mission.Scene.GetGroundHeightAtPosition(vec3_3, BodyFlags.CommonCollisionExcludeFlags, true);
            }

            WorldFrame worldFrame = new WorldFrame(referenceAgent.Frame.rotation, new WorldPosition(base.Mission.Scene, referenceAgent.Frame.origin));
            Vec3 vec3_4 = new Vec3(worldFrame.Origin.AsVec2 - vec3_3.AsVec2, 0f, -1f);

            agentToTeleport.LookDirection = vec3_4.NormalizedCopy();
            agentToTeleport.TeleportToPosition(vec3_3);
        }

        private void TeleportAgentBehindAgent(Agent agentToTeleport, Agent agentToTeleportBehind)
        {
            Vec3 behindPosition = agentToTeleportBehind.Position - agentToTeleportBehind.LookDirection.NormalizedCopy() * 4f;

            var randomPointAroundBehindPosition = Mission.Current.GetRandomPositionAroundPoint(behindPosition, 2f, 4f, true);
            randomPointAroundBehindPosition.z = base.Mission.Scene.GetGroundHeightAtPosition(randomPointAroundBehindPosition, BodyFlags.CommonCollisionExcludeFlags, true);

            agentToTeleport.TeleportToPosition(randomPointAroundBehindPosition);
        }
    }
}
