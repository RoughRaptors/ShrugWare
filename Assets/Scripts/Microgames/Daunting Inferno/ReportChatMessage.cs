using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private int index = 0;

        private List<string> characterNames = new List<string> { "1", "2", "3", "4"};

        private List<string> chatMessages = new List<string> { "a", "b", "c", "d"};

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

            if(Input.GetKeyDown(KeyCode.Space))
            {
                ++index;
                GameObject newChatMessage = Instantiate(chatMessageInitialObj, scrollViewContent.transform);
                newChatMessage.GetComponentInChildren<TextMeshPro>().text = index.ToString();
                newChatMessage.SetActive(true);
            }
        }

        protected override bool VictoryCheck()
        {
            return true;
        }
    }
}