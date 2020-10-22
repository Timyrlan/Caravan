using CrvService.Contracts.Dto;

namespace Assets.Contracts
{
    public interface ICaravanServer
    {
        WorldDto ProcessWorld(string worldGuid, string clientGuid);
    }
}