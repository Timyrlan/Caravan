namespace ServerContracts
{
    public class CityContract : WorldStateContractBase
    {
        public float Size { get; set; } = 1;
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
    }
}