using UnityEngine;

namespace ShrugWare
{
    public class GetOutOfFireMicrogame : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        private const float PLAYER_MOVE_SPEED = 5.0f;

        private bool inFire = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadExit += FireEscape;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadExit -= FireEscape;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            HandleInput();
        }

        protected override bool VictoryCheck() => !inFire;

        private void HandleInput()
        {
            Vector3 newPos = playerObject.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                newPos.y -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                newPos.x -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                newPos.x += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            playerObject.transform.position = newPos;
        }

        // once they're out, we don't care if they go back in
        private void FireEscape(GameObject fireObject)
        {
            inFire = false;
            SetMicrogameEndText(true);
        }
    }
}