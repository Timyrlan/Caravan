using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;
using Assets.LogicScripts.Buildings;
using Assets.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.Menu
{
    public class EnterCityMenuDialogController : MonoBehaviour, IGameDialog
    {
        private CityController City { get; set; }

        public void ShowDialog()
        {
            transform.gameObject.SetActive(true);
        }

        public void CloseDialog()
        {
            transform.gameObject.SetActive(false);
        }

        public void ShowDialog(CityController city)
        {
            City = city;

            WelcomeToCityCaption.text = $"Welcome to {city.Name}";

            BuildingsCaption.text = string.Join($",{Environment.NewLine}", city.Buildings.Select(c => UfNameHelper.Format(c.Type)).OrderBy(c => c));


            transform.gameObject.SetActive(true);
        }

        private static List<string> ConcatLog(string message, List<string> log)
        {
            return new List<string> {message}.Concat(log).ToList();
        }

        #region SerializeField

        [SerializeField] private TextMeshProUGUI WelcomeToCityCaption;
        [SerializeField] private TextMeshProUGUI BuildingsCaption;

        #endregion
    }
}