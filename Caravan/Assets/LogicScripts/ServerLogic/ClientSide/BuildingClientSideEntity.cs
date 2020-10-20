using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class BuildingClientSideEntity : ClientSideEntityBase, IBuilding
    {
        public ICollectionWrapper<ICargo> Cargos { get; set; } = new ClientSideCollectionWrapper<ICargo, CargoClientSideEntity>();
    }
}