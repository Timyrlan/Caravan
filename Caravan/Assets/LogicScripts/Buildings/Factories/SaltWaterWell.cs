using System.Collections.Generic;
using Assets.LogicScripts.DifferentCargos;

namespace Assets.LogicScripts.Buildings.Factories
{
    public class SaltWaterWell : Factory
    {
        public override Produce Produce { get; } = new Produce
        {
            To = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new FreshWater(), Value = 1}
            },
            Speed = 1
        };
    }

    public class LivingHouse : Factory
    {
        public override Produce Produce { get; } = new Produce
        {
            From = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new FreshWater(), Value = 1}
            },
            Speed = 0.1m
        };
    }
}