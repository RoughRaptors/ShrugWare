using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

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

        private const int NUM_MESSAGES_TO_DISPLAY = 12;
        private const float CHAT_DELAY_MIN = 0.45f;
        private const float CHAT_DELAY_MAX = 0.85f;

        private int curMessageIndex = -1;
        private int chosenBadChatMessageIndex = -1; // which bad message we choose
        private int badChatMessageLocation = -1; // where in the whole message list we are placing the bad message
        private List<string> badChatMessages = new List<string> { "what the fuck is wrong with you", "fuck you", "go fuck yourself", "you're a bitch",
            "fuck you shithead", "you're an asshole", "i fucking hate you", "die in a fucking fire", "eat shit", " kys ", "piece of shit", "i wipe my face with your ass"};

        private List<string> chatMessages = new List<string> { "hey everyone", "how are you?", "hello", "i love you", "good job", "good dps", 
            "good healing", "what's up?", "nice cat", "longcat is long", "more dots", "whelps left side", "at least i have chicken", "i like animals",
            "what spec are you?", "how do i shot web?", "never gonna give you up", "chuck norris", "i'm eating tacos", "did you sleep ok?", "you just lost the game",
            "LFG tank", "LFG healer", "LFG dps", "LFG support", "50 DKP minus", "i am your father", "how's the weather", "it's hot here", "it's cold here",
            "get out of fire", "watch out for the fire", "stop standing in fire", "teleport me please", "what are the mechanics?", "drag the boss over here", "i farted" };

        private bool clickedCorrectMessage = false;

        protected override void Awake()
        {
            base.Awake();

            microGameTime *= 1.5f;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            chosenBadChatMessageIndex = UnityEngine.Random.Range(0, badChatMessages.Count);
            badChatMessageLocation = UnityEngine.Random.Range(0, NUM_MESSAGES_TO_DISPLAY - 1);

            float newMessageTime = UnityEngine.Random.Range(CHAT_DELAY_MIN, CHAT_DELAY_MAX);
            Invoke("PostNewMessage", newMessageTime);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return clickedCorrectMessage;
        }

        private void PostNewMessage()
        {
            scrollRect.verticalNormalizedPosition = 0;

            GameObject newChatMessage = Instantiate(chatMessageInitialObj, scrollViewContent.transform);
            //GameObject newChatMessage = Instantiate(chatMessageInitialObj);
            string message = "";

            // if we have enough chat messages, put the bad one in instead
            if (scrollViewContent.transform.childCount - 1 == badChatMessageLocation) 
            {
                message = badChatMessages[chosenBadChatMessageIndex];
            }
            else if (scrollViewContent.transform.childCount < NUM_MESSAGES_TO_DISPLAY)
            {
                int messageIndex = UnityEngine.Random.Range(0, chatMessages.Count);
                message = chatMessages[messageIndex];
                chatMessages.RemoveAt(messageIndex);
            }

            int currentMessageIndex = ++curMessageIndex;
            newChatMessage.GetComponent<Button>().onClick.AddListener(() => OnMessageClick(currentMessageIndex));
            newChatMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
            newChatMessage.SetActive(true);

            if (scrollViewContent.transform.childCount < NUM_MESSAGES_TO_DISPLAY)
            {
                float newMessageTime = UnityEngine.Random.Range(CHAT_DELAY_MIN, CHAT_DELAY_MAX);
                Invoke("PostNewMessage", newMessageTime);
            }
        }

        public void OnMessageClick(int messageIndex)
        {
            if(messageIndex == badChatMessageLocation)
            {
                clickedCorrectMessage = true;
            }

            SetMicrogameEndText(clickedCorrectMessage);
        }
    }
}