using System.Collections.Generic;

namespace Assets.Scripts.World.Cargos
{
    public abstract class Cargo
    {
        public decimal Weight { get; set; }
        public abstract string Type { get; }
    }

    public class Solt : Cargo
    {
        public override string Type => nameof(Solt);
    }

    public class FreshWater : Cargo
    {
        public override string Type => nameof(FreshWater);
    }

    public class SoltWater : Cargo
    {
        public override string Type => nameof(SoltWater);
    }

    public class PricesMap
    {
        public Dictionary<string, decimal> Costs { get; set; } = new Dictionary<string, decimal>();
    }
}