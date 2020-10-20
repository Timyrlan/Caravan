using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class ProduceClientSideEntity : ClientSideEntityBase, IProduce
    {
        public ICollectionWrapper<IProduceIngredient> From { get; set; } = new ClientSideCollectionWrapper<IProduceIngredient, ProduceIngredientClientSideEntity>();
        public ICollectionWrapper<IProduceIngredient> To { get; set; } = new ClientSideCollectionWrapper<IProduceIngredient, ProduceIngredientClientSideEntity>();
        public decimal Speed { get; set; }
    }
}