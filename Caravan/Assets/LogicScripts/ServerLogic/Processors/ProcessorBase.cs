using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.Processors
{
    public abstract class ProcessorBase<TEntity> where TEntity : IEntityBase
    {
        public abstract void Process(TEntity entity);
    }
}