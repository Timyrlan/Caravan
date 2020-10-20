using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.Processors
{
    public class BraminProcessor : ProcessorBase<IBramin>
    {
        public override void Process(IBramin entity)
        {
            entity.Age++;
        }
    }
}