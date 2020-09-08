using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField]
    public WorldController WorldController;

    public float Size { get; set; } = 1;
    public float X { get; set; }
    public float Y { get; set; }
    public string Name { get; set; }

    public void Initialize(InitializeCity initializeCity)
    {
        Name = initializeCity.Name;
        Size = initializeCity.Size;
        X = transform.position.x;
        Y = transform.position.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Size, Size, 1);
    }
}
