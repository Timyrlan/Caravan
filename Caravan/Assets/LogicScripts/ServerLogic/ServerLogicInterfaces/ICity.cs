namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface ICity : IEntityBase
    {
        float Size { get; set; }
        float X { get; set; }
        float Y { get; set; }
        string Name { get; set; }
    }
}