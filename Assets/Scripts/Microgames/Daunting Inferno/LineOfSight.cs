using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ShrugWare
{
    public class LineOfSight : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject bossObject = null;

        [SerializeField]
        GameObject laserObj = null;

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

            Quaternion targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            bossObject.transform.rotation = Quaternion.Slerp(bossObject.transform.rotation, targetRotation, 1.0f * Time.deltaTime);
        }

        protected override bool VictoryCheck()
        {
            // draw a laser from the boss to the player
            Vector3 target = laserObj.transform.position;
            target.z = 0f;

            Vector3 objectPos = playerObject.transform.position;
            target.x -= objectPos.x;
            target.y -= objectPos.y;

            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            laserObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
            laserObj.GetComponent<ShootLaser>().enabled = true;

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