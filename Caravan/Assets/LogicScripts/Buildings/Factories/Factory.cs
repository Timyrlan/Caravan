using System;
using System.Linq;
using Assets.LogicScripts.Cargos;

namespace Assets.LogicScripts.Buildings.Factories
{
    public abstract class Factory : Building
    {
        public virtual Produce Produce { get; } = new Produce();

        /// <summary>
        ///     Добавлять можно только тот ресурс, который есть в ингридентах, из которых что-то производится
        /// </summary>
        public override void AddCargo(Cargo cargo)
        {
            if (Produce?.From == null || Produce.From.All(c => c.Cargo.Type != cargo.Type)) return;

            base.AddCargo(cargo);
        }

        public void AddResultCargo(Cargo cargo)
        {
            if (Produce?.To == null || Produce.To.All(c => c.Cargo.Type != cargo.Type)) return;

            base.AddCargo(cargo);
        }

        public override void Process()
        {
            if (Produce?.From == null || Produce.To == null || !Produce.To.Any()) return;

            //проверяем, что ингридиентов достаточно, чтобы приготовить
            foreach (var produceIngredient in Produce.From)
            {
                var fromCargo = Cargos.FirstOrDefault(c => c.Type == produceIngredient.Cargo.Type);
                if (fromCargo == null || fromCargo.Count < produceIngredient.Value * Produce.Speed) return;
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
    }
}