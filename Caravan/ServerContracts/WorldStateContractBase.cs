namespace ServerContracts
{
    public abstract class WorldStateContractBase
    {
        protected WorldStateContractBase()
        {
            Type = GetType().Name.Replace("Contract", string.Empty);
        }

        public string Guid { get; set; }

        public string Type { get; }
    }
}