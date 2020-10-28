using System;
using System.Collections;
using CrvService.Shared.Contracts.Entities;

namespace Assets.Scripts.World
{
    public interface ICaravanServerConnector
    {
        IEnumerator ProcessWorld(IProcessWorldRequest request, Action<IProcessWorldRequest, IProcessWorldResponse> callback);
    }
}