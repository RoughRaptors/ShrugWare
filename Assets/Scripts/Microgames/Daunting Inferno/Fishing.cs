using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace ShrugWare
{
    public class Fishing : Microgame
    {
        [SerializeField]
        GameObject reelInButton;

        [SerializeField]
        GameObject fishObj;

        [SerializeField]
        AudioClipData buttonClickAudio;

        [SerializeField]
        GameObject fishingPoleObj;

        [SerializeField]
        GameObject bobObj;

        [SerializeField]
        AudioClipData biteAudio;

        [SerializeField]
        AudioClipData waterAudio;

        private const float BOB_BITE_DISTANCE = 0.35f;
        private const float FISH_SPEED = 1.25f;

        // take some time to spawn to make it challenging
        private bool caughtFish = false;
        private bool fishMoving = false;
        private bool hasBitten = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            bool leftSetup = UnityEngine.Random.Range(0, 2) == 0;
            if (!leftSetup)
            {
                fishObj.transform.rotation = new Quaternion(0, 180, 0, 0);
                fishingPoleObj.transform.Rotate(new Vector3(0, 180, 0));
                fishingPoleObj.transform.position = new Vector3(3.3f, -0.75f, 3.3f);
            }

            float prepareDelay = UnityEngine.Random.Range(1.0f, 2.3f);
            Invoke("PrepareFishToBite", prepareDelay);
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime *= 1.25f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(waterAudio);
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            if(fishMoving)
            {
                fishObj.transform.position = Vector3.MoveTowards(fishObj.transform.position, bobObj.transform.position, FISH_SPEED * Time.deltaTime);
                float distance = Vector3.Distance(fishObj.transform.position, bobObj.transform.position);
                if (distance <= BOB_BITE_DISTANCE && !hasBitten)
                {
                    if(AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(biteAudio, true);
                    }

                    hasBitten = true;
                    bobObj.SetActive(false);
                    fishingPoleObj.transform.Rotate(new Vector3(8.0f, 0, 0));
                }
            }
        }

        protected override bool VictoryCheck() => caughtFish;

        public void ReelInButtonPressed()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(buttonClickAudio);
            }

            if(!hasBitten)
            {
                fishingPoleObj.SetActive(false);
                fishObj.SetActive(false);
                SetMicrogameEndText(false);
            }
            else
            {
                caughtFish = true;
                SetMicrogameEndText(true);
            }
        }

        private void MoveFish()
        {
            fishMoving = true;
        }

        private void PrepareFishToBite()
        {
            float biteDelay = UnityEngine.Random.Range(0.25f, 1.05f);
            Invoke("MoveFish", biteDelay);

            fishObj.transform.Rotate(new Vector3(0, 180, 0));  
        }
    }
}