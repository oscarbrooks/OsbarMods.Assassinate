using OsbarMods.Assassinate.Missions;
using SimpleInjector;

namespace OsbarMods.Assassinate
{
    public static class DependencyInjection
    {
        public static Container Configure(this Container container)
        {
            container.Register<ISneakInHandler, SneakInHandler>();
            container.Register<IEncounterProvider, EncounterProvider>();
            container.Register<IMissionLoader, MissionLoader>();
            container.Register<IDialogProvider, DialogProvider>();
            container.Register<ILocationCharacterProvider, LocationCharacterProvider>();

            container.RegisterSingleton<IAssassinationHistoryService, AssassinationHistoryService>();
            container.RegisterSingleton<IMissionOutcomeHandler, MissionOutcomeHandler>();
            container.RegisterSingleton<IAssassinationActions, AssassinationActions>();

            return container;
        }
    }
}
