using System;
using System.Collections;
using CrvService.Shared.Contracts.Entities;
using CrvService.Shared.Logic;
using CrvService.Shared.Logic.ClientSide;
using CrvService.Shared.Logic.ClientSide.Server;

namespace Assets.Scripts.World
{
    public class CaravanServerConnectorClientSide : ICaravanServerConnector
    {
        public CaravanServerConnectorClientSide()
        {
            var worldRepository = new WorldRepositoryClientSide();
            var playerRepository = new PlayerRepositoryClientSide();
            var newInstanceFactory = new NewInstanceFactoryClientSide(worldRepository, playerRepository);
            var processorsProvider = new ProcessorsProvider(newInstanceFactory);
            var newWorldGenerator = new NewWorldGenerator(newInstanceFactory, playerRepository);
            CaravanServer = new CaravanServerClientSide(processorsProvider, newInstanceFactory, newWorldGenerator, playerRepository, worldRepository);
        }

        private ICaravanServer CaravanServer { get; }

        public IEnumerator ProcessWorld(IProcessWorldRequest request, Action<IProcessWorldRequest, IProcessWorldResponse> callback)
        {
            var response = CaravanServer.ProcessWorld(request);
            yield return null;
            callback(request, response);
        }

        public IEnumerator GetNewWorld(IGetNewWorldRequest request, Action<IGetNewWorldRequest, IProcessWorldResponse> callback)
        {
            var response = CaravanServer.GetNewWorld(request);
            yield return null;
            callback(request, response);
        }
    }
}