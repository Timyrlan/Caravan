using Assets.Scripts.World;
using Assets.Scripts.World.Cargos;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Assets.Scripts
{
    public class CityController : MonoBehaviour
    {
        [SerializeField] private Canvas Canvas;

        [SerializeField] public WorldController WorldController;

        public float Size { get; set; } = 1;
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
        public bool Visible { get; private set; }

        public PricesMap PricesMap { get; set; }

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
        }

        public void SetVisible()
        {
            Visible = true;
            gameObject.SetActive(Visible);
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void ChangeSize()
        {
            transform.Find("Scalable").transform.localScale = new Vector3(Size, Size, 1);
            var caption = transform.Find("Caption").gameObject;
            caption.transform.position = new Vector3(transform.position.x - Name.Length * 0.05f, transform.position.y - Size / 2 - 0.05f, 0);
        }
    }
}