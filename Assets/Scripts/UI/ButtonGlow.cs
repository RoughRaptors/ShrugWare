using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    Sprite buttonUnselected;

    // what we show when we mouse over the object
    [SerializeField]
    GameObject glow;

    [SerializeField]
    Sprite buttonPressed;

    public void OnPointerEnter(PointerEventData eventData)
    {
        glow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        glow.SetActive(false);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        GetComponent<Button>().image.sprite = buttonPressed;
    }
}
