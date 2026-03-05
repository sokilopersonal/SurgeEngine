using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.DI
{
    public class DebugInfoInstaller : MonoInstaller
    {
        [SerializeField] private DebugWindow windowPrefab;

        public override void InstallBindings()
        {
            Container.InstantiatePrefabForComponent<DebugWindow>(windowPrefab);
        }
    }
}