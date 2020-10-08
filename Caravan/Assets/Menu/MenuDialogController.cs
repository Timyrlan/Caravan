using Assets.Contracts;
using UnityEngine;

namespace Assets.Menu
{
    public class MenuDialogController : MonoBehaviour, IGameDialog
    {
        [SerializeField] private SettingsDialogController _settingsDialogController;
        public void OnExitButton()
        {
            Application.Quit();
        }

        public void OnSettingsButton()
        {
            CloseDialog();
            _settingsDialogController.ShowDialog();
        }

        public void CloseDialog()
        {
            transform.gameObject.SetActive(false);
        }

        public void ShowDialog()
        {
            transform.gameObject.SetActive(transform);
        }
    }
}