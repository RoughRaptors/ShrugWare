using ShrugWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIObject : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    AudioClipData dragAudio;

    private GameObject lastPickupObject;

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, 
            pointerData.position, canvas.worldCamera, out pos);

        transform.position = canvas.transform.TransformPoint(pos);

        if (AudioManager.Instance != null && dragAudio != null && lastPickupObject != pointerData.pointerPress)
        {
            lastPickupObject = pointerData.pointerPress;
            AudioManager.Instance.PlayAudioClip(dragAudio);
        }
    }
}
