using UnityEngine;

namespace Assets.Scripts.World
{
    public class AllObjectsDictionaryItem
    {
        public MonoBehaviour Controller { get; set; }
        public bool Updated { get; set; }
        public object ItemFromServer { get; set; }
    }
}