using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Menu;
using CrvService.Shared.Contracts.Dto;
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
    public class AllObjectsDictionaryItem
    {
        public MonoBehaviour Controller { get; set; }
        public bool Updated { get; set; }
        public object ItemFromServer { get; set; }
    }

    //public class CaravanServerFactory
    //{
    //    public ICaravanServer GetServer(bool local = false)
    //    {
    //        if (local)
    //        {
    //            var newInstanceFactory = new NewInstanceFactoryClientSide();
    //            var processorsProvider = new ProcessorsProvider(newInstanceFactory);
    //            var newWorldGenerator = new NewWorldGenerator(newInstanceFactory);
    //            ICaravanServer caravanServer = new CaravanServerClientSide(processorsProvider, newInstanceFactory, newWorldGenerator);
    //            return caravanServer;
    //        }

    //        return new CaravanServerHttpConnector();
    //    }
    //}

    public class WorldController : MonoBehaviour
    {
        public const float CameraWidth = 20;
        public const float CameraHeight = 10;
        public const float CameraBorder = 2;
        public const float CityUnDensity = 2;
        public const float CoordinateAccuracy = 0.01f;

        public const int InitializeCityCount = 10;

        private const int LogLength = 100;

        private DateTime lastPingDateTimeUtc = DateTime.MinValue;

        private Dictionary<string, AllObjectsDictionaryItem> AllObjects { get; } = new Dictionary<string, AllObjectsDictionaryItem>();

        private List<string> Log { get; set; }

        private List<CityController> Cities { get; } = new List<CityController>();

        private MovePlayer MovePlayer { get; set; }
        //private Player Player { get; set; }

        private DateTime LastWorldProcessedDateTime { get; set; }

        private long ProcessWorldIteration { get; set; }

        private ICaravanServerConnector CaravanServer { get; set; }

        private IWorld World { get; set; }
        private IPlayer Player { get; set; }

        private List<IClientCommand> CommandsToSend { get; } = new List<IClientCommand>();


        private IEnumerator Start()
        {
            var a = new ProcessWorldRequest {WorldGuid = "qqq", Player = new PlayerDto {MoveToX = 1}};
            var b = JsonUtility.ToJson(a);
            var c = JsonUtility.FromJson<ProcessWorldRequest>(b);
            // var response = JsonUtility.FromJson<ProcessWorldResponse>(responseStr);
            SettingsDialogController.LoadAndApplySettings();
            MenuCanvas.gameObject.SetActive(true);
            //BackgroundMenuCanvas.gameObject.SetActive(true);
            SettingsDialogController.gameObject.SetActive(false);
            InformationDialog.gameObject.SetActive(false);
            EnterCityMenuDialog.gameObject.SetActive(false);
            CityControllerBase.transform.position = new Vector3(99999, 0, 0);
            PlayerControllerBase.transform.position = new Vector3(99999, 0, 0);
            //var factory = new CaravanServerFactory();
            CaravanServer = new CaravanServerHttpConnector();


            Log = new List<string>();
            //MovePlayer = null;

            yield return GetWorld();

            //InitializeCities();
            //InitializePlayer();
            //UpdateHeader();
        }

        private void Update()
        {
            //if (GameStatus.Paused) return;

            try
            {
                //if (lastPingDateTimeUtc.AddSeconds(1) < DateTime.UtcNow)
                //{
                //    var request = new ProcessWorldRequestClientSideEntity
                //    {
                //        WorldGuid = World?.Guid,
                //        Player = Player,
                //        ClientCommands = CommandsToSend.ToArray()
                //    };

                //    var response = CaravanServer.ProcessWorld(request);
                //    CommandsToSend = new List<IClientCommand>();
                //    ProcessServerResponse(response);
                //}

                //ProcessMovePlayer();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while WorldController.Update(): {e}");
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
                }
            }
        }

        private IEnumerator GetWorld()
        {
            var request = new ProcessWorldRequestClientSideEntity
            {
                WorldGuid = World?.Guid,
                Player = Player,
                ClientCommands = CommandsToSend.ToArray()
            };
            yield return CaravanServer.ProcessWorld(request, ProcessServerResponse);
            ;
        }

        public void ProcessServerResponse(IProcessWorldResponse response)
        {
            foreach (var obj in AllObjects) obj.Value.Updated = false;


            foreach (var city in response.World.Cities.Collection) MapCity(city);
            MapPayer(response.Player);

            DestroyNotMappedWorldObjects();

            World = response.World;
            Player = response.Player;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (Player.IsMoving)
                MovePlayer = new MovePlayer(new Vector3(Player.X, Player.Y), new Vector3(Player.MoveToX, Player.MoveToY));
            else
                MovePlayer = null;

            lastPingDateTimeUtc = DateTime.UtcNow;
        }

        private void MapPayer(IPlayer player)
        {
            if (!AllObjects.TryGetValue(player.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem
                {
                    ItemFromServer = player
                };

                AllObjects.Add(player.Guid, item);

                item.Controller = Instantiate(PlayerControllerBase, new Vector3(player.X, player.Y, 0), Quaternion.identity);
            }

            // ReSharper disable once PossibleNullReferenceException
            (item.Controller as PlayerController).UpdateFromServer(player);
            item.Updated = true;
        }

        private void MapCity(ICity city)
        {
            if (!AllObjects.TryGetValue(city.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem {ItemFromServer = city};
                AllObjects.Add(city.Guid, item);
                item.Controller = Instantiate(CityControllerBase, new Vector3(city.X, city.Y, 0), Quaternion.identity);
            }

            // ReSharper disable once PossibleNullReferenceException
            (item.Controller as CityController).UpdateFromServer(city);
            item.Updated = true;
        }

        private void DestroyNotMappedWorldObjects()
        {
            foreach (var item in AllObjects)
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
            CommandsToSend.Add(new MovePlayerClientCommandClientSide
            {
                ToX = moveTo.x,
                ToY = moveTo.y
            });
        }

        //public void WorldClick()
        //{
        //    MovePlayer = new MovePlayer(PlayerController.transform.position, );
        //}


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

        #endregion
    }
}