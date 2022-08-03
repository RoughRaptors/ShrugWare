using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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

        private enum RotationMapping
        {
            Frostbolt = 0,
            GlacialSpike = 1,
            IceBlast = 2
        }

        private List<RotationMapping> rotation = new List<RotationMapping>();

        protected override void Start()
        {
            base.Start();

            // get a random, 3 button rotation
            rotation.Add((RotationMapping)0);
            rotation.Add((RotationMapping)1);
            rotation.Add((RotationMapping)2);
            rotation = rotation.OrderBy(i => Random.value).ToList();
            startText = rotation[0].ToString() + " -> " + rotation[1].ToString() + " -> " + rotation[2];

            frostboltButtonObj.onClick.AddListener(() => ButtonPressed(frostboltButtonObj, RotationMapping.Frostbolt));
            glacialSpikeButtonObj.onClick.AddListener(() => ButtonPressed(glacialSpikeButtonObj, RotationMapping.GlacialSpike));
            iceBlastButtonObj.onClick.AddListener(() => ButtonPressed(iceBlastButtonObj, RotationMapping.IceBlast));
        }

        protected override bool VictoryCheck() => rotation.Count == 0;

        private void ButtonPressed(Button button, RotationMapping rotationElement)
        {
            if(rotation[0] == rotationElement)
            {
                rotation.RemoveAt(0);
                button.gameObject.SetActive(false);
            }
            else
            {
                frostboltButtonObj.gameObject.SetActive(false);
                glacialSpikeButtonObj.gameObject.SetActive(false);
                iceBlastButtonObj.gameObject.SetActive(false);
                SetMicrogameEndText(false, "Wrong rotation");
            }
            
            if(rotation.Count == 0)
            {
                SetMicrogameEndText(true);
            }
        }
    }
}