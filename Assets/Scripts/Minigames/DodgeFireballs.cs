using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    // 45 seconds long
    // 0-15s - come from right
    // 15s-30s come from top + right
    // 30s-45s come from bottom + top + right

    public class DodgeFireballs : Minigame
    {
        [SerializeField]
        GameObject fireballInitObj;

        const float FIREBALL_X_MIN = -10;
        const float FIREBALL_X_MAX = 125;
        const float FIREBALL_Y_MIN = -30;
        const float FIREBALL_Y_MAX = 40;
        const float FIREBALL_SPEED_MIN = 0.05f;
        const float FIREBALL_SPEED_MAX = 0.2f;

        const float FIREBALL_SPAWN_INTERVAL = 0.5f;
        float timeSinceLastSpawn = 0.0f;

        private float timeInGame = 0.0f;

        private enum FromDirection
        {
            FromRight = 0,
            FromTop,
            FromBottom
        }

        protected struct Fireball
        {
            public GameObject fireballObj;
            public Vector3 position;
            public Vector3 targetPos;
            public float speed;
            FromDirection fromDir;
        }

        private List<Fireball> fireballsList = new List<Fireball>();

        private new void Start()
        {
            // initial wave
            for(int i = 0; i < 15; ++i)
            {
                SpawnFireball(FromDirection.FromRight);
            }
        }

        private void Update()
        {
            if(timeSinceLastSpawn >= FIREBALL_SPAWN_INTERVAL)
            {
                timeSinceLastSpawn = 0;

                // cast down to int to be able to cleanly mod 15
                // spawn more at 15s and 30s
                if ((int)timeInGame != 0 && (int)timeInGame % 30 == 0)
                {
                    // spawn top
                    for (int i = 0; i < 15; ++i)
                    {
                        SpawnFireball(FromDirection.FromBottom);
                    }
                }
                else if ((int)timeInGame != 0 && (int)timeInGame % 15 == 0)
                {
                    // spawn bottom
                    for (int i = 0; i < 15; ++i)
                    {
                        SpawnFireball(FromDirection.FromTop);
                    }
                }

                SpawnFireball(FromDirection.FromRight);
                SpawnFireball(FromDirection.FromBottom);
                SpawnFireball(FromDirection.FromTop);
            }

            UpdateFireballs();
            timeSinceLastSpawn += Time.deltaTime;
            timeInGame += Time.deltaTime;
        }

        private void SpawnFireball(FromDirection fromDir)
        {
            Fireball newFireball = new Fireball();
            GameObject newFireballObj = Instantiate(fireballInitObj);
            newFireball.fireballObj = newFireballObj;
            newFireballObj.gameObject.SetActive(true);

            float speed = UnityEngine.Random.Range(FIREBALL_SPEED_MIN, FIREBALL_SPEED_MAX);
            newFireball.speed = speed;

            float xPos = 0.0f;
            float yPos = 0.0f;
            if (fromDir == FromDirection.FromRight)
            {
                xPos = FIREBALL_X_MAX;
                yPos = UnityEngine.Random.Range(FIREBALL_Y_MIN, FIREBALL_Y_MAX);
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(-25, yPos, 0);
                newFireball.targetPos = targetPos;
            }
            else if (fromDir == FromDirection.FromTop)
            {
                xPos = UnityEngine.Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
                yPos = 50;
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(xPos, -40, 0);
                newFireball.targetPos = targetPos;
            }
            else if (fromDir == FromDirection.FromBottom)
            {
                xPos = UnityEngine.Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
                yPos = -40;
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(xPos, 50, 0);
                newFireball.targetPos = targetPos;
            }

            fireballsList.Add(newFireball);
        }

        private void UpdateFireballs()
        {
            for(int i = 0; i < fireballsList.Count; ++i)
            {
                Fireball fireball = fireballsList[i];

                // done with it
                if (fireball.fireballObj.transform.position == fireball.targetPos)
                {
                    fireballsList.RemoveAt(i);
                    Destroy(fireball.fireballObj);
                    continue;
                }

                fireball.fireballObj.transform.position =
                    Vector3.MoveTowards(fireball.fireballObj.transform.position, fireball.targetPos, fireball.speed);
            }
        }
    }
}