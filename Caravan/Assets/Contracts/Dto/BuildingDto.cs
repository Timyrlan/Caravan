namespace Assets.Contracts.Dto
{
    public abstract class BuildingDto : DtoBase
    {
        public Cargo[] Cargos { get; set; } = { };
    }
}