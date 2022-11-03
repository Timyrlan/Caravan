using CrvService.Contracts.Entities;

public static class ToServerMapper
{
    public static Player Map(Player c)
    {
        return new Player
        {
            Guid = c.Guid,
            IsMoving = c.IsMoving,
            X = c.X,
            Y = c.Y
        };
    }
}