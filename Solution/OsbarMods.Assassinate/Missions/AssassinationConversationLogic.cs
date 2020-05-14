using SandBox;
using SandBox.Source.Missions.Handlers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace OsbarMods.Assassinate.Missions
{
    public class AssassinationConversationLogic : MissionLogic
    {
        private readonly IDialogProvider _dialogProvider;
        private readonly ConversationManager _conversationManager;

        private Action _conversationOutcome;

        /// <summary>
        /// The character to talk to
        /// </summary>
        private readonly CharacterObject _conversationCharacter;

        private readonly bool _sneakInSuccessful;

        public AssassinationConversationLogic(IDialogProvider dialogProvider, ConversationManager conversationManager, CharacterObject conversationCharacter, bool sneakInSuccessful)
        {
            _dialogProvider = dialogProvider;
            _conversationManager = conversationManager;
            _conversationCharacter = conversationCharacter;

            _sneakInSuccessful = sneakInSuccessful;
        }

        public override void EarlyStart()
        {
            if (_sneakInSuccessful)
            {
                var dialogFlow = _dialogProvider.GetAssassinationDialogFlow(Hero.MainHero, _conversationCharacter.HeroObject, OnFightStart, OnPlayerCancelLeave);

                _conversationManager.AddDialogFlow(dialogFlow, this);

                _conversationManager.ConversationEnd += OnConversationEnd;
            }
        }

        protected override void OnEndMission()
        {
            _conversationManager.RemoveRelatedLines(this);
        }

        public void StartGuardConversation(Agent guardToTalkTo, bool canBribe, Action onBribeStart)
        {
            var dialogFlow = _dialogProvider.GetCaughtByGuardsDialogFlow(_conversationCharacter.HeroObject, canBribe, OnFightStart, onBribeStart);

            _conversationManager.AddDialogFlow(dialogFlow, this);

            _conversationManager.ConversationEnd += OnConversationEnd;

            Mission.GetMissionBehaviour<MissionConversationHandler>().StartConversation(guardToTalkTo, false);
        }

        private void OnPlayerCancelLeave()
        {
            _conversationOutcome = () =>
            {
                Mission.GetMissionBehaviour<AssassinationOutcomeLogic>().DoEndMissionUpdates();
                Mission.EndMission();
            };
        }

        private void OnFightStart()
        {
            _conversationOutcome = () =>
            {
                var playerSideAgents = Mission.Agents.Where(a => a.IsPlayerControlled).ToList();
                var enemySideAgents = Mission.Agents.Where(a => !a.IsPlayerControlled).ToList();

                foreach (var agent in enemySideAgents)
                {
                    agent.GetComponent<CampaignAgentComponent>().AgentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>();
                }

                var fightHandler = Mission.GetMissionBehaviour<MissionFightHandler>();

                fightHandler.StartCustomFight(
                    playerSideAgents,
                    enemySideAgents,
                    false,
                    false,
                    false,
                    null,
                    false
                );
            };
        }

        private void OnConversationEnd()
        {
            _conversationManager.ConversationEnd -= OnConversationEnd;

            _conversationOutcome?.Invoke();
        }
    }
}
