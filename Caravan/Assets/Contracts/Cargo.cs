namespace Assets.Contracts
{
    public abstract class Cargo
    {
        protected Cargo()
        {
            Type = GetType().Name;
        }

        public virtual decimal WeightPerCount { get; } = 1;
        public decimal Count { get; set; } = 0; //sic!
        public decimal Weight => Count * WeightPerCount;

        public string Type { get; }
    }
}