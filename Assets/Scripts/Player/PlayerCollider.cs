using System;
using UnityEngine;

namespace ShrugWare
{
    /// <summary>
    /// Sends relevant collisions as events to be picked up by microgames
    /// </summary>
    public class PlayerCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask goodMasks = 0;
        [SerializeField] private LayerMask badMasks = 0;
        public static event Action<GameObject> OnGoodCollision;
        public static event Action<GameObject> OnBadCollision;
        public static event Action<GameObject> OnAnyCollision;

        public static event Action<GameObject> OnGoodExit;
        public static event Action<GameObject> OnBadExit;
        public static event Action<GameObject> OnAnyExit;

        private void OnTriggerEnter(Collider other)
        {
            OnAnyCollision?.Invoke(other.gameObject);

            if (goodMasks.HasLayer(other.gameObject.layer))
            {
                OnGoodCollision?.Invoke(other.gameObject);
            }
            else if (badMasks.HasLayer(other.gameObject.layer))
            {
                OnBadCollision?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnAnyExit?.Invoke(other.gameObject);

            if (goodMasks.HasLayer(other.gameObject.layer))
            {
                OnGoodExit?.Invoke(other.gameObject);
            }
            else if (badMasks.HasLayer(other.gameObject.layer))
            {
                OnBadExit?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnAnyCollision?.Invoke(other.gameObject);

            if(goodMasks.HasLayer(other.gameObject.layer))
            {
                OnGoodCollision?.Invoke(other.gameObject);
            }
            else if(badMasks.HasLayer(other.gameObject.layer))
            {
                OnBadCollision?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            OnAnyExit?.Invoke(other.gameObject);

            if(goodMasks.HasLayer(other.gameObject.layer))
            {
                OnGoodExit?.Invoke(other.gameObject);
            }
            else if(badMasks.HasLayer(other.gameObject.layer))
            {
                OnBadExit?.Invoke(other.gameObject);
            }
        }

        public static void LaserHit(GameObject other)
        {
            OnBadCollision?.Invoke(other.gameObject);
        }
    }
}