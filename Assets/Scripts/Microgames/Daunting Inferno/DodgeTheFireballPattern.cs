using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class DodgeTheFireballPattern : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        List<GameObject> fireballs = new List<GameObject>();

        [SerializeField] 
        float FIREBALL_MOVE_SPEED = 60.0f;

        [SerializeField]
        float minBallScale = 5.0f;

        [SerializeField]
        float maxBallScale = 17.5f;

        [SerializeField]
        List<GameObject> hitVFXList;

        const float MIN_PLAYER_X_START_POS = -80;
        const float MAX_PLAYER_X_START_POS = 0;
        const float MIN_PLAYER_Y_START_POS = -50f;
        const float MAX_PLAYER_Y_START_POS = 30.0f;

        protected override void Awake()
        {
            base.Awake();

            float spawnPosX = Random.Range(MIN_PLAYER_X_START_POS, MAX_PLAYER_X_START_POS);
            float spawnPosY = Random.Range(MIN_PLAYER_Y_START_POS, MAX_PLAYER_Y_START_POS);
            playerObject.transform.position = new Vector2(spawnPosX, spawnPosY);

            // set each fireball to a random scale
            foreach (GameObject fireball in fireballs)
            {
                float newScale = Random.Range(minBallScale, maxBallScale + 1);
                fireball.transform.localScale = new Vector3(newScale, newScale, newScale);
                ParticleSystem.MainModule main = fireball.GetComponentInChildren<ParticleSystem>().main;
                main.startSize = newScale * 2;
            }
        }

        protected override void Start()
        {
            base.Start();
            playerObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += PlayerCollision;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= PlayerCollision;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            foreach(GameObject fireball in fireballs)
            {
                fireball.transform.position =
                    Vector3.MoveTowards(fireball.transform.position, new Vector3(-100, fireball.transform.position.y, 0),
                    FIREBALL_MOVE_SPEED * (Random.Range(1, 1.5f) * Time.deltaTime));
            }
        }

        protected override bool VictoryCheck() => playerObject.activeInHierarchy;

        private void PlayerCollision(GameObject fireball)
        {
            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], playerObject.transform.position, Quaternion.identity);
            Destroy(fireball);
            fireballs.Remove(fireball);

            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}