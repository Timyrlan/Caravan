namespace Assets.Contracts.Dto
{
    public abstract class CargoDto : DtoBase
    {
        public decimal WeightPerCount { get; set; }
        public decimal Count { get; set; }
        public decimal Weight { get; set; }
    }
}