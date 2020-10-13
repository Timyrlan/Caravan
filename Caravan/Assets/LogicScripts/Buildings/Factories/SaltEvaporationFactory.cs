using System.Collections.Generic;
using Assets.LogicScripts.DifferentCargos;

namespace Assets.LogicScripts.Buildings.Factories
{
    public class SaltEvaporationFactory : ProcessBuilding
    {
        public override Produce Produce { get; } = new Produce
        {
            From = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new SaltWater(), Value = 1}
            },
            To = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new FreshWater(), Value = 0.95m},
                new ProduceIngredient {Cargo = new Salt(), Value = 0.05m}
            },
            Speed = 1
        };

        public override Dictionary<string, decimal> CanStoreCargos { get; set; } = new Dictionary<string, decimal>
        {
            {nameof(SaltWater), 100},
            {nameof(FreshWater), 100},
            {nameof(Salt), 100}
        };
    }
}