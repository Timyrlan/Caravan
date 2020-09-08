using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour
{
    [SerializeField]
    private DesertController DesertController;

    [SerializeField]
    public PlayerController PlayerControllerBase;

    [SerializeField]
    private PlayerController PlayerController; 

    [SerializeField]
    public CityController CityControllerBase;

    public const float CameraWidth = 20;
    public const float CameraHeight = 10;
    public const float CameraBorder = 2;
    
    public const float CoordinateAccuracy = 0.01f;

    public const int InitializeCityCount = 10;

    private List<CityController> Cities { get; set; } = new List<CityController>();

    private InitializeCity[] InitializeCitiesArray = new InitializeCity[] {
        new InitializeCity{ Name = "Moscow", Size = 1},
        new InitializeCity{ Name = "St.Petersburg", Size = 0.7f},
        new InitializeCity{ Name = "Chita", Size = 0.5f},
        new InitializeCity{ Name = "Voronezh", Size = 0.5f},
        new InitializeCity{ Name = "Kaluga", Size = 0.5f},
        new InitializeCity{ Name = "Ufa", Size = 0.3f},
        new InitializeCity{ Name = "Novgorod", Size = 0.5f},
        new InitializeCity{ Name = "Zelenograd", Size = 0.3f},
        new InitializeCity{ Name = "Izhevsk", Size = 0.7f},
        new InitializeCity{ Name = "Perm", Size = 1.1f},
    };        

    private MovePlayer MovePlayer { get; set; }    

    void Start()
    {
        InitializeCities();
        InitializePlayer();
    }

    private Vector3 GenerateInitializedCityVectorInScreenXN(int n, float size)
    {
        var cityX = Random.Range(-CameraWidth * n / 2 + CameraBorder, CameraWidth * n / 2 - CameraBorder);
        var cityY = Random.Range(-CameraHeight * n / 2 + CameraBorder, CameraHeight * n / 2 - CameraBorder);

        foreach (var city in Cities)
        {
            if (Mathf.Abs(city.X - cityX) < (city.Size+ size) * 3 && Mathf.Abs(city.Y - cityY) < (city.Size + size))
            {
                return GenerateInitializedCityVectorInScreenXN(n, size);
            }
        }

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
        for(var i = 0; i< InitializeCityCount; i++)
        {
            var initializeCity = GetRandomInitializeCity();
            var vector = GenerateInitializedCityVectorInScreenXN(i<3?1:3, initializeCity.Size);            
            var city = Instantiate(CityControllerBase, vector, Quaternion.identity);
            city.Initialize(initializeCity);
            Cities.Add(city);
            Debug.Log($"Created {city.Name} with X={city.X} and Y={city.Y} and size={city.Size}");
        }       
    }

    private void InitializePlayer()
    {
        var firstCity = Cities[0];        
        PlayerController = Instantiate(PlayerControllerBase, new Vector3(firstCity.X, firstCity.Y, 0), Quaternion.identity);
        PlayerController.CityEntered = firstCity;

        Debug.Log($"Created Player in {firstCity.Name} with X={firstCity.X} and Y={firstCity.Y}");
        
        PlayerController.InitializePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            MovePlayer = new MovePlayer(PlayerController.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //PlayerController.LastCity = FindCurrentCity();
        }

        if (MovePlayer != null)
        {
            var distance = Vector3.Distance(MovePlayer.MoveTo, PlayerController.transform.position);
            if (distance > CoordinateAccuracy)
            {
                var targetPosition = MovePlayer.GetPlayerTargetPosition(PlayerController.transform.position, Time.deltaTime);
                //Debug.Log($"Move player to: {targetPosition}, distance: {distance}");
                PlayerController.transform.position = targetPosition;

                var city = FindCurrentCity();
                if (city == null && PlayerController.CityEntered != null)
                {
                    LeaveCity(PlayerController.CityEntered);
                }
            }
            else
            {
                var city = FindCurrentCity();
                if (city != null && PlayerController.CityEntered == null)
                {
                    EnterCity(city);
                }
            }
        }
    }

    private void LeaveCity(CityController city)
    {
        Debug.Log($"You leave {city.Name}");
        PlayerController.CityEntered = null;
    }

    private void EnterCity(CityController city)
    {
        PlayerController.CityEntered = city;
        Debug.Log($"You enter city {city}");
    }

    private CityController FindCurrentCity()
    {
        foreach (var city in Cities)
        {
            if (Mathf.Abs(city.X - PlayerController.transform.position.x) < city.Size / 2 && Mathf.Abs(city.Y - PlayerController.transform.position.y) < city.Size / 2)
            {
                return city;
            }
        }

        return null;
    }
}

public class MovePlayer
{
    public const float PlayerSpeed = 1.0f;    
    public MovePlayer(Vector3 moveFrom, Vector3 moveTo)
    {
        MoveFrom = new Vector3(moveFrom.x, moveFrom.y, 0);
        MoveTo = new Vector3(moveTo.x, moveTo.y, 0);
    }
    public Vector3 MoveFrom { get; }
    public Vector3 MoveTo { get;  }
    
    public Vector3 GetPlayerTargetPosition(Vector3 playerPosition, float deltaTime)
    {
        var dirNormalized = (MoveTo - playerPosition).normalized;
        var targetPosition = playerPosition + dirNormalized * PlayerSpeed * deltaTime;
        return new Vector3(targetPosition.x, targetPosition.y, 0);

    }
}

public class InitializeCity
{
    public string Name { get; set; }

    public float Size { get; set; }
}