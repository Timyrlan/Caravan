using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Menu;
using CrvService.Shared.Contracts.Entities;
using CrvService.Shared.Contracts.Entities.ClientCommands.Base;
using CrvService.Shared.Logic.ClientSide;
using CrvService.Shared.Logic.ClientSide.ClientCommands;
using TMPro;
using UnityEngine;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment


namespace Assets.Scripts.World
{
#pragma warning disable 649

// ReSharper disable InconsistentNaming


    public class WorldController : MonoBehaviour
    {
        public const float CoordinateAccuracy = 0.01f;

        private const int LogLength = 100;

        private DateTime lastPingDateTimeUtc = DateTime.MinValue;

        private Dictionary<string, AllObjectsDictionaryItem> AllObjects { get; } = new Dictionary<string, AllObjectsDictionaryItem>();

        private List<string> Log { get; set; }

        private MovePlayer MovePlayer { get; set; }

        private ICaravanServerConnector CaravanServer { get; set; }

        private IWorld World { get; set; }
        private IPlayer Player { get; set; }

        private List<IClientCommand> CommandsToSend { get; set; } = new List<IClientCommand>();

        private bool WaitingServerResponse { get; set; }

        private void Start()
        {
            SettingsDialogController.LoadAndApplySettings();
            MenuCanvas.gameObject.SetActive(true);
            SettingsDialogController.gameObject.SetActive(false);
            InformationDialog.gameObject.SetActive(false);
            EnterCityMenuDialog.gameObject.SetActive(false);
            MenuDialog.gameObject.SetActive(false);
            CityControllerBase.transform.position = new Vector3(99999, 0, 0);
            PlayerControllerBase.transform.position = new Vector3(99999, 0, 0);

            MenuDialog.ShowDialog();
        }

        private void Update()
        {
            //if (GameStatus.Paused) return;
            if (CaravanServer != null && World != null)
                try
                {
                    if (!WaitingServerResponse && (lastPingDateTimeUtc.AddSeconds(1) < DateTime.UtcNow || CommandsToSend.Any()))
                    {
                        WaitingServerResponse = true;

                        var request = new ProcessWorldRequestClientSideEntity
                        {
                            WorldGuid = World?.Guid,
                            Player = Player,
                            ClientCommands = CommandsToSend.ToArray()
                        };

                        StartCoroutine(CaravanServer.ProcessWorld(request, ProcessServerResponse));
                        lastPingDateTimeUtc = DateTime.UtcNow;
                    }

                    ProcessMovePlayer();
                }
                catch (Exception e)
                {
                    CaravanServer = null;
                    Debug.LogError($"Error while WorldController.Update(): {e}");
                }
        }

        public void OnStartLocalGameButton()
        {
            if (CaravanServer == null)
            {
                CaravanServer = new CaravanServerConnectorClientSide();

                Log = new List<string>();

                StartCoroutine(GetWorld());

                MenuDialog.CloseDialog();
            }
        }

        public void OnStartOnlineGameButton()
        {
            if (CaravanServer == null)
            {
                CaravanServer = new CaravanServerHttpConnector();

                Log = new List<string>();

                StartCoroutine(GetWorld());

                MenuDialog.CloseDialog();
            }
        }

        private void ProcessMovePlayer()
        {
            if (MovePlayer != null)
            {
                var distance = Vector3.Distance(MovePlayer.MoveTo, PlayerController.transform.position);
                if (distance > CoordinateAccuracy)
                {
                    var targetPosition = MovePlayer.GetPlayerTargetPosition(PlayerController.transform.position, Time.deltaTime);

                    PlayerController.transform.position = targetPosition;
                    PlayerController.Player.X = targetPosition.x;
                    PlayerController.Player.Y = targetPosition.y;
                }
            }
        }

        private IEnumerator GetWorld()
        {
            var request = new GetNewWorldRequestClientSideEntity
            {
                UserGuid = SettingsDialogController.Settings.UserGuid
            };


            yield return CaravanServer.GetNewWorld(request, ProcessServerResponse);
        }

        private void RemoveSendedCommands(IClientCommand[] commands)
        {
            var commandsGuids = commands.Select(c => c.Guid).ToArray();
            var resultCommands = new List<IClientCommand>();
            foreach (var clientCommand in CommandsToSend)
                if (!commandsGuids.Contains(clientCommand.Guid))
                    resultCommands.Add(clientCommand);

            CommandsToSend = resultCommands;
        }


