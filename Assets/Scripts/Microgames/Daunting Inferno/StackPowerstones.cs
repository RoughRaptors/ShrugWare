using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StackPowerstones : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject powerstoneObj = null;

        private const float X_MIN = -65.0f;
        private const float X_MAX = 65.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 14.0f;

        private const float NUM_POWERSTONES_TOTAL = 4;
        private float numPowerstonesCollected = 0;

        private List<GameObject> powerstones = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            SpawnPowerstones();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += CollectStone;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= CollectStone;
        }

        protected override bool VictoryCheck()
        {
            playerObject.transform.GetChild(0).gameObject.SetActive(false);
            bool victory = numPowerstonesCollected == NUM_POWERSTONES_TOTAL;
            if (!victory)
            {
                foreach(GameObject go in powerstones)
                {
                    go.SetActive(false);
                }
            }
            return victory;
        }

        private void SpawnPowerstones()
        {
            for (int i = 0; i < NUM_POWERSTONES_TOTAL; ++i)
            {
                int numTries = 0;

                while (numTries < 100)
                {
                    ++numTries;

                    float xPos = Random.Range(X_MIN, X_MAX);
                    float yPos = Random.Range(Y_MIN, Y_MAX);
                    Vector3 powerstonePos = new Vector3(xPos, yPos, 0);
                    if (Vector3.Distance(playerObject.transform.position, powerstonePos) > 15.0f)
                    {
                        GameObject powerstone = Instantiate(powerstoneObj);
                        powerstone.transform.position = powerstonePos;
                        powerstone.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);

                        powerstones.Add(powerstone);
                        break;
                    }
                }
            }
        }

        private void CollectStone(GameObject stone)
        {
            stone.SetActive(false);
            if (++numPowerstonesCollected == NUM_POWERSTONES_TOTAL)
            {
                SetMicrogameEndText(true);
            }
        }
    }
}