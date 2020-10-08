using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Menu;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace Assets.Scripts.World
{
#pragma warning disable 649

// ReSharper disable InconsistentNaming

    public class WorldController : MonoBehaviour
    {
        public const float CameraWidth = 20;
        public const float CameraHeight = 10;
        public const float CameraBorder = 2;
        public const float CityUnDensity = 2;
        public const float CoordinateAccuracy = 0.01f;

        public const int InitializeCityCount = 10;

        private const int LogLength = 100;


        private InitializeCity[] InitializeCitiesArray =
        {
            new InitializeCity {Name = "Moscow", Size = 1},
            new InitializeCity {Name = "St.Petersburg", Size = 0.7f},
            new InitializeCity {Name = "Chita", Size = 0.5f},
            new InitializeCity {Name = "Voronezh", Size = 0.5f},
            new InitializeCity {Name = "Kaluga", Size = 0.5f},
            new InitializeCity {Name = "Ufa", Size = 0.3f},
            new InitializeCity {Name = "Novgorod", Size = 0.5f},
            new InitializeCity {Name = "Zelenograd", Size = 0.3f},
            new InitializeCity {Name = "Izhevsk", Size = 0.7f},
            new InitializeCity {Name = "Perm", Size = 1.1f}
        };


        private List<string> Log { get; set; }

        private List<CityController> Cities { get; } = new List<CityController>();

        private MovePlayer MovePlayer { get; set; }
        private Player Player { get; set; }

        private void Start()
        {
            SettingsDialogController.LoadAndApplySettings();
            MenuCanvas.gameObject.SetActive(true);
            BackgroundMenuCanvas.gameObject.SetActive(true);


            Log = new List<string>();
            MovePlayer = null;
            InitializeCities();
            InitializePlayer();
            UpdateHeader();
        }

        private Vector3? GenerateInitializedCityVectorInScreenXN(int n, float size, int deep)
        {
            if (deep >= 20) return null;

            var cityX = Random.Range(-CameraWidth * n / 2 + CameraBorder, CameraWidth * n / 2 - CameraBorder);
            var cityY = Random.Range(-CameraHeight * n / 2 + CameraBorder, CameraHeight * n / 2 - CameraBorder);

            foreach (var city in Cities)
                if (Mathf.Abs(city.X - cityX) < (city.Size + size) * CityUnDensity && Mathf.Abs(city.Y - cityY) < (city.Size + size) * CityUnDensity)
                    return GenerateInitializedCityVectorInScreenXN(n, size, deep + 1);

            return new Vector3(cityX, cityY, 0);
        }


        private InitializeCity GetRandomInitializeCity()
        {
            var random = Random.Range(0, InitializeCitiesArray.Length);
            var result = InitializeCitiesArray[random];
            InitializeCitiesArray = InitializeCitiesArray.Where(c => c.Name != result.Name).ToArray();
            return result;
        }


        private void InitializeCities()
        {
            for (var i = 0; i < InitializeCityCount; i++)
            {
                var initializeCity = GetRandomInitializeCity();

                var vector = GenerateInitializedCityVectorInScreenXN(i < 5 ? 1 : 3, initializeCity.Size, 0);

                if (vector == null)
                {
                    Debug.Log($"Couldn't create cities on iteration {i}");
                    return;
                }

                var city = Instantiate(CityControllerBase, (Vector3) vector, Quaternion.identity);

                var cityVisible = i == 0;
                city.Initialize(initializeCity, cityVisible);
                Cities.Add(city);
                Debug.Log($"Created {city.Name} with X={city.X} and Y={city.Y} and size={city.Size}");
            }
        }

        private void UpdateVisibleCities()
        {
            foreach (var city in Cities.Where(c => !c.Visible))
                if (Mathf.Abs(city.X - PlayerController.transform.position.x) < 1 + city.Size / 2 && Mathf.Abs(city.Y - PlayerController.transform.position.y) < 1 + city.Size / 2)
                    city.SetVisible();
        }

        private void InitializePlayer()
        {
            Player = new Player();
            var firstCity = Cities[0];
            PlayerController = Instantiate(PlayerControllerBase, new Vector3(firstCity.X, firstCity.Y, 0), Quaternion.identity);
            PlayerController.CityEntered = firstCity;

            Debug.Log($"Created Player in {firstCity.Name} with X={firstCity.X} and Y={firstCity.Y}");

            PlayerController.InitializePlayer();
        }

        public void WorldClick()
        {
            MovePlayer = new MovePlayer(PlayerController.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private void Update()
        {
            try
            {
                if (MovePlayer != null)
                {
                    var distance = Vector3.Distance(MovePlayer.MoveTo, PlayerController.transform.position);
                    if (distance > CoordinateAccuracy)
                    {
                        var targetPosition = MovePlayer.GetPlayerTargetPosition(PlayerController.transform.position, Time.deltaTime);
                        //Debug.Log($"Move player to: {targetPosition}, distance: {distance}");
                        PlayerController.transform.position = targetPosition;

                        var city = FindCurrentCity();
                        if (city == null && PlayerController.CityEntered != null) LeaveCity(PlayerController.CityEntered);

                        UpdateVisibleCities();
                    }
                    else
                    {
                        var city = FindCurrentCity();
                        if (city != null && PlayerController.CityEntered == null) EnterCity(city);
                    }
                }

                UpdateHeader();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while WorldController.Update(): {e}");
            }
        }

        private void LeaveCity(CityController city)
        {
            PlayerController.CityEntered = null;

            Player.Bramins.Add(new Bramin());

            //foreach (var playerBramin in Player.Bramins)
            //{
            //    playerBramin.Bag.Weight = 400;
            //}

            WriteLog($"You get 1 bramin and {Player.BraminWeightSumm} weight");
            WriteLog($"You leave {city.Name}");
        }

        private void EnterCity(CityController city)
        {
            WriteLog($"You enter {city.Name}");

            PlayerController.CityEntered = city;


            var getTokens = 0;
            //foreach (var playerBramin in Player.Bramins)
            //{
            //    getTokens += playerBramin.Bag.Weight;
            //    playerBramin.Bag.Weight = 0;
            //}

            Player.Tokens += getTokens;
            WriteLog($"You get {getTokens} tokens");
        }

        private CityController FindCurrentCity()
        {
            foreach (var city in Cities)
                if (Mathf.Abs(city.X - PlayerController.transform.position.x) < city.Size / 2 && Mathf.Abs(city.Y - PlayerController.transform.position.y) < city.Size / 2)
                    return city;

            return null;
        }

        public void UpdateHeader()
        {
            Header.text = $"TOKENS: {Player.Tokens}  BRAMINS: {Player.Bramins.Count}  WEIGHT:  {Player.BraminWeightSumm}";
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

        [SerializeField] private InformationDialogController InformationDialog;

        [SerializeField] private TextMeshProUGUI LogCaption;

        [SerializeField] private PlayerController PlayerController;

        [SerializeField] private PlayerController PlayerControllerBase;

        [SerializeField] private SettingsDialogController SettingsDialogController;

        [SerializeField] private Canvas MenuCanvas;
        [SerializeField] private Canvas BackgroundMenuCanvas;

        #endregion
    }
}