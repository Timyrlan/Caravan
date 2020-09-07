using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField]
    private PlayerController PlayerController;

    [SerializeField]
    private CityController CityController;

    public const float CameraWidth = 20;
    public const float CameraHeight = 10;
    public const float CameraBorder = 2;

    private List<CityController> Cities { get; set; } = new List<CityController>();

    // Start is called before the first frame update
    void Start()
    {
        var firstCityX = Random.Range(-CameraWidth/2 + CameraBorder, CameraWidth/2 - CameraBorder);
        var firstCityY = Random.Range(-CameraHeight/2 + CameraBorder, CameraHeight/2 - CameraBorder);
        Debug.Log($"Creating first city with firstCityX={firstCityX} and firstCityY={firstCityY}");
        var firstCity = Instantiate(CityController, new Vector3(firstCityX, firstCityY, 0), Quaternion.identity);
        Cities.Add(firstCity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
