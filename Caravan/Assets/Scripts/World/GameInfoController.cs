using System.Linq;
using CrvService.Contracts.Entities;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class GameInfoController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Bramins;
        [SerializeField] private TextMeshProUGUI Cities;
        [SerializeField] private TextMeshProUGUI WorldAge;
        private Player Player { get; set; }

        public void UpdateData(Player player)
        {
            Player = player;

            WorldAge.text = player?.World != null ? player.World.Date.Second.ToString() : "0";
            Cities.text = player?.VisibleCitys != null && player.VisibleCitys.Any() ? player.VisibleCitys.Count.ToString() : "0";
            Bramins.text = player?.Bramins != null && player.Bramins.Any() ? player.Bramins.Count.ToString() : "0";
        }
    }
}