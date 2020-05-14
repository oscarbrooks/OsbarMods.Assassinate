using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using SimpleInjector;

namespace OsbarMods.Assassinate
{
    public class Main : MBSubModuleBase
    {
        private static Container _container;

        protected override void OnSubModuleLoad()
        {
            _container = new Container();
            _container.Configure();

            InformationManager.DisplayMessage(new InformationMessage("Assassinate Mod Loaded", new Color(0, 0.5f, 0.5f)));
        }

        /// <summary>
        /// When a new campaign is started, beginning the character creation process.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="starterObject"></param>
        public override void OnCampaignStart(Game game, object starterObject)
        {
            //InformationManager.DisplayMessage(new InformationMessage("OnCampaignStart"));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(_container.GetInstance<AssassinationBehavior>());
            }
        }
    }
}
