using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace OsbarMods.Assassinate.Missions
{
    public class AssassinationOutcomeLogic : MissionLogic
    {
        private readonly IMissionOutcomeEvents _missionOutcomeHandler;
        private readonly IAssassinationActions _assassinationActions;
        private readonly Settlement _settlement;
        private readonly Hero _assassinationTarget;
        private readonly bool _sneakInSuccessful;

        private bool _fightIsEnded;
        private float _fightEndedTime;

        private const float EndMissionDelayInSeconds = 2;

        private bool _didWinFight;

        private bool _guardsAcceptedBribe = false;

        public AssassinationOutcomeLogic(IMissionOutcomeEvents missionOutcomeHandler, IAssassinationActions assassinationActions, Settlement settlement, Hero assassinationTarget, bool sneakInSuccessful)
        {
            _missionOutcomeHandler = missionOutcomeHandler;
            _assassinationActions = assassinationActions;
            _settlement = settlement;
            _assassinationTarget = assassinationTarget;
            _sneakInSuccessful = sneakInSuccessful;
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, int weaponKind, int currentWeaponUsageIndex)
        {
            if (_fightIsEnded) return;

            // Player is dead
            if (Agent.Main.Health < 1)
            {
                var allEnemiesDead = !Mission.Agents.Any(a => a.Team != Agent.Main.Team && a.Health > 0);
                OnAssassinationFightEnded(allEnemiesDead);
                return;
            }

            if (affectedAgent.IsPlayerControlled) return;

            if (!_sneakInSuccessful && _guardsAcceptedBribe)
            {
                if (affectedAgent.Character.Id == _assassinationTarget.CharacterObject.Id && affectedAgent.Health < 1)
                {
                    OnAssassinationFightEnded(true);
                    return;
                }
            }

            // No enemies left
            if (!Mission.Agents.Any(a => a.Team != Agent.Main.Team && a.Health > 0))
            {
                OnAssassinationFightEnded(true);
            }
        }

        public void OnGuardsAcceptedBribe()
        {
            _guardsAcceptedBribe = true;
        }

        /// <summary>
        /// The updates to the assassination mission, regardless of the outcome
        /// </summary>
        public void DoEndMissionUpdates()
        {
            if (!_assassinationTarget.HasMet) _assassinationTarget.HasMet = true;
        }

        public void OnAssassinationFightEnded(bool didWin)
        {
            DoEndMissionUpdates();

            _didWinFight = didWin;
            _fightIsEnded = true;
            _fightEndedTime = Mission.TimeSpeedTimerElapsedTime;
        }

        public override void OnMissionTick(float dt)
        {
            if (!_fightIsEnded) return;

            if (_fightEndedTime + EndMissionDelayInSeconds > Mission.TimeSpeedTimerElapsedTime) return;

            if (_didWinFight)
            {
                OnTargetAssassinated();
            }
            else
            {
                OnPlayerDefeated();
            }

            Mission.EndMission();
        }

        private void OnTargetAssassinated()
        {
            // don't need to listen for player defeated
            _missionOutcomeHandler.ClearListeners();

            _assassinationActions.ApplySuccessfulAssassination(_settlement, Hero.MainHero, _assassinationTarget);

            CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, args =>
            {
                CampaignEvents.GameMenuOpened.ClearListeners(this);

                Campaign.Current.GameMenuManager.GetLeaveMenuOption(args.MenuContext).RunConsequence(args.MenuContext);
            });
        }

        private void OnPlayerDefeated()
        {

            CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, args =>
            {
                CampaignEvents.GameMenuOpened.ClearListeners(this);

                _missionOutcomeHandler.OnPlayerAssassinationFailed(_settlement, _assassinationTarget);
            });
        }
    }
}
