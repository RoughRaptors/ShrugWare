using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

namespace ShrugWare
{
    public class ReportChatMessage : Microgame
    {
        [SerializeField]
        AudioClipData buttonClickAudio;

        [SerializeField]
        GameObject scrollViewContent;

        [SerializeField]
        GameObject chatMessageInitialObj;

        [SerializeField]
        ScrollRect scrollRect;

        int index = 0;
        private List<string> characterNames = new List<string> { "1", "2", "3", "4"};

        private List<string> chatMessages = new List<string> { "a", "b", "c", "d"};

        private bool clickedCorrectMessage = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            scrollRect.verticalNormalizedPosition = 0;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject newChatMessage = Instantiate(chatMessageInitialObj, scrollViewContent.transform);
                newChatMessage.GetComponentInChildren<TextMeshPro>().text = index++.ToString();
                //Resize(5, new Vector2(1, 0), ref newChatMessage);
                newChatMessage.SetActive(true);
            }
        }

        protected override bool VictoryCheck()
        {
            return clickedCorrectMessage;
        }

        private void Resize(float amount, Vector3 direction, ref GameObject go)
        {
            go.transform.position += direction * amount / 2; // Move the object in the direction of scaling, so that the corner on ther side stays in place
            go.transform.localScale += direction * amount; // Scale object in the specified direction
        }
    }
}