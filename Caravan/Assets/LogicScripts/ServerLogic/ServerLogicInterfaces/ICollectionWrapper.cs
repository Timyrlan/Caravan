using System.Collections.Generic;

namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface ICollectionWrapper<TClass> where TClass : IGuidEntity
    {
        IEnumerable<TClass> Collection { get; }
        TClass GetNew();
        void Remove(string guid);
        void AddToCollection(TClass entity);

        /// <summary>
        ///     Только для client-side
        /// </summary>
        void LoadCollection(TClass[] collection);
    }
}