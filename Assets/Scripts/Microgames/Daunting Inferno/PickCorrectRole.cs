using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class PickCorrectRole : Microgame
    {
        [SerializeField]
        AudioClipData buttonClickAudio;

        [SerializeField]
        GameObject queueTankButton;

        [SerializeField]
        GameObject queueDPSButton;

        [SerializeField]
        GameObject queueHealerButton;

        [SerializeField]
        GameObject queueSupportButton;

        [SerializeField]
        GameObject rolesObj;

        [SerializeField]
        GameObject tankButton;

        [SerializeField]
        GameObject dpsButton;

        [SerializeField]
        GameObject healerButton;

        [SerializeField]
        GameObject supportButton;

        public enum Roles
        {
            Tank = 0,
            DPS = 1,
            Healer = 2,
            Support = 3,
        }

        private Roles missingRole;
        private bool pickedCorrectly = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            SetupRoles();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return pickedCorrectly;
        }

        private void SetupRoles()
        {
            queueTankButton.SetActive(true);
            queueDPSButton.SetActive(true);
            queueHealerButton.SetActive(true);
            queueSupportButton.SetActive(true);

            tankButton.SetActive(true);
            dpsButton.SetActive(true);
            healerButton.SetActive(true);
            supportButton.SetActive(true);

            rolesObj.SetActive(true);

            RandomizeQueueButtons();

            int randomRole = UnityEngine.Random.Range(0, 4);
            if (randomRole == (int)Roles.Tank)
            {
                missingRole = Roles.Tank;
                tankButton.SetActive(false);
            }
            else if (randomRole == (int)Roles.DPS)
            {
                missingRole = Roles.DPS;
                dpsButton.SetActive(false);
            }
            else if (randomRole == (int)Roles.Healer)
            {
                missingRole = Roles.Healer;
                healerButton.SetActive(false);
            }
            else
            {
                missingRole = Roles.Support;
                supportButton.SetActive(false);
            }
        }

        private void RandomizeQueueButtons()
        {
            List<Vector2> buttonPositions = new List<Vector2>();
            buttonPositions.Add(queueTankButton.transform.localPosition);
            buttonPositions.Add(queueDPSButton.transform.localPosition);
            buttonPositions.Add(queueHealerButton.transform.localPosition);
            buttonPositions.Add(queueSupportButton.transform.localPosition);

            int newTankPosIndex = UnityEngine.Random.Range(0, buttonPositions.Count);
            queueTankButton.transform.localPosition = new Vector3(buttonPositions[newTankPosIndex].x, buttonPositions[newTankPosIndex].y);
            buttonPositions.RemoveAt(newTankPosIndex);

            int newDPSPosIndex = UnityEngine.Random.Range(0, buttonPositions.Count);
            queueDPSButton.transform.localPosition = new Vector3(buttonPositions[newDPSPosIndex].x, buttonPositions[newDPSPosIndex].y);
            buttonPositions.RemoveAt(newDPSPosIndex);

            int newHealerPosIndex = UnityEngine.Random.Range(0, buttonPositions.Count);
            queueHealerButton.transform.localPosition = new Vector3(buttonPositions[newHealerPosIndex].x, buttonPositions[newHealerPosIndex].y);
            buttonPositions.RemoveAt(newHealerPosIndex);

            int newSupportPosIndex = UnityEngine.Random.Range(0, buttonPositions.Count);
            queueSupportButton.transform.localPosition = new Vector3(buttonPositions[newSupportPosIndex].x, buttonPositions[newSupportPosIndex].y);
            buttonPositions.RemoveAt(newSupportPosIndex);
        }

        public void OnRoleClicked(int role)
        {
            if(role == (int)missingRole)
            {
                pickedCorrectly = true;
            }

            queueTankButton.GetComponent<Button>().interactable = false;
            queueDPSButton.GetComponent<Button>().interactable = false;
            queueHealerButton.GetComponent<Button>().interactable = false;
            queueSupportButton.GetComponent<Button>().interactable = false;

            SetMicrogameEndText(pickedCorrectly);
        }
    }
}