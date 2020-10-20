namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface IProduce : IEntityBase
    {
        ICollectionWrapper<IProduceIngredient> From { get; set; }
        ICollectionWrapper<IProduceIngredient> To { get; set; }
        decimal Speed { get; set; }
    }
}