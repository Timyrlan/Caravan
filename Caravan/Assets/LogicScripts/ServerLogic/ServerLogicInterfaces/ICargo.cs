namespace Assets.LogicScripts.ServerLogic.ServerLogicInterfaces
{
    public interface ICargo : IEntityBase
    {
        decimal WeightPerCount { get; set; }
        decimal Count { get; set; }
        decimal Weight { get; set; }
    }
}