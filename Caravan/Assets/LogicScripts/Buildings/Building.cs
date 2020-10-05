using System;
using System.Collections.Generic;
using System.Linq;
using Assets.LogicScripts.Cargos;

namespace Assets.LogicScripts.Buildings
{
    public abstract class Building
    {
        public List<Cargo> Cargos { get; set; } = new List<Cargo>();

        public abstract string Type { get; }

        public Cargo GetCargoOfType(string type)
        {
            return Cargos.FirstOrDefault(c => c.Type == type);
        }

        public Cargo GetCargoOfType<TType>()
        {
            return Cargos.FirstOrDefault(c => c.Type == typeof(TType).Name);
        }

        public abstract void Process();

        public virtual void AddCargo(Cargo cargo)
        {
            var cargoToAdd = Cargos.FirstOrDefault(c => c.Type == cargo.Type);
            if (cargoToAdd == null)
            {
                cargoToAdd = (Cargo) Activator.CreateInstance(cargo.GetType());
                Cargos.Add(cargoToAdd);
            }

            cargoToAdd.Count += cargo.Count;
        }
    }
}