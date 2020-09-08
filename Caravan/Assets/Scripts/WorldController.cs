using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField]
    private DesertController DesertController;

    [SerializeField]
    private PlayerController PlayerControllerBase;

    [SerializeField]
    private PlayerController PlayerController;

    [SerializeField]
    private CityController CityControllerBase;

    public const float CameraWidth = 20;
    public const float CameraHeight = 10;
    public const float CameraBorder = 2;
    
    public const float CoordinateAccuracy = 0.01f;

    private List<CityController> Cities { get; set; } = new List<CityController>();

    private MovePlayer MovePlayer { get; set; }    

    // Start is called before the first frame update
    void Start()
    {
        var firstCityX = Random.Range(-CameraWidth / 2 + CameraBorder, CameraWidth / 2 - CameraBorder);
        var firstCityY = Random.Range(-CameraHeight / 2 + CameraBorder, CameraHeight / 2 - CameraBorder);
        //Debug.Log($"Creating first city with firstCityX={firstCityX} and firstCityY={firstCityY}");
        var firstCity = Instantiate(CityControllerBase, new Vector3(firstCityX, firstCityY, 0), Quaternion.identity);
        firstCity.X = firstCityX;
        firstCity.Y = firstCityY;
        firstCity.Name = "Moscow";
        Cities.Add(firstCity);

        //Debug.Log($"Creating player with firstCityX={firstCityX} and firstCityY={firstCityY}");
        PlayerController = Instantiate(PlayerControllerBase, new Vector3(firstCityX, firstCityY, 0), Quaternion.identity);

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
    public const float PlayerSpeed = 10.0f;    
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