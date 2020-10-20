namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface IBuilding : IEntityBase
    {
        ICollectionWrapper<ICargo> Cargos { get; set; }
    }
}