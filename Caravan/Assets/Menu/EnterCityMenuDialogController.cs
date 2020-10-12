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

            var cityNames = city.Buildings.Select(c => c.Type).GroupBy(c => c)
                .Select(group => new
                {
                    CityName = group.Key,
                    Count = group.Count()
                })
                .OrderBy(c => c.CityName).Select(c => FormatName(c.CityName, c.Count));

            BuildingsCaption.text = string.Join($",{Environment.NewLine}", cityNames);


            transform.gameObject.SetActive(true);
        }

        private string FormatName(string name, int count)
        {
            return count > 1 ? $"{UfNameHelper.Format(name)} ({count})" : $"{UfNameHelper.Format(name)}";
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