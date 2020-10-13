using System;
using System.Linq;
using Assets.Contracts;

namespace Assets.LogicScripts.Buildings.Factories
{
    public abstract class ProcessBuilding : Building
    {
        public virtual Produce Produce { get; } = new Produce();

        public override void Process()
        {
            if (Produce?.From == null || Produce.To == null || !Produce.From.Any() && !Produce.To.Any()) return;

            //проверяем, что ингридиентов достаточно, чтобы приготовить и достао
            foreach (var produceIngredient in Produce.From)
            {
                var fromCargo = Cargos.FirstOrDefault(c => c.Type == produceIngredient.Cargo.Type);

                if (fromCargo == null) return;

                if (fromCargo.Count < produceIngredient.Value * Produce.Speed) return;
            }

            //проверяем, что место достаточно, чтобы хранить ингридиенты
            foreach (var produceIngredient in Produce.To)
            {
                var count = produceIngredient.Value * Produce.Speed;
                if (count > CanAddCargoMore(produceIngredient.Cargo.Type)) return;
            }

            //тратим ингридиенты
            foreach (var produceIngredient in Produce.From)
            {
                var fromCargo = Cargos.First(c => c.Type == produceIngredient.Cargo.Type);
                fromCargo.Count -= produceIngredient.Value * Produce.Speed;
            }

            //создаем ингридиенты
            foreach (var produceIngredient in Produce.To)
            {
                var cargoToAdd = (Cargo) Activator.CreateInstance(produceIngredient.Cargo.GetType());
                cargoToAdd.Count = produceIngredient.Value * Produce.Speed;

                AddResultCargo(cargoToAdd);
            }
        }

        /// <summary>
        ///     Добавлять можно только тот ресурс, который есть в ингридентах, из которых что-то производится
        /// </summary>
        public override bool AddCargo(Cargo cargo)
        {
            if (Produce?.From == null || Produce.From.All(c => c.Cargo.Type != cargo.Type)) return false;

            return base.AddCargo(cargo);
        }


        public bool AddResultCargo(Cargo cargo)
        {
            if (Produce?.To == null || Produce.To.All(c => c.Cargo.Type != cargo.Type)) return false;

            return base.AddCargo(cargo);
        }
    }
}