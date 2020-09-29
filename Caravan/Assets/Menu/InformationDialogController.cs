using TMPro;
using UnityEngine;

public class InformationDialogController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    [SerializeField] private GameObject parent;

    void Start()
    {
        parent.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        label.text = message;
        parent.SetActive(true);
    }

    public void InformationDialogClick(string message)
    {
        parent.SetActive(false);
    }
}