using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

namespace ShrugWare
{
    // create a list of 3 abilities and randomize them, we need to press their buttons in the correct order
    public class PerformTheRotation : Microgame
    {
        [SerializeField]
        Button frostboltButtonObj = null;

        [SerializeField]
        Button glacialSpikeButtonObj = null;

        [SerializeField]
        Button iceBlastButtonObj = null;

        [SerializeField]
        TextMeshProUGUI instructionsText = null;

        [SerializeField]
        GameObject bossObj;

        [SerializeField]
        AudioClipData frostboltAudio;

        [SerializeField]
        AudioClipData iceBlastAudio;

        [SerializeField]
        AudioClipData glacialSpikeAudio;

        private enum RotationMapping
        {
            Frostbolt = 0,
            GlacialSpike = 1,
            IceBlast = 2
        }

        private List<RotationMapping> rotation = new List<RotationMapping>();

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            // get a random, 3 button rotation
            rotation.Add((RotationMapping)0);
            rotation.Add((RotationMapping)1);
            rotation.Add((RotationMapping)2);
            rotation = rotation.OrderBy(i => Random.value).ToList();

            startText = rotation[0].ToString() + " -> " + rotation[1].ToString() + " -> " + rotation[2];
            instructionsText.text = startText;
            instructionsText.enabled = true;

            frostboltButtonObj.onClick.AddListener(() => ButtonPressed(frostboltButtonObj, RotationMapping.Frostbolt));
            glacialSpikeButtonObj.onClick.AddListener(() => ButtonPressed(glacialSpikeButtonObj, RotationMapping.GlacialSpike));
            iceBlastButtonObj.onClick.AddListener(() => ButtonPressed(iceBlastButtonObj, RotationMapping.IceBlast));

            // this is a hard microgame when too fast and, there's not enough time, give it a time boost the faster the timescale is
            if(BossGameManager.Instance != null)
            {
                microGameTime += BossGameManager.Instance.GetCurTimeScale() * 2;
            }

            bossObj.SetActive(true);
        }

        protected override bool VictoryCheck() => rotation.Count == 0;

        private void ButtonPressed(Button button, RotationMapping rotationElement)
        {
            if(rotation[0] == rotationElement)
            {
                if(AudioManager.Instance != null)
                {
                    if(rotationElement == RotationMapping.Frostbolt)
                    {
                        AudioManager.Instance.PlayAudioClip(frostboltAudio);
                    }
                    else if (rotationElement == RotationMapping.GlacialSpike)
                    {
                        AudioManager.Instance.PlayAudioClip(glacialSpikeAudio);
                    }
                    else
                    {
                        AudioManager.Instance.PlayAudioClip(iceBlastAudio);
                    }
                }

                rotation.RemoveAt(0);
                button.interactable = false;
            }
            else
            {
                frostboltButtonObj.interactable = false;
                glacialSpikeButtonObj.interactable = false;
                iceBlastButtonObj.interactable = false;
                //SetMicrogameEndText(false, "Wrong rotation");
            }
            
            if(rotation.Count == 0)
            {
                SetMicrogameEndText(true);
            }
        }
    }
}