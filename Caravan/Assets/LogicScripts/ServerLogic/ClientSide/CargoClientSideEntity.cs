using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class CargoClientSideEntity : ClientSideEntityBase, ICargo
    {
        public decimal WeightPerCount { get; set; }
        public decimal Count { get; set; }
        public decimal Weight { get; set; }
    }
}