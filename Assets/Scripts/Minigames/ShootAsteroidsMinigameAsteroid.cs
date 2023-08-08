using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    // 45 seconds long
    // 0-15s - come from right
    // 15s-30s come from top + right
    // 30s-45s come from bottom + top + right
    public class ShootAsteroidsMinigameAsteroid : MonoBehaviour
    {
        public struct Asteroid
        {
            public GameObject asteroidObj;
            //public Vector3 targetPos;
            //public float speed;
        }

        // something hit an asteroid
        private void OnTriggerEnter(Collider other)
        {
            Destroy(this.gameObject);

            if(other.tag != "Player")
            {
                Destroy(other.gameObject);
            }

            ++ShootAsteroidsMinigame.NumAsteroidsDestroyed;
            ShootAsteroidsMinigame.NumAsteroidsDestroyed += 15;
        }
    }
}