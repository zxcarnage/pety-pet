namespace Ecs.Core.Utils.TickInterfaces
{
    public interface ITickable : IController
    {
        public void Tick();
    }
}