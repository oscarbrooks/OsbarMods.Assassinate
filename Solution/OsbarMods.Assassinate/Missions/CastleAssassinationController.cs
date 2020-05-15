using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace OsbarMods.Assassinate.Missions
{
    public class CastleAssassinationController : MissionLogic
    {
        private readonly ILocationCharacterProvider _locationCharacterProvider;
        
        private MissionAgentHandler _missionAgentHandler;

        private readonly Hero _assassinationTarget;
        private Agent _assassinationTargetAgent;
        private readonly bool _sneakInSuccessful;

        public CastleAssassinationController(ILocationCharacterProvider locationCharacterProvider, Hero assassinationTarget, bool sneakInSuccessful)
        {
            _locationCharacterProvider = locationCharacterProvider;
            _assassinationTarget = assassinationTarget;
            _sneakInSuccessful = sneakInSuccessful;
        }

        public override void OnBehaviourInitialize()
        {
            _missionAgentHandler = this.Mission.GetMissionBehaviour<MissionAgentHandler>();

            Mission.IsFriendlyMission = false;
        }

        public override void AfterStart()
        {
            var mission = this.Mission;

            if (GameNetwork.IsClientOrReplay) return;

            mission.SetMissionMode(MissionMode.StartUp, true);

            mission.IsInventoryAccessible = false;
            mission.IsQuestScreenAccessible = false;
            mission.DoesMissionRequireCivilianEquipment = false;

            _missionAgentHandler.SpawnPlayer(mission.DoesMissionRequireCivilianEquipment, true, false, true, false, false, string.Empty);

            SpawnAssassinationTarget(_assassinationTarget);
        }

        public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
        {
            var mode = Mission.Mode;

            if (mode == MissionMode.Battle || mode == MissionMode.Duel || mode == MissionMode.Conversation) return false;

            return otherAgent.IsActive() && otherAgent == _assassinationTargetAgent;
        }

        private void SpawnAssassinationTarget(Hero assassinationTarget)
        {
            var locationCharacter = _locationCharacterProvider.GetFromCharacterObject(assassinationTarget.CharacterObject, false);

            if (!_sneakInSuccessful) locationCharacter.CharacterRelation = LocationCharacter.CharacterRelations.Enemy;

            CampaignMission.Current.Location.AddCharacter(locationCharacter);

            _assassinationTargetAgent = _missionAgentHandler.SpawnLocationCharacter(locationCharacter);
        }
    }
}
