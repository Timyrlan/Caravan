using Assets.Contracts;

namespace ServerContracts
{
    public abstract class BuildingContract : WorldStateContractBase
    {
        protected BuildingContract()
        {
            Type = GetType().Name;
        }

        public string Type { get; }


        public Cargo[] Cargos { get; set; } = { };
    }


}