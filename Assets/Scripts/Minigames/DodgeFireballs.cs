using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class DodgeFireballs : Minigame
    {
        [SerializeField]
        GameObject fireballInitObj;

        const float FIREBALL_X_POS = 125;
        const float FIREBALL_Y_MIN = -30;
        const float FIREBALL_Y_MAX = 40;
        const float FIREBALL_SPEED_MIN = 0.05f;
        const float FIREBALL_SPEED_MAX = 0.2f;

        const float FIREBALL_SPAWN_INTERVAL = 0.5f;
        float timeSinceLastSpawn = 0.0f;

        protected struct Fireball
        {
            public GameObject fireballObj;
            public Vector3 position;
            public Vector3 targetPos;
            public float speed;
        }

        private List<Fireball> fireballsList = new List<Fireball>();

        private void Start()
        {
            // initial wave
            for(int i = 0; i < 15; ++i)
            {
                SpawnFireball();
            }
        }

        private void Update()
        {
            if(timeSinceLastSpawn >= FIREBALL_SPAWN_INTERVAL)
            {
                timeSinceLastSpawn = 0;
                SpawnFireball();                
            }

            UpdateFireballs();
            timeSinceLastSpawn += Time.deltaTime;
        }

        private void SpawnFireball()
        {
            Fireball newFireball = new Fireball();
            GameObject newFireballObj = Instantiate(fireballInitObj);
            newFireball.fireballObj = newFireballObj;
            newFireballObj.gameObject.SetActive(true);

            float yPos = UnityEngine.Random.Range(FIREBALL_Y_MIN, FIREBALL_Y_MAX);
            newFireballObj.transform.position = new Vector3(FIREBALL_X_POS, yPos, 0);

            float speed = UnityEngine.Random.Range(FIREBALL_SPEED_MIN, FIREBALL_SPEED_MAX);
            newFireball.speed = speed;

            Vector3 targetPos = new Vector3(-25, yPos, 0);
            newFireball.targetPos = targetPos;

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