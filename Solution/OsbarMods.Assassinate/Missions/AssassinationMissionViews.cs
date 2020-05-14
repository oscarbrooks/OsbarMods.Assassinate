using SandBox.View;
using SandBox.View.Missions;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using TaleWorlds.MountAndBlade.View.Missions;

namespace OsbarMods.Assassinate.Missions
{
    [ViewCreatorModule]
    public static class AssassinationMissionViews
    {
        [ViewMethod("CastleAssassination")]
        public static MissionView[] CreateCastleAssassinationView(Mission mission)
        {
            return new MissionView[]
            {
				new CampaignMissionView(),
				new ConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(),
				ViewCreator.CreateOptionsUIHandler(),
				new MissionSingleplayerUIHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				new MusicSilencedMissionView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateBoardGameView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionSettlementPrepareView()
			};
        }
    }
}
