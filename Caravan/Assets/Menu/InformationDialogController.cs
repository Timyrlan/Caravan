using TMPro;
using UnityEngine;

// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace Assets.Menu
{
    public class InformationDialogController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        [SerializeField] private GameObject parent;

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
}