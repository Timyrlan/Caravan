using CrvService.Contracts.Entities;

namespace Assets.Scripts.World
{
    public static class ToServerMapper
    {
        public static Player Map(Player c)
        {
            return new Player
            {
                Guid = c.Guid,
                IsMoving = c.IsMoving,
                X = c.X,
                Y = c.Y,
                World = new CrvService.Contracts.Entities.World { Guid = c.World.Guid }
            };
        }
    }
}