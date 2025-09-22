using Ecs.Core.Utils;
using Ecs.Core.Utils.TickInterfaces;
using Reflex.Core;
using Scellecs.Morpeh;
using UnityEngine;

namespace Ecs.Core.Installers
{
    public class GameEcsInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            var world = World.Create();

            containerBuilder.AddSingleton(world, typeof(World));

            GameEcsSystems.Install(containerBuilder);

            containerBuilder.AddSingleton(typeof(SystemLoopHandler), typeof(IController));
        }
    }
}