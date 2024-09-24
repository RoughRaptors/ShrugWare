using UnityEngine;

namespace ShrugWare
{
    public class GetOutOfFireMicrogame : Microgame
    {
        [SerializeField]
        GameObject topWall;

        [SerializeField]
        GameObject bottomWall;

        [SerializeField]
        GameObject leftWall;

        [SerializeField]
        GameObject rightWall;

        private bool escaped = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += FireEscape;
            PlayerCollider.OnBadCollision += HitFire;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= FireEscape;
            PlayerCollider.OnBadCollision -= HitFire;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            int randNumber = UnityEngine.Random.Range(0, 4);
            if(randNumber == 0)
            {
                topWall.SetActive(false);
            }
            else if (randNumber == 1)
            {
                bottomWall.SetActive(false);
            }
            else if (randNumber == 2)
            {
                leftWall.SetActive(false);
            }
            else
            {
                rightWall.SetActive(false);
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck() => escaped;

        private void HitFire(GameObject obj)
        {
            SetMicrogameEndText(false);
        }

        // once they're out, we don't care if they go back in
        private void FireEscape(GameObject fireObject)
        {
            escaped = true;
            SetMicrogameEndText(true);
        }
    }
}