using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Cargos;

namespace Assets.Scripts.World.Buildings
{
    public abstract class Building
    {
        public List<Cargo> Cargos { get; set; } = new List<Cargo>();

        public abstract string Type { get; }
        public abstract void Process();

        public virtual void AddCargo(Cargo cargo)
        {
            var cargoToAdd = Cargos.FirstOrDefault(c => c.Type == cargo.Type);
            if (cargoToAdd == null)
            {
                cargoToAdd = (Cargo) Activator.CreateInstance(cargo.GetType());
                Cargos.Add(cargoToAdd);
            }

            cargoToAdd.Weight += cargo.Weight;
        }
    }

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

        public override void Process()
        {
            if (Produce?.From == null || Produce.To == null || !Produce.To.Any()) return;

            //проверяем, что ингридиентов достаточно, чтобы приготовить
            foreach (var produceIngredient in Produce.From)
            {
                var fromCargo = Cargos.FirstOrDefault(c => c.Type == produceIngredient.Cargo.Type);
                if (fromCargo == null || fromCargo.Weight < produceIngredient.Value * Produce.Speed) return;
            }

            //тратим ингридиенты
            foreach (var produceIngredient in Produce.From)
            {
                var fromCargo = Cargos.FirstOrDefault(c => c.Type == produceIngredient.Cargo.Type);
                if (fromCargo == null || fromCargo.Weight < produceIngredient.Value * Produce.Speed) return;
            }

            //создаем ингридиенты
            foreach (var produceIngredient in Produce.To)
            {
                var toCargo = Cargos.FirstOrDefault(c => c.Type == produceIngredient.Cargo.Type);

                if (toCargo == null)
                {
                    toCargo = (Cargo) Activator.CreateInstance(produceIngredient.Cargo.GetType());
                    Cargos.Add(toCargo);
                }


                toCargo.Weight += produceIngredient.Value * Produce.Speed;
            }
        }
    }

    public class Produce
    {
        public List<ProduceIngredient> From { get; set; } = new List<ProduceIngredient>();
        public List<ProduceIngredient> To { get; set; } = new List<ProduceIngredient>();
        public decimal Speed { get; set; }
    }

    public class ProduceIngredient
    {
        public Cargo Cargo { get; set; }
        public decimal Value { get; set; }
    }

    public class SaltEvaporationFactory : Factory
    {
        public override Produce Produce { get; } = new Produce
        {
            From = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new SoltWater(), Value = 1}
            },
            To = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new FreshWater(), Value = 0.95m},
                new ProduceIngredient {Cargo = new Solt(), Value = 0.05m}
            },
            Speed = 1
        };


        public override string Type => nameof(SaltEvaporationFactory);
    }
}