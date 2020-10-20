namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface IBramin : IEntityBase
    {
        ICollectionWrapper<ICargo> Cargos { get; }
        long Age { get; set; }
    }
}