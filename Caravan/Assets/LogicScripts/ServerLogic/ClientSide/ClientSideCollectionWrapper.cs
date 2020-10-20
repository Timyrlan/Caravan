using System.Collections.Generic;
using System.Linq;
using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class ClientSideCollectionWrapper<TInterface, TImplementation> : ICollectionWrapper<TInterface>
        where TInterface : IEntityBase
        where TImplementation : TInterface, new()
    {
        private List<TInterface> PrivateCollection { get; set; } = new List<TInterface>();

        public IEnumerable<TInterface> Collection => PrivateCollection;

        public TInterface GetNew()
        {
            return new TImplementation();
        }

        public void Remove(string guid)
        {
            var toRemove = PrivateCollection.FirstOrDefault(c => c.Guid == guid);

            if (toRemove != null) PrivateCollection.Remove(toRemove);
        }

        public void AddToCollection(TInterface entity)
        {
            PrivateCollection.Add(entity);
        }

        public void LoadCollection(TInterface[] collection)
        {
            PrivateCollection = new List<TInterface>();
            PrivateCollection.AddRange(collection);
        }
    }
}