namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface IProduceIngredient : IEntityBase
    {
        ICargo Cargo { get; set; }
    }
}