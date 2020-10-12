using System.Collections.Generic;

namespace Assets.Contracts
{
    public interface IGameCargoContainer
    {
        List<Cargo> Cargos { get; }
    }
}