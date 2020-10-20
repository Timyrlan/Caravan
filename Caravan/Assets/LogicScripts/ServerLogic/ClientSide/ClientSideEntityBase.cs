using System;
using Assets.LogicScripts.ServerLogic.ServerLogicInterfaces;

namespace Assets.LogicScripts.ServerLogic.ClientSide
{
    public abstract class ClientSideEntityBase : IEntityBase
    {
        protected ClientSideEntityBase()
        {
            Type = GetType().Name.Replace("ClientSideEntity", string.Empty);
            Guid = new Guid().ToString();
        }

        public string Guid { get; set; }
        public string Type { get; set; }
    }
}