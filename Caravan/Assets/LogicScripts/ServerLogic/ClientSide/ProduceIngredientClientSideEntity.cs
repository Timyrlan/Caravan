using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class ProduceIngredientClientSideEntity : ClientSideEntityBase, IProduceIngredient
    {
        public ICargo Cargo { get; set; }
    }
}