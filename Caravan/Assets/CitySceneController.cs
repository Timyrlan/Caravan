using System;
using System.Linq;
using CrvService.Contracts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CitySceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CityName;

    private City City { get; set; }
    private DateTime Date { get; set; } = DateTime.MinValue;

    // Start is called before the first frame update
    private void Start()
    {
        S.SceneLoaded = SceneLoaded.City;
        UpdateCity();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateCity();
    }

    private void UpdateCity()
    {
        if (Date < S.Player.World.Date)
        {
            City = S.Player.VisibleCitys.First(c => c.Guid == S.EnterCityGuid);
            CityName.text = City.Name;
            Date = S.Player.World.Date;
        }
    }

    public void OnCloseButton()
    {
        SceneManager.UnloadSceneAsync("CityScene");
        S.SceneLoaded = SceneLoaded.World;
    }
}