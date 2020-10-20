using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public class CityClientSideEntity : ClientSideEntityBase, ICity
    {
        public float Size { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
    }
}