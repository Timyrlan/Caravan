using System;
using Assets.Scripts.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Menu
{
    public class SettingsDialogController : DialogBase, IGameDialog
    {
        [SerializeField] private Toggle HowLogToggle;


        [SerializeField] private GameObject Log;
        [SerializeField] private TextMeshProUGUI PlayerGuid;
        [SerializeField] private TextMeshProUGUI UserGuid;
        [SerializeField] private WorldController WorldController;

        public GameSettings Settings { get; private set; }

        public override void ShowDialog()
        {
            SetSettingsFromGameSettingsToControls(Settings);
            base.ShowDialog();
        }


        public override void CloseDialog()
        {
            OnResetSettings();
            base.CloseDialog();
        }

        /// <summary>
        ///     Задаем значения полей объекта GameSettings из контролов в диалоге
        /// </summary>
        /// <param name="settings"></param>
        private void SetSettingsFromControlsToGameSettings(GameSettings settings)
        {
            settings.ShowLog = HowLogToggle.isOn;
            settings.UserGuid = UserGuid.text;
            settings.PlayerGuid = PlayerGuid.text;
        }

        /// <summary>
        ///     Задаем контролам в диалоге значения из GameSettings
        /// </summary>
        /// <param name="settings"></param>
        private void SetSettingsFromGameSettingsToControls(GameSettings settings)
        {
            HowLogToggle.isOn = settings.ShowLog;
            UserGuid.text = settings.UserGuid;
            PlayerGuid.text = settings.PlayerGuid;
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
            SaveSettingsToStore();
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
            if (Guid.NewGuid().ToString() == Guid.Empty.ToString())
            {
                PlayerPrefs.DeleteAll();
            }

            var result = new GameSettings
            {
                ShowLog = bool.Parse(PlayerPrefs.GetString(nameof(GameSettings.ShowLog), "False")),
                PlayerGuid = PlayerPrefs.GetString(nameof(GameSettings.PlayerGuid), null),
                UserGuid = PlayerPrefs.GetString(nameof(GameSettings.UserGuid), Guid.NewGuid().ToString())
            };

            PlayerPrefs.SetString(nameof(result.UserGuid), result.UserGuid + "");


            return result;
        }

        /// <summary>
        ///     Сохраняем настройки в стор
        /// </summary>
        public void SaveSettingsToStore()
        {
            PlayerPrefs.SetString(nameof(Settings.ShowLog), Settings.ShowLog.ToString());
            PlayerPrefs.SetString(nameof(Settings.UserGuid), Settings.UserGuid);
            PlayerPrefs.SetString(nameof(Settings.PlayerGuid), Settings.PlayerGuid);
        }
    }

    public class GameSettings
    {
        public bool ShowLog { get; set; }
        public string PlayerGuid { get; set; }

        public string UserGuid { get; set; }
    }
}