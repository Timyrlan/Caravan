using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }


    private void LateUpdate()
    {
        //transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}