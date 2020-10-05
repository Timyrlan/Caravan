using System.Collections.Generic;

namespace Assets.LogicScripts.Cargos
{
    public abstract class Cargo
    {
        public virtual decimal WeightPerCount { get; } = 1;
        public decimal Count { get; set; } = 0;//sic!
        public decimal Weight => Count * WeightPerCount;
        public abstract string Type { get; }
    }

    public class Salt : Cargo
    {
        public override string Type => nameof(Salt);
    }

    public class FreshWater : Cargo
    {
        public override string Type => nameof(FreshWater);
    }

    public class SaltWater : Cargo
    {
        public override string Type => nameof(SaltWater);
    }

    public class PricesMap
    {
        public Dictionary<string, decimal> Costs { get; set; } = new Dictionary<string, decimal>();
    }
}