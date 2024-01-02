using UnityEngine;

namespace ShrugWare
{
    public class ColoredPlatforms : Microgame
    {
        [SerializeField]
        GameObject[] tiles;

        GameObject curTile;

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
            SetTileColors();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
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

        private void SetTileColors()
        {
            if (tiles.Length > 0)
            {
                foreach (GameObject tile in tiles)
                {
                    tile.GetComponent<MeshRenderer>().material.color = Color.red;
                }

                // roll 0-2, that's our green
                int greenRand = Random.Range(0, 3);
                tiles[greenRand].GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }
}