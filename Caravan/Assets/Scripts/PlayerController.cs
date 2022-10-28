using CrvService.Contracts;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerDto Player { get; private set; }

        public void UpdateFromServer(PlayerDto player)
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