using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Menu;
using CrvService.Contracts;
using CrvService.Contracts.Entities;
using CrvService.Contracts.Entities.Commands.ClientCommands;
using CrvService.Contracts.Entities.Commands.ClientCommands.Base;
using CrvService.Contracts.Entities.Commands.ServerCommands;
using CrvService.Contracts.Entities.Commands.ServerCommands.Base;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0649

namespace Assets.Scripts.World
{
    public class WorldController : MonoBehaviour
    {
        public const float CoordinateAccuracy = 0.01f;
        public const int PingPeriodMillisecond = 1000;

        private const int LogLength = 100;

        private DateTime LastPingDateTimeUtc = DateTime.MinValue;

        private Dictionary<Guid, AllObjectsDictionaryItem> AllObjects { get; } = new();

        private List<string> Log { get; set; }

        private MovePlayer MovePlayer { get; set; }

        private CaravanServerHttpConnector CaravanServerHttpConnector { get; set; }

        private Player Player { get; set; }

        private List<ClientCommand> ClientCommands { get; set; } = new();
        private List<ServerCommand> ServerCommands { get; } = new();

        private int LastProcessedCommand { get; set; } = -1;


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

            if (PlayerPrefs.GetInt("SceneReloading", 0) == 1)
            {
                PlayerPrefs.SetInt("SceneReloading", 0);
                CaravanServerHttpConnector ??= new CaravanServerHttpConnector();

                Log = new List<string>();

                StartCoroutine(GetWorld());

                //MenuDialog.CloseDialog(); 
            }
            else
            {
                MenuDialog.ShowDialog();
            }
        }

        private void Update()
        {
            if (CaravanServerHttpConnector != null && Player != null)
                try
                {
                    if (!WaitingServerResponse && (LastPingDateTimeUtc.AddMilliseconds(PingPeriodMillisecond) < DateTime.UtcNow || ClientCommands.Any()))
                    {
                        WaitingServerResponse = true;

                        var request = new PingRequest
                        {
                            Player = ToServerMapper.Map(Player),
                            ClientCommands = ClientCommands.ToArray()
                        };

                        StartCoroutine(CaravanServerHttpConnector.ProcessWorld(request, ProcessServerResponse));
                        LastPingDateTimeUtc = DateTime.UtcNow;
                    }

                    ProcessMovePlayer();

                    ProcessServerCommand();
                }
                catch (Exception e)
                {
                    CaravanServerHttpConnector = null;
                    Debug.LogError($"Error while WorldController.Update(): {e}");
                }
        }

        private void ProcessServerCommand()
        {
            var command = ServerCommands.OrderBy(c => c.Id).FirstOrDefault();
            if (command == null) return;

            switch (command)
            {
                case EnterCityServerCommand enterCityServerCommand:
                    ProcessEnterCityServerCommand(enterCityServerCommand);
                    break;
                default:
                    Debug.Log($"Unexpected command={JsonConvert.SerializeObject(command)}");
                    break;
            }

            LastProcessedCommand = command.Id;
            ServerCommands.Remove(command);
        }

        private void ProcessEnterCityServerCommand(EnterCityServerCommand enterCityServerCommand)
        {
            Debug.Log($"Enter city {enterCityServerCommand.Guid}");
        }

