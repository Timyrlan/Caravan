using UnityEngine;

namespace Assets.Menu
{
    public class MenuDialogController : DialogBase
    {
        [SerializeField] private SettingsDialogController _settingsDialogController;

        public GameObject ContinueGameButton;
        public GameObject StartNewGameButton;

        public void OnExitButton()
        {
            Application.Quit();
        }

        public void OnSettingsButton()
        {
            CloseDialog();
            _settingsDialogController.ShowDialog();
        }

        public virtual void ShowDialog()
        {
            StartNewGameButton.gameObject.SetActive(_settingsDialogController.Settings.UserGuid != null);
            ContinueGameButton.gameObject.SetActive(_settingsDialogController.Settings.PlayerGuid != null);
            base.ShowDialog();
        }
    }
}