using UnityEngine;

namespace ShrugWare
{
    public class LineOfSight : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject bossObject = null;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            bossObject.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // rotate around until he's facing forward
            bossObject.transform.Rotate(Vector3.up * 52 * Time.deltaTime);

            Debug.DrawRay(bossObject.transform.position, playerObject.transform.position - bossObject.transform.position, Color.green);

        }

        protected override bool VictoryCheck()
        {
            // did our raycast hit the player?
            RaycastHit hit;
            if (Physics.Raycast(bossObject.transform.position, (playerObject.transform.position - bossObject.transform.position), out hit, 1000))
            {
                if(hit.collider.tag == "Player")
                {
                    // boss can see us, we lose
                    return false;
                }
            }

            return true;
        }
    }
}