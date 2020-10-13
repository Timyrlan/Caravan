using System.Collections.Generic;

namespace Assets.Contracts
{
    public interface ICargoStoreObject
    {
        List<Cargo> Cargos { get; }

        decimal CanAddCargoMore(string type);

        bool AddCargo(Cargo cargo);
    }
}