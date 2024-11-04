using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StackPowerstones : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        List<GameObject> powerstoneObjects = new List<GameObject>();

        [SerializeField]
        List<AudioClipData> pickupAudioList = new List<AudioClipData>();

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -25.0f;
        private const float Y_MAX = 27.0f;

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
                    Vector3 powerstonePos = new Vector3(xPos, yPos, -20);
                    if (Vector3.Distance(playerObject.transform.position, powerstonePos) > 35.0f)
                    {
                        int index = UnityEngine.Random.Range(0, powerstoneObjects.Count);
                        GameObject powerstone = Instantiate(powerstoneObjects[index]);
                        powerstone.transform.position = powerstonePos;
                        powerstone.layer = 8;
                        powerstone.gameObject.AddComponent<BoxCollider2D>();
                        powerstone.SetActive(true);

                        powerstones.Add(powerstone);
                        playerObject.GetComponent<PlayerMover>().AddSpeed(10);
                        break;
                    }
                }
            }
        }

        private void CollectStone(GameObject stone)
        {
            if(AudioManager.Instance != null)
            {
                int index = UnityEngine.Random.Range(0, pickupAudioList.Count - 1);
                AudioManager.Instance.PlayAudioClip(pickupAudioList[index]);
            }

            stone.SetActive(false);
            if (++numPowerstonesCollected == NUM_POWERSTONES_TOTAL)
            {
                SetMicrogameEndText(true);
            }
        }
    }
}