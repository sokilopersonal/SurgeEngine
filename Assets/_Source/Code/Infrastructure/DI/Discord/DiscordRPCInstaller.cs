using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI.Discord
{
    public class DiscordRPCInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SurgeRPC>().FromNew().AsSingle().NonLazy();
        }
    }
}