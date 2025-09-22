namespace Ecs.Core.Utils.TickInterfaces
{
    public interface IFixedTickable : IController
    {
        public void FixedTick();
    }
}