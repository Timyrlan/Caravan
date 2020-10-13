using System.Collections.Generic;
using Assets.LogicScripts.DifferentCargos;

namespace Assets.LogicScripts.Buildings.Factories
{
    public class SaltWaterWell : ProcessBuilding
    {
        public override Produce Produce { get; } = new Produce
        {
            To = new List<ProduceIngredient>
            {
                new ProduceIngredient {Cargo = new SaltWater(), Value = 1}
            },
            Speed = 0.01m
        };

        public override Dictionary<string, decimal> CanStoreCargos { get; set; } = new Dictionary<string, decimal>
        {
            {nameof(SaltWater), 10}
        };
    }
}