        public void ProcessServerResponse(IProcessWorldRequest request, IProcessWorldResponse response)
        {
            WaitingServerResponse = false;
            lastPingDateTimeUtc = DateTime.UtcNow;

            if (response != null)
            {
                RemoveSendedCommands(request.ClientCommands);

                foreach (var obj in AllObjects) obj.Value.Updated = false;


                foreach (var city in response.World.Cities.Collection) MapCity(city, response.Player);
                MapPayer(response.Player);

                DestroyNotMappedWorldObjects();

                World = response.World;
                Player.IsMoving = response.Player.IsMoving;
                Player.MoveToX = response.Player.MoveToX;
                Player.MoveToY = response.Player.MoveToY;
                Player.VisibleCities = response.Player.VisibleCities;

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (Player.IsMoving)
                    MovePlayer = new MovePlayer(new Vector3(Player.X, Player.Y), new Vector3(Player.MoveToX, Player.MoveToY));
                else
                    MovePlayer = null;
            }
        }

        public void ProcessServerResponse(IGetNewWorldRequest request, IProcessWorldResponse response)
        {
            WaitingServerResponse = false;
            lastPingDateTimeUtc = DateTime.UtcNow;

            if (response != null)
            {
                CommandsToSend = new List<IClientCommand>();

                foreach (var obj in AllObjects) obj.Value.Updated = false;

                foreach (var city in response.World.Cities.Collection) MapCity(city, response.Player);
                MapPayer(response.Player, true);

                DestroyNotMappedWorldObjects();

                World = response.World;
                Player = response.Player;

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (Player.IsMoving)
                    MovePlayer = new MovePlayer(new Vector3(Player.X, Player.Y), new Vector3(Player.MoveToX, Player.MoveToY));
                else
                    MovePlayer = null;
            }
        }

