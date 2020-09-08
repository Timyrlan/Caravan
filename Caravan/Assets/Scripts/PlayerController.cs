using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public WorldController WorldController;

    private bool Initialized { get; set; }

    public void InitializePlayer()
    {
        Debug.Log($"Initializing player");
        Initialized = true;
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
