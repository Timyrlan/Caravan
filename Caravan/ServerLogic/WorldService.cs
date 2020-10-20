using System;
using System.Collections.Generic;
using ServerContracts;

namespace ServerLogic
{
    public class ClientSideWorldService : ICaravanServer
    {
        private Dictionary<string, WorldContract> Worlds { get; } = new Dictionary<string, WorldContract>();


        public WorldContract GetWorld(string worldGuid, string clientGuid)
        {
            if (!Worlds.TryGetValue(worldGuid, out var world))
            {
                world = new WorldContract {Guid = Guid.NewGuid().ToString()};
                Worlds.Add(world.Guid, world);
            }
        }

        private WorldContract MapWorldToClient()
    }
}