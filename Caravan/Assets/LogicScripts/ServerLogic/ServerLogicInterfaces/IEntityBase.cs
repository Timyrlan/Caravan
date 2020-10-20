namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface IEntityBase : IGuidEntity
    {
        string Type { get; set; }
    }
}