        private void MapPayer(IPlayer player, bool newWorld = false)
        {
            if (!AllObjects.TryGetValue(player.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem
                {
                    ItemFromServer = player
                };

                AllObjects.Add(player.Guid, item);

                item.Controller = Instantiate(PlayerControllerBase, new Vector3(player.X, player.Y, 0), Quaternion.identity);
                PlayerController = item.Controller as PlayerController;
            }

            if (newWorld)
            {
                // ReSharper disable once PossibleNullReferenceException
                (item.Controller as PlayerController).UpdateFromServer(player);
            }
            else
            {
                Player.IsMoving = player.IsMoving;
                Player.MoveToX = player.MoveToX;
                Player.MoveToY = player.MoveToY;
                Player.WorldGuid = player.WorldGuid;
                Player.VisibleCities = player.VisibleCities;
            }


            item.Updated = true;
        }

        private void MapCity(ICity city, IPlayer player)
        {
            if (!AllObjects.TryGetValue(city.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem {ItemFromServer = city};
                AllObjects.Add(city.Guid, item);
                item.Controller = Instantiate(CityControllerBase, new Vector3(city.X, city.Y, 0), Quaternion.identity);
            }

            // ReSharper disable once PossibleNullReferenceException
            (item.Controller as CityController).UpdateFromServer(city, player);
            item.Updated = true;
        }

        private void DestroyNotMappedWorldObjects()
        {
            foreach (var item in AllObjects.ToArray())
                if (!item.Value.Updated)
                    try
                    {
                        if (item.Value.Controller != null)
                        {
                            Destroy(item.Value.Controller);
                            item.Value.Controller = null;
                        }

                        AllObjects.Remove(item.Key);
                    }
                    catch (Exception e)
                    {
                        WriteLog($"Error while DestroyNotMappedWorldObjects='{e}'");
                    }
        }


        //private void UpdateVisibleCities()
        //{
        //    foreach (var city in Cities.Where(c => !c.Visible))
        //        if (Mathf.Abs(city.X - PlayerController.transform.position.x) < 1 + city.Size / 2 && Mathf.Abs(city.Y - PlayerController.transform.position.y) < 1 + city.Size / 2)
        //        {
        //            city.SetVisible();
        //            WriteLog($"You find city {city.Name}");
        //        }
        //}

        //private void InitializePlayer()
        //{
        //    Player = new Player();
        //    var firstCity = Cities[0];
        //    PlayerController = Instantiate(PlayerControllerBase, new Vector3(firstCity.X, firstCity.Y, 0), Quaternion.identity);
        //    PlayerController.CityEntered = firstCity;

        //    WriteSystemLog($"Created Player in {firstCity.Name} with X={firstCity.X} and Y={firstCity.Y}");

        //    PlayerController.InitializePlayer();


        //    //Player.Bramins = new List<Bramin> {new Bramin(), new Bramin(), new Bramin()};
        //}

        public void WorldClick()
        {
            var moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CommandsToSend.Add(new MovePlayerClientCommandClientSideEntity
            {
                ToX = moveTo.x,
                ToY = moveTo.y
            });
        }


        //private void Update()
        //{
        //    //if (GameStatus.Paused) return;

        //    try
        //    {
        //        if (MovePlayer != null)
        //        {
        //            var distance = Vector3.Distance(MovePlayer.MoveTo, PlayerController.transform.position);
        //            if (distance > CoordinateAccuracy)
        //            {
        //                var targetPosition = MovePlayer.GetPlayerTargetPosition(PlayerController.transform.position, Time.deltaTime);
        //                //WriteSystemLog($"Move player to: {targetPosition}, distance: {distance}");
        //                PlayerController.transform.position = targetPosition;

        //                var city = FindCurrentCity();
        //                if (city == null && PlayerController.CityEntered != null) LeaveCity(PlayerController.CityEntered);

        //                UpdateVisibleCities();
        //            }
        //            else
        //            {
        //                var city = FindCurrentCity();
        //                if (city != null && PlayerController.CityEntered == null) EnterCity(city);
        //            }
        //        }

        //        ProcessWorld();
        //        UpdateHeader();
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogError($"Error while WorldController.Update(): {e}");
        //    }
        //}

        //public void ProcessWorld()
        //{
        //    var now = DateTime.UtcNow;

        //    if (LastWorldProcessedDateTime < DateTime.UtcNow.AddSeconds(-1))
        //    {
        //        LastWorldProcessedDateTime = now;

        //        ProcessWorldIteration++;

        //        WriteSystemLog($"Process world iteration={ProcessWorldIteration}");

        //        foreach (var city in Cities) city.Process();

        //        Player.Process();
        //    }
        //}

        //private void LeaveCity(CityController city)
        //{
        //    PlayerController.CityEntered = null;

        //    WriteLog($"You leave {city.City.Name}");
        //}

        //private void EnterCity(CityController city)
        //{
        //    WriteLog($"You enter {city.City.Name}");

        //    PlayerController.CityEntered = city;

        //    EnterCityMenuDialog.ShowDialog(city);

        //    //var getTokens = 0;
        //    //foreach (var playerBramin in Player.Bramins)
        //    //{
        //    //    getTokens += playerBramin.Bag.Weight;
        //    //    playerBramin.Bag.Weight = 0;
        //    //}

        //    //Player.Tokens += getTokens;
        //    //WriteLog($"You get {getTokens} tokens");
        //}

        //private CityController FindCurrentCity()
        //{
        //    foreach (var city in Cities)
        //        if (Mathf.Abs(city.X - PlayerController.transform.position.x) < city.Size / 2 && Mathf.Abs(city.Y - PlayerController.transform.position.y) < city.Size / 2)
        //            return city;

        //    return null;
        //}

        public void UpdateHeader()
        {
            //Header.text = $"TOKENS: {Player.Tokens}  BRAMINS: {Player.Bramins.Count}  WEIGHT:  {Player.BraminWeightSumm}";
        }

        private void WriteUserLog(string message)
        {
            Log = ConcatLog(message, Log);
            LogCaption.text = Log.Count + " " + string.Join(", ", Log);
            LogCaption.text = string.Join(Environment.NewLine, Log);
        }

        private static List<string> ConcatLog(string message, List<string> log)
        {
            return new List<string> {message}.Concat(log).ToList();
        }

        private void WriteSystemLog(string message)
        {
            Debug.Log(message);
        }

        private void WriteLog(string message)
        {
            WriteSystemLog(message);
            WriteUserLog(message);
        }

        #region SerializeField

        [SerializeField] private Canvas Canvas;

        [SerializeField] private CityController CityControllerBase;

        [SerializeField] private DesertController DesertController;

        [SerializeField] private TextMeshProUGUI Header;

        [SerializeField] private TextMeshProUGUI LogCaption;

        [SerializeField] private PlayerController PlayerController;

        [SerializeField] private PlayerController PlayerControllerBase;

        #endregion

        #region Menu

        [SerializeField] private Canvas MenuCanvas;

        [SerializeField] private Canvas BackgroundMenuCanvas;

        [SerializeField] private SettingsDialogController SettingsDialogController;

        [SerializeField] private InformationDialogController InformationDialog;

        [SerializeField] private EnterCityMenuDialogController EnterCityMenuDialog;

        [SerializeField] private MenuDialogController MenuDialog;

        #endregion
    }
}