using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public WorldController WorldController;

    public CityController CityEntered { get; set; }

    public bool CityLeaved { get; set; }

    private bool Initialized { get; set; }

    public void InitializePlayer()
    {
        //Debug.Log($"Initializing player");
        Initialized = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("City entered");
        CityController city = other.GetComponent<CityController>();
        if (city != null)
        {
            Debug.Log("City entered");
        }        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
