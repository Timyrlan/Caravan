using CrvService.Contracts.Entities;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public Player Player { get; private set; }

        public void UpdateFromServer(Player player)
        {
            Player = player;
            ChangeCoordinates();
        }

        private void ChangeCoordinates()
        {
            transform.position = new Vector3(Player.X, Player.Y, 0);
        }
    }
}