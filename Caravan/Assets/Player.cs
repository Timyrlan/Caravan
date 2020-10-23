using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 8f;

    // Update is called once per frame
    private void Update()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * movementSpeed * Time.deltaTime;
        transform.position += movement;
    }
}