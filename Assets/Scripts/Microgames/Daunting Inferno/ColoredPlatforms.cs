using System;
using UnityEngine;

namespace ShrugWare
{
    public class ColoredPlatforms : Microgame
    {
        [SerializeField]
        GameObject[] tiles;

        [SerializeField]
        GameObject playerSpriteRendererObj;

        private GameObject curTile;

        private const float ROTATE_TIME = 1.0f;
        private float timeSpent = 0;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += TileEntered;
            PlayerCollider.OnBadCollision += LaserHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= TileEntered;
            PlayerCollider.OnBadCollision -= LaserHit;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            playerSpriteRendererObj.SetActive(true);

            RotateColors();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // don't rotate if we're about to end the game because that's unfair
            timeSpent += Time.deltaTime;
            if(timeSpent >= ROTATE_TIME && timeElapsed <= microGameTime - 1.0f)
            {
                timeSpent = 0;
                RotateColors();
            }
        }

        protected override bool VictoryCheck()
        {
            return curTile && curTile.GetComponent<MeshRenderer>().material.color == Color.green;
        }

        private void LaserHit(GameObject gameObj)
        {
            SetMicrogameEndText(false);
        }

        private void TileEntered(GameObject gameObj)
        {
            curTile = gameObj;
        }

        private void RotateColors()
        {
            foreach (GameObject tile in tiles)
            {
                tile.GetComponent<MeshRenderer>().material.color = Color.red;
            }

            // roll 0-2, that's our green
            int greenRand = UnityEngine.Random.Range(0, 3);
            tiles[greenRand].GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }
}