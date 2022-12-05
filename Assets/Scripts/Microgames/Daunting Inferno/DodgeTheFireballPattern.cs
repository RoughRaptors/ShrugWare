using UnityEngine;

namespace ShrugWare
{
    public class DodgeTheFireballPattern : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject[] fireballs = new GameObject[0];

        [SerializeField] 
        float FIREBALL_MOVE_SPEED = 60.0f;

        [SerializeField] 
        private float minPlayerStartPosX = -80;

        [SerializeField] 
        private float maxPlayerStartPosX = 80;

        [SerializeField]
        float minPlayerStartPosY = -40f;

        [SerializeField]
        float maxPlayerStartPosY = 20f;

        [SerializeField]
        float minBallScale = 5.0f;

        [SerializeField]
        float maxBallScale = 17.5f;


        protected override void Awake()
        {
            base.Awake();

            float spawnPosX = Random.Range(minPlayerStartPosX, maxPlayerStartPosX);
            float spawnPosY = Random.Range(minPlayerStartPosY, maxPlayerStartPosY);
            playerObject.transform.position = new Vector2(spawnPosX, spawnPosY);

            // set each fireball to a random scale
            foreach (GameObject fireball in fireballs)
            {
                float newScale = Random.Range(minBallScale, maxBallScale + 1);
                fireball.transform.localScale = new Vector3(newScale, newScale, newScale);
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
            PlayerCollider.OnAnyCollision += PlayerCollision;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnAnyCollision -= PlayerCollision;
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
            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}