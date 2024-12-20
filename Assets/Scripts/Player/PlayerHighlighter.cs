using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

namespace ShrugWare
{
    // start off with the player object flashing to make it clear it is you
    public class PlayerHighlighter : MonoBehaviour
    {
        const float TIME_TO_STOP = 1.5f;
        float timeElapsed = 0;

        private void Update()
        {
            timeElapsed += Time.deltaTime;
        }

        private void OnEnable()
        {
            SwapColor();
        }

        private void SwapColor()
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if(renderer.color == Color.red)
                {
                    renderer.color = Color.white;
                }
                else
                {
                    renderer.color = Color.red;
                }
            }

            if (timeElapsed < TIME_TO_STOP)
            {
                Invoke("SwapColor", 0.2f);
            }
            else
            {
                // make sure we end up back to normal
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.color = Color.white;
                }

                // stop running this, time is up
                this.enabled = false;
            }
        }
    }
}