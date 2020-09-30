using UnityEngine;

namespace Assets.Scripts.World
{
    public class MovePlayer
    {
        public const float PlayerSpeed = 1.0f;

        public MovePlayer(Vector3 moveFrom, Vector3 moveTo)
        {
            MoveFrom = new Vector3(moveFrom.x, moveFrom.y, 0);
            MoveTo = new Vector3(moveTo.x, moveTo.y, 0);
        }

        public Vector3 MoveFrom { get; }
        public Vector3 MoveTo { get; }

        public Vector3 GetPlayerTargetPosition(Vector3 playerPosition, float deltaTime)
        {
            var dirNormalized = (MoveTo - playerPosition).normalized;
            var targetPosition = playerPosition + dirNormalized * PlayerSpeed * deltaTime;
            return new Vector3(targetPosition.x, targetPosition.y, 0);
        }
    }
}