using System;
using System.Collections.Generic;

namespace ServerContracts
{
    public class WorldContract : WorldStateContractBase
    {
        public DateTime WorldDate { get; set; } = new DateTime(3000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private List<CityContract> Cities { get; set; }
        private List<BuildingContract> Buildings { get; set; }
    }

    public class PlayerContract: WorldStateContractBase
    {

    }

    public class BraminContract : WorldStateContractBase
    {
        public const int MaxBraminBagWeight = 400;

        public List<CargoContract> Cargos { get; set; } = new List<CargoContract>();
        
        public long Age { get; set; }

        public void Process()
        {
            Age++;
        }
    }


    public abstract class CargoContract
    {
        protected CargoContract()
        {
            Type = GetType().Name;
        }

        public virtual decimal WeightPerCount { get; } = 1;
        public decimal Count { get; set; } = 0; //sic!
        public decimal Weight => Count * WeightPerCount;

        public string Type { get; }
    }
}