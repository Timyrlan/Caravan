namespace Assets.Contracts.Dto
{
    public class BraminDto : DtoBase
    {
        public CargoDto[] Cargos { get; set; } = { };
        public long Age { get; set; }
    }
}