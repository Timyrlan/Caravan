using System;

namespace Assets.Contracts.Dto
{
    public class WorldDto : DtoBase
    {
        public DateTime WorldDate { get; set; }
        public CityDto[] Cities { get; set; }
        public BuildingDto Buildings { get; set; }

        public PlayerDto Player { get; set; }
    }
}