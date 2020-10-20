using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class BraminClientSideEntity : ClientSideEntityBase, IBramin
    {
        public ICollectionWrapper<ICargo> Cargos { get; } = new ClientSideCollectionWrapper<ICargo, CargoClientSideEntity>();
        public long Age { get; set; }
    }
}