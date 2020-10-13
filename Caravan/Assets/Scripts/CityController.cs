﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Contracts;
using Assets.LogicScripts.Buildings;
using Assets.LogicScripts.Buildings.Factories;
using Assets.LogicScripts.DifferentCargos;
using Assets.Scripts.World;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Assets.Scripts
{
    public class CityController : MonoBehaviour, IGameProcessibleObject
    {
        [SerializeField] private Canvas Canvas;

        [SerializeField] public WorldController WorldController;

        public float Size { get; set; } = 1;
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
        public bool Visible { get; private set; }

        //public PricesMap PricesMap { get; set; }

        public List<Building> Buildings { get; } = new List<Building>();

        public void Process()
        {
            FillLivingHouses();

            foreach (var building in Buildings)
                try
                {
                    building.Process();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while CityController='{Name}'.Process(): {e}");
                }
        }

        private void FillLivingHouses()
        {
            var livingHousesCount = Buildings.Count(c => c.Type == nameof(LivingHouse));
            var countToAdd = (int) Math.Floor(Size * 10 - livingHousesCount);

            for (var i = 0; i < countToAdd; i++)
                Buildings.Add(new LivingHouse
                {
                    Cargos = new List<Cargo>
                    {
                        new FreshWater {Count = 1}
                    }
                });
        }

        public void Initialize(InitializeCity initializeCity, bool visible = false)
        {
            Name = initializeCity.Name;
            Size = initializeCity.Size;
            X = transform.position.x;
            Y = transform.position.y;
            var caption = transform.Find("Caption");
            caption.gameObject.GetComponent<TextMesh>().text = Name;
            Visible = visible;
            gameObject.SetActive(Visible);

            ChangeSize();

            FillLivingHouses();
        }

        public void SetVisible()
        {
            Visible = true;
            gameObject.SetActive(Visible);
        }

        private void ChangeSize()
        {
            transform.Find("Scalable").transform.localScale = new Vector3(Size, Size, 1);
            var caption = transform.Find("Caption").gameObject;
            caption.transform.position = new Vector3(transform.position.x - Name.Length * 0.05f, transform.position.y - Size / 2 - 0.05f, 0);
        }
    }
}