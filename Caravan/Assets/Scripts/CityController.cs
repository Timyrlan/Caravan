using System;
using System.Linq;
using Assets.Scripts.World;
using CrvService.Contracts;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Assets.Scripts
{
    public class CityController : MonoBehaviour
    {
        [SerializeField] private Canvas Canvas;


        public CityDto City { get; private set; }

        public bool Visible { get; set; }

        public void UpdateFromServer(CityDto city, PlayerDto player)
        {
            var caption = transform.Find("Caption");
            var oldCity = City;
            City = city;

            if (oldCity == null || City.Name != oldCity.Name) caption.gameObject.GetComponent<TextMesh>().text = City.Name;

            var visible = player.Citys.Any(c => c.Guid == city.Guid);

            if (oldCity == null || Visible != visible)
            {
                Visible = visible;
                gameObject.SetActive(Visible);
            }

            if (oldCity == null || Math.Abs(City.X - oldCity.X) > SharedValues.Tolerance || Math.Abs(City.Y - oldCity.Y) > SharedValues.Tolerance || Math.Abs(City.Size - oldCity.Size) > SharedValues.Tolerance) ChangeSize();
        }

        private void ChangeSize()
        {
            transform.Find("Scalable").transform.localScale = new Vector3(City.Size, City.Size, 1);
            var caption = transform.Find("Caption").gameObject;
            caption.transform.position = new Vector3(transform.position.x - City.Name.Length * 0.05f, transform.position.y - City.Size / 2 - 0.05f, 0);
        }
    }
}