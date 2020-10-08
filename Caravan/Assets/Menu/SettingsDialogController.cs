using Assets.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Menu
{
    public class SettingsDialogController : MonoBehaviour, IGameDialog
    {
        [SerializeField] private Toggle HowLogToggle;

        [SerializeField] private GameObject Log;

        private GameSettings Settings { get; set; }

        public void ShowDialog()
        {
            transform.gameObject.SetActive(true);
            SetSettingsFromGameSettingsToControls(Settings);
        }


        public void CloseDialog()
        {
            OnResetSettings();
            transform.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Задаем значения полей объекта GameSettings из контролов в диалоге
        /// </summary>
        /// <param name="settings"></param>
        private void SetSettingsFromControlsToGameSettings(GameSettings settings)
        {
            settings.ShowLog = HowLogToggle.isOn;
        }

        /// <summary>
        ///     Задаем контролам в диалоге значения из GameSettings
        /// </summary>
        /// <param name="settings"></param>
        private void SetSettingsFromGameSettingsToControls(GameSettings settings)
        {
            HowLogToggle.isOn = settings.ShowLog;
        }

        /// <summary>
        ///     Нажатие на кнопку "Apply settings"
        /// </summary>
        public void OnApplySettings()
        {
            var tempSettings = new GameSettings();
            SetSettingsFromControlsToGameSettings(tempSettings);
            ApplySettings(tempSettings);
        }


        /// <summary>
        ///     Нажатие на кнопку "Apply and Save"
        /// </summary>
        public void OnApplyAndSaveSettings()
        {
            
            SetSettingsFromControlsToGameSettings(Settings);
            ApplySettings(Settings);
            SaveSettingsToStore(Settings);
            CloseDialog();
        }


        /// <summary>
        ///     Нажатие на кнопку "Reset"
        /// </summary>
        public void OnResetSettings()
        {
            LoadAndApplySettings();
        }

        /// <summary>
        ///     Загружаем настройки из хранилища и применяем в игре
        /// </summary>
        public void LoadAndApplySettings()
        {
            Settings = LoadSettingsFromStore();
            ApplySettings(Settings);
            SetSettingsFromGameSettingsToControls(Settings);
        }


        /// <summary>
        ///     Применяем настройки в игре
        /// </summary>
        private void ApplySettings(GameSettings settings)
        {
            Log.SetActive(settings.ShowLog);
        }

        /// <summary>
        ///     Загружаем настройки из стора
        /// </summary>
        private GameSettings LoadSettingsFromStore()
        {
            var result = new GameSettings
            {
                ShowLog = bool.Parse(PlayerPrefs.GetString(nameof(GameSettings.ShowLog), "False"))
            };

            return result;
        }

        /// <summary>
        ///     Сохраняем настройки в стор
        /// </summary>
        private void SaveSettingsToStore(GameSettings settings)
        {
            PlayerPrefs.SetString(nameof(settings.ShowLog), settings.ShowLog + "");
        }
    }

    public class GameSettings
    {
        public bool ShowLog { get; set; }
    }
}