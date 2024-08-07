using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ShrugWare
{
    public class ShootTheBoss : Microgame
    {
        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        GameObject bulletObj;

        [SerializeField]
        GameObject enemyObj;

        [SerializeField]
        TextMeshProUGUI debugText;

        private LineRenderer lineRenderer;

        private const int BULLET_SPEED = 175;

        private bool enemyHit = false;

        private float time = 0;

        private const int LEFT_ENEMY_LIMIT = -85;
        private const int RIGHT_ENEMY_LIMIT = 85;
        private Vector3 enemyMovePos = new Vector3(0, 0, 0);
        private Vector3 enemyStartPos = new Vector3(0, 0, 0);

        new private void Start()
        {
            base.Start();

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;

            enemyObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

            PickEnemyMovePosition();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            enemyObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            lineRenderer.enabled = true;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (!enemyHit)
            {
                DrawLine();
                time += Time.deltaTime / microGameTime;
                enemyObj.transform.position = Vector3.Lerp(enemyStartPos, enemyMovePos, time);
            }
        }

        protected override bool VictoryCheck()
        {
            return enemyHit;
        }

        private void DrawLine()
        {
            Vector3 startPos = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y);
            lineRenderer.SetPosition(0, startPos);

            Vector3 mousePosModified = Input.mousePosition;
            mousePosModified.z = 10;

            Vector3 endPos = Camera.main.ScreenToWorldPoint(mousePosModified);
            lineRenderer.SetPosition(1, endPos);

            debugText.text = "Mouse Pos: " + mousePosModified.ToString() + "\nEnd Pos: " + endPos.ToString();
        }

        private void Shoot()
        {
            Vector3 mousePosModified = Input.mousePosition;
            mousePosModified.z = 10;

            GameObject bullet = Instantiate(bulletObj, playerObj.transform.position, Quaternion.identity);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Camera.main.transform.forward, playerObj.transform.position);

            float rayDistanceAtPlane;
            bool hasHitPlane = plane.Raycast(ray, out rayDistanceAtPlane);
            Vector3 mousePosWorldSpace = ray.origin + ray.direction * rayDistanceAtPlane;
            Vector3 playerToMouseDirection = (mousePosWorldSpace - playerObj.transform.position);
            playerToMouseDirection.z = 0;
            playerToMouseDirection.Normalize();

            bullet.GetComponent<Rigidbody>().velocity = playerToMouseDirection * BULLET_SPEED;
            bullet.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            enemyHit = true;
            Destroy(other.gameObject);
            SetMicrogameEndText(true);
        }

        private void PickEnemyMovePosition()
        {
            // pick left or right and move there
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                enemyMovePos = new Vector2(LEFT_ENEMY_LIMIT, 40);
            }
            else
            {
                enemyMovePos = new Vector2(RIGHT_ENEMY_LIMIT, 40);
            }

            enemyStartPos = enemyObj.transform.position;
        }
    }
}