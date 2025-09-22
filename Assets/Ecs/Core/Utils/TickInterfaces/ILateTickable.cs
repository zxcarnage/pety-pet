namespace Ecs.Core.Utils.TickInterfaces
{
    public interface ILateTickable : IController
    {
        public void LateTick();
    }
}