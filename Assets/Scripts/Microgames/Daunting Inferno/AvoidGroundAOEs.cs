using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class AvoidGroundAOEs : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        List<GameObject> aoeInitialObjs = new List<GameObject>();

        private List<GameObject> aoeObjects = new List<GameObject>();

        private float lastSpawnTime = -1.0f;
        private const float AOE_SPAWN_TIMER = 0.5f;
        private const float AOE_SCALE_RATE = 3.35f;

        private const float DISTANCE_REQUIRED = 20.0f;
        private const float X_SPAWN_MIN = -80.0f;
        private const float X_SPAWN_MAX = 80.0f;
        private const float Y_SPAWN_MIN = -40.0f;
        private const float Y_SPAWN_MAX = 35.0f;

        private bool hit = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnBadCollision += HitAOE;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnBadCollision -= HitAOE;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if(Time.time >= lastSpawnTime + AOE_SPAWN_TIMER)
            {
                lastSpawnTime = Time.time;
                SpawnAOE();
            }

            foreach(GameObject aoeObj in aoeObjects)
            {
                float scale = aoeObj.transform.localScale.x + (AOE_SCALE_RATE * Time.deltaTime);
                aoeObj.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        protected override bool VictoryCheck()
        {
            return !hit;
        }

        private void HitAOE(GameObject go)
        {
            hit = true;
        }

        private void SpawnAOE()
        {
            bool spawned = false;
            while (!spawned)
            {
                float posX = UnityEngine.Random.Range(X_SPAWN_MIN, X_SPAWN_MAX);
                float posY = UnityEngine.Random.Range(Y_SPAWN_MIN, Y_SPAWN_MAX);
                Vector2 pos = new Vector2(posX, posY);

                if (Vector2.Distance(pos, playerObject.transform.position) > DISTANCE_REQUIRED)
                {
                    int aoeIndex = UnityEngine.Random.Range(0, aoeInitialObjs.Count);
                    GameObject newAOE = Instantiate(aoeInitialObjs[aoeIndex], pos, Quaternion.identity);
                    newAOE.SetActive(true);
                    newAOE.transform.Rotate(new Vector3(90, 0));
                    newAOE.transform.localScale = new Vector2(0.5f, 0.5f);
                    aoeObjects.Add(newAOE);

                    spawned = true;
                }
            }
        }
    }
}