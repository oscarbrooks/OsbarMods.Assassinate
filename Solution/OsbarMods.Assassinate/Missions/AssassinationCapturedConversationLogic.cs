using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace OsbarMods.Assassinate.Missions
{
    public class AssassinationCapturedConversationLogic : MissionLogic
    {
        private readonly ILocationCharacterProvider _locationCharacterProvider;
        private readonly IDialogProvider _dialogProvider;
        private readonly ConversationManager _conversationManager;

        private MissionAgentHandler _missionAgentHandler;

        private readonly Hero _assassinationTarget;
        private Agent _assassinationTargetAgent;

        private bool _isReady;
        private float _readyAt;

        public AssassinationCapturedConversationLogic(ILocationCharacterProvider locationCharacterProvider, IDialogProvider dialogProvider, ConversationManager conversationManager, Hero assassinationTarget)
        {
            _locationCharacterProvider = locationCharacterProvider;
            _dialogProvider = dialogProvider;
            _conversationManager = conversationManager;
            _assassinationTarget = assassinationTarget;
        }

        public override void OnBehaviourInitialize()
        {
            _missionAgentHandler = this.Mission.GetMissionBehaviour<MissionAgentHandler>();
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

            SetUpConversation();
        }

        public override void OnMissionTick(float dt)
        {
            if (!_isReady || _readyAt + 5 > Campaign.CurrentTime) return;

            Mission.GetMissionBehaviour<MissionConversationHandler>().StartConversation(_assassinationTargetAgent, false, true);
        }

        private void SpawnAssassinationTarget(Hero assassinationTarget)
        {
            var locationCharacter = _locationCharacterProvider.GetFromCharacterObject(assassinationTarget.CharacterObject, false);

            CampaignMission.Current.Location.AddCharacter(locationCharacter);

            _assassinationTargetAgent = _missionAgentHandler.SpawnLocationCharacter(locationCharacter);
        }

        private void SetUpConversation()
        {
            var dialogFlow = _dialogProvider.GetCapturedDialogFlow(Hero.MainHero, _assassinationTarget);

            _conversationManager.AddDialogFlow(dialogFlow, this);

            _conversationManager.ConversationEnd += OnConversationEnd;

            //Mission.GetMissionBehaviour<MissionConversationHandler>().StartConversation(_assassinationTargetAgent, false, true);
        }

        private void OnConversationEnd()
        {
            _conversationManager.ConversationEnd -= OnConversationEnd;

            _conversationManager.RemoveRelatedLines(this);

            Mission.EndMission();
        }
    }
}
