using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class ImageHelper : MonoBehaviour
    {
        public static ImageHelper Instance;

        [SerializeField]
        public Sprite healthPotionImage;

        [SerializeField]
        public Sprite maxHealthPotionImage;

        [SerializeField]
        public Sprite runSpeedPotionImage;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}