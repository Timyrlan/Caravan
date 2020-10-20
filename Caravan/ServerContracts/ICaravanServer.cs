namespace ServerContracts
{
    public interface ICaravanServer
    {
        WorldContract ProcessWorld(string worldGuid, string clientGuid);
    }
}