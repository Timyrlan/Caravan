using CrvService.Shared.Contracts.Entities;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public IPlayer Player { get; private set; }

        public void UpdateFromServer(IPlayer player, bool newWorld)
        {
            var oldPlayer = Player;
            Player = player;

            //if (oldPlayer == null || Math.Abs(Player.X - oldPlayer.X) > SharedValues.Tolerance || Math.Abs(Player.Y - oldPlayer.Y) > SharedValues.Tolerance) ChangeCoordinates();
            if (newWorld) ChangeCoordinates();
        }

        private void ChangeCoordinates()
        {
            transform.position = new Vector3(Player.X, Player.Y, 0);
        }
    }
}