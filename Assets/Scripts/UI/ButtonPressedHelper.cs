using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShrugWare
{
    public class ButtonPressedHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool buttonPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            buttonPressed = true;
            GetComponent<Button>().OnPointerClick(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            buttonPressed = false;
        }
    }
}