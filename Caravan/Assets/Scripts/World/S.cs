using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using CrvService.Contracts.Entities;
using CrvService.Contracts.Entities.Commands.ClientCommands.Base;
using CrvService.Contracts.Entities.Commands.ServerCommands.Base;

#pragma warning disable CS0649
public static class S
{
    public static CaravanServerHttpConnector CaravanServerHttpConnector { get; set; }

    public static Player Player { get; set; }
    public static List<ClientCommand> ClientCommands { get; set; } = new();
    public static List<ServerCommand> ServerCommands { get; } = new();

    public static Guid EnterCityGuid { get; set; }

    public static SceneLoaded SceneLoaded { get; set; }
}

public enum SceneLoaded
{
    World = 0,
    City = 1
}