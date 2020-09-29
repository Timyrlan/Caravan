using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreLabel;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("scoreLabel: " + scoreLabel?.GetType());
    }


    public void OnOpenSettings()
    {
        Debug.Log("open settings");
    }
}