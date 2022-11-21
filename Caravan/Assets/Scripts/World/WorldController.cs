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
                S.CaravanServerHttpConnector ??= new CaravanServerHttpConnector();

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
            if (S.CaravanServerHttpConnector != null && S.Player != null)
                try
                {
                    if (!WaitingServerResponse && (LastPingDateTimeUtc.AddMilliseconds(PingPeriodMillisecond) < DateTime.UtcNow || S.ClientCommands.Any()))
                    {
                        WaitingServerResponse = true;

                        var request = new PingRequest
                        {
                            Player = ToServerMapper.Map(S.Player),
                            ClientCommands = S.ClientCommands.ToArray()
                        };

                        StartCoroutine(S.CaravanServerHttpConnector.ProcessWorld(request, ProcessServerResponse));
                        LastPingDateTimeUtc = DateTime.UtcNow;
                    }

                    DisabledGameObject.gameObject.SetActive(S.SceneLoaded == SceneLoaded.World);

                    if (S.SceneLoaded == SceneLoaded.World)
                    {
                        ProcessMovePlayer();
                        ProcessServerCommand();
                    }
                    
                }
                catch (Exception e)
                {
                    S.CaravanServerHttpConnector = null;
                    Debug.LogError($"Error while WorldController.Update(): {e}");
                }
        }

        private void ProcessServerCommand()
        {
            var command = S.ServerCommands.OrderBy(c => c.Id).FirstOrDefault();
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
            S.ServerCommands.Remove(command);
        }

        private void ProcessEnterCityServerCommand(EnterCityServerCommand enterCityServerCommand)
        {
            Debug.Log($"Enter city {enterCityServerCommand.Guid}");

            // ReSharper disable once PossibleInvalidOperationException
            S.EnterCityGuid = enterCityServerCommand.City.Guid;
            SceneManager.LoadScene("CityScene", LoadSceneMode.Additive);
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
            if (S.CaravanServerHttpConnector == null) S.CaravanServerHttpConnector = new CaravanServerHttpConnector();

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


            yield return S.CaravanServerHttpConnector.GetNewWorld(request, ProcessServerResponse);
        }

        private void RemoveSendedCommands(ClientCommand[] commands)
        {
            var commandsGuids = commands.Select(c => c.Guid).ToArray();
            var resultCommands = new List<ClientCommand>();
            foreach (var clientCommand in S.ClientCommands)
                if (!commandsGuids.Contains(clientCommand.Guid))
                    resultCommands.Add(clientCommand);

            S.ClientCommands = resultCommands;
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

                    GameInfoController.UpdateData(S.Player);


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
                    S.ClientCommands = new List<ClientCommand>();

                    foreach (var obj in AllObjects) obj.Value.Updated = false;

                    foreach (var city in response.Player.VisibleCitys) MapCity(city, response.Player);
                    MapPayer(response.Player, true);

                    DestroyNotMappedWorldObjects();

                    MapCommands(response.Player);

                    GameInfoController.UpdateData(S.Player);
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
                if (command.Id > LastProcessedCommand && S.ServerCommands.All(c => c.Id != command.Id))
                    S.ServerCommands.Add(command);
        }

        private void MapPayer(Player player, bool newWorld = false)
        {
            if (newWorld)
            {
                S.Player = player;
            }
            else
            {
                S.Player.IsMoving = player.IsMoving;
                S.Player.MoveToX = player.MoveToX;
                S.Player.MoveToY = player.MoveToY;
                S.Player.X = player.X;
                S.Player.Y = player.Y;
                S.Player.VisibleCitys = player.VisibleCitys.ToArray();
                S.Player.World = player.World;
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

            PlayerController?.UpdateFromServer(S.Player);

            
            MovePlayer = S.Player.IsMoving ? new MovePlayer(new Vector3(S.Player.X, S.Player.Y), new Vector3(S.Player.MoveToX, S.Player.MoveToY)) : null;

            item.Updated = true;
        }

        private void MapCity(City city, Player player)
        {
            if (!AllObjects.TryGetValue(city.Guid, out var item))
            {
                item = new AllObjectsDictionaryItem { ItemFromServer = city };
                AllObjects.Add(city.Guid, item);
                item.Controller = Instantiate(CityControllerBase, new Vector3(city.X, city.Y, 0), Quaternion.identity, DisabledGameObject.transform);
            }

            (item.Controller as CityController)?.UpdateFromServer(city, player);
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

        public void WorldClick()
        {
            if (S.SceneLoaded == SceneLoaded.World)
            {
                var moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                S.ClientCommands.Add(new MovePlayerClientCommand
                {
                    ToX = moveTo.x,
                    ToY = moveTo.y
                });
            }
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
        [SerializeField] private GameObject DisabledGameObject;

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