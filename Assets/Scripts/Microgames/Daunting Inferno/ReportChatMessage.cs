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

            if (Input.GetKeyDown(KeyCode.Space) && chatMessages.Count > 0)
            {
                GameObject newChatMessage = Instantiate(chatMessageInitialObj, scrollViewContent.transform);

                int charNameIndex = UnityEngine.Random.Range(0, characterNames.Count);
                string charName = characterNames[charNameIndex];
                characterNames.RemoveAt(charNameIndex);

                int messageIndex = UnityEngine.Random.Range(0, chatMessages.Count);
                string message = chatMessages[messageIndex];
                chatMessages.RemoveAt(messageIndex);

                newChatMessage.GetComponentInChildren<TextMeshProUGUI>().text = charName + ": " + message;
                newChatMessage.SetActive(true);
            }
        }

        protected override bool VictoryCheck()
        {
            return clickedCorrectMessage;
        }
    }
}