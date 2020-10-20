using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;

namespace Assets.LogicScripts.Buildings
{
    public abstract class Building : IGameProcessibleObject, IGameCargoContainer//, ICargoStoreObject
    {
        protected Building()
        {
            Type = GetType().Name;
        }

        public string Type { get; }

        public virtual Dictionary<string, decimal> CanStoreCargos { get; set; } = new Dictionary<string, decimal>();

        public virtual decimal CanAddCargoMore(string type)
        {
            if (!CanStoreCargos.TryGetValue(type, out var canStoreAtAll)) return 0;

            var count = GetFullCargoCount(type);

            var result = canStoreAtAll - count;

            return result > 0 ? result : 0;
        }

        public virtual bool AddCargo(Cargo cargo)
        {
            if (CanAddCargoMore(cargo.Type) < cargo.Count) return false;


            var existedCargo = Cargos.FirstOrDefault(c => c.Type == cargo.Type);

            if (existedCargo != null)
                existedCargo.Count += cargo.Count;
            else
                Cargos.Add(cargo);

            return true;
        }

        public List<Cargo> Cargos { get; set; } = new List<Cargo>();

        public abstract void Process();

        public decimal GetFullCargoCount(string type)
        {
            return Cargos.Where(c => c.Type == type).Sum(c => c.Count);
        }

        public Cargo GetCargoOfType(string type)
        {
            return Cargos.FirstOrDefault(c => c.Type == type);
        }

        public Cargo GetCargoOfType<TType>()
        {
            return Cargos.FirstOrDefault(c => c.Type == typeof(TType).Name);
        }
    }
}