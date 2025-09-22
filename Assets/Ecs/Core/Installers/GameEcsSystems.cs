using Reflex.Core;

namespace Ecs.Core.Installers
{
    public static class GameEcsSystems
    {
        public static void Install(ContainerBuilder container){
            Urgent(container);
            High(container);
            Normal(container);
            Low(container);
        }

        private static void Urgent(ContainerBuilder container)
        {

        }

        private static void High(ContainerBuilder container)
        {

        }

        private static void Normal(ContainerBuilder container)
        {

        }

        private static void Low(ContainerBuilder container)
        {
            
        }
    }
}