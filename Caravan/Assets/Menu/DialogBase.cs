using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Menu
{
    public abstract class DialogBase : MonoBehaviour, IGameDialog
    {
        public virtual void ShowDialog()
        {
            //GameStatus.Paused = true;
            transform.gameObject.SetActive(true);
        }

        public virtual void CloseDialog()
        {
            transform.gameObject.SetActive(false);
            SharedValues.Paused = false;
        }
    }
}