        /// <summary>
        ///     Кнопка "Начать новую игру"
        /// </summary>
        public void OnStartNewGameButton()
        {
            PlayerPrefs.SetInt("SceneReloading", 1);
            SettingsDialogController.Settings.PlayerGuid = Guid.NewGuid().ToString();
            SettingsDialogController.SaveSettingsToStore();

            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        /// <summary>
        ///     Кнопка "Продолжить игру"
        /// </summary>
        public void OnContinueGameButton()
        {
            if (SettingsDialogController.Settings.PlayerGuid == null) return;
            if (CaravanServerHttpConnector == null) CaravanServerHttpConnector = new CaravanServerHttpConnector();

            Log = new List<string>();


            StartCoroutine(GetWorld());

            MenuDialog.CloseDialog();
        }

        private void ProcessMovePlayer()
        {
            if (MovePlayer != null)
            {
                var distance = Vector3.Distance(MovePlayer.MoveTo, PlayerController.transform.position);
                if (distance > CoordinateAccuracy)
                {
                    var targetPosition = MovePlayer.GetPlayerTargetPosition(PlayerController.transform.position, Time.deltaTime * 0.7f);

                    PlayerController.transform.position = targetPosition;
                    PlayerController.Player.X = targetPosition.x;
                    PlayerController.Player.Y = targetPosition.y;
                }
            }
        }

        private IEnumerator GetWorld()
        {
            var request = new GetNewWorldRequest
            {
                UserGuid = Guid.Parse(SettingsDialogController.Settings.UserGuid),
                PlayerGuid = Guid.Parse(SettingsDialogController.Settings.PlayerGuid)
            };


            yield return CaravanServerHttpConnector.GetNewWorld(request, ProcessServerResponse);
        }

        private void RemoveSendedCommands(ClientCommand[] commands)
        {
            var commandsGuids = commands.Select(c => c.Guid).ToArray();
            var resultCommands = new List<ClientCommand>();
            foreach (var clientCommand in ClientCommands)
                if (!commandsGuids.Contains(clientCommand.Guid))
                    resultCommands.Add(clientCommand);

            ClientCommands = resultCommands;
        }


        public void ProcessServerResponse(PingRequest request, PingResponse response)
        {
            try
            {
                WaitingServerResponse = false;
                LastPingDateTimeUtc = DateTime.UtcNow;

                if (response != null)
                {
                    RemoveSendedCommands(request.ClientCommands);

                    foreach (var obj in AllObjects) obj.Value.Updated = false;


                    foreach (var city in response.Player.VisibleCitys) MapCity(city, response.Player);
                    MapPayer(response.Player);

                    DestroyNotMappedWorldObjects();

                    GameInfoController.UpdateData(Player);


                    MapCommands(response.Player);
                }
            }
            catch (Exception e)
            {
                WriteLog($"Error while ProcessServerResponse='{e}'");
            }
        }

        public void ProcessServerResponse(GetNewWorldRequest request, PingResponse response)
        {
            try
            {
                WaitingServerResponse = false;
                LastPingDateTimeUtc = DateTime.UtcNow;

                if (response != null)
                {
                    ClientCommands = new List<ClientCommand>();

                    foreach (var obj in AllObjects) obj.Value.Updated = false;

                    foreach (var city in response.Player.VisibleCitys) MapCity(city, response.Player);
                    MapPayer(response.Player, true);

                    DestroyNotMappedWorldObjects();

                    MapCommands(response.Player);

                    GameInfoController.UpdateData(Player);
                }
            }
            catch (Exception e)
            {
                WriteLog($"Error while ProcessServerResponse='{e}'");
            }
        }

        private void MapCommands(Player player)
        {
            foreach (var command in player.ServerCommands)
                if (command.Id > LastProcessedCommand && ServerCommands.All(c => c.Id != command.Id))
                    ServerCommands.Add(command);
        }

        private void ProcessServerCommands()
        {
        }

        private void MapPayer(Player player, bool newWorld = false)
        {
            if (newWorld)
            {
                Player = player;
            }
            else
            {
                Player.IsMoving = player.IsMoving;
                Player.MoveToX = player.MoveToX;
                Player.MoveToY = player.MoveToY;
                Player.X = player.X;
                Player.Y = player.Y;
                Player.VisibleCitys = player.VisibleCitys.ToArray();
                Player.World = player.World;
            }

            if (!AllObjects.TryGetValue(player.Guid, out var item))
            {
                WriteLog($"Creating new player with guid='{player.Guid}'");
                item = new AllObjectsDictionaryItem
                {
                    ItemFromServer = player
                };

                AllObjects.Add(player.Guid, item);

                item.Controller = Instantiate(PlayerControllerBase, new Vector3(player.X, player.Y, 0), Quaternion.identity);
                PlayerController = item.Controller as PlayerController;
            }

            PlayerController?.UpdateFromServer(Player);

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (Player.IsMoving)
                MovePlayer = new MovePlayer(new Vector3(Player.X, Player.Y), new Vector3(Player.MoveToX, Player.MoveToY));
            else
                MovePlayer = null;

            item.Updated = true;
        }

        private void MapCity(City city, Player player)
        {
            if (!AllObjects.TryGetValue(city.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem { ItemFromServer = city };
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
            ClientCommands.Add(new MovePlayerClientCommand
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
            return new List<string> { message }.Concat(log).ToList();
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

        [SerializeField] private GameInfoController GameInfoController;

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