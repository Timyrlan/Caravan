using System.Collections.Generic;

namespace Assets.LogicScripts.Buildings.Factories
{
    public class Produce
    {
        public List<ProduceIngredient> From { get; set; } = new List<ProduceIngredient>();
        public List<ProduceIngredient> To { get; set; } = new List<ProduceIngredient>();
        public decimal Speed { get; set; }
    }
}