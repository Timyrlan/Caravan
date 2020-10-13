using Assets.Contracts;
using UnityEngine;

namespace Assets.Menu
{
    public class MenuDialogController : DialogBase
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
    }
}