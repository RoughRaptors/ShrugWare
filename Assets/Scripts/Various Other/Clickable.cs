using System;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public event Action<Clickable> Clicked;
    private void OnMouseDown()
    {
        Clicked?.Invoke(this);
    }
}
