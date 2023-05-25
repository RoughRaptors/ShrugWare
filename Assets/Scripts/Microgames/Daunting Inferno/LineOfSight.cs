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
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // rotate around until he's facing forward
            bossObject.transform.Rotate(Vector3.up * 52 * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Raycast(bossObject.transform.position, (bossObject.transform.position - playerObject.transform.position), out hit, 100))
            {

            }

            Debug.DrawRay(bossObject.transform.position, bossObject.transform.position - playerObject.transform.position, Color.green);

            //Vector3 forward = bossObject.transform.TransformDirection(Vector3.forward) * 10;
            //Debug.DrawRay(bossObject.transform.position, forward, Color.green);

        }

        protected override bool VictoryCheck()
        {
            RaycastHit hit;
            if (Physics.Raycast(bossObject.transform.position, (bossObject.transform.position - playerObject.transform.position), out hit, 100))
            {
                if (hit.collider.tag != "Player")
                {
                    return true;
                }
            }

            return false;
        }
    }
}