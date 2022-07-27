using System;
using UnityEngine;

/// <summary>
/// Sends relevant collisions as events to be picked up by microgrames
/// </summary>
public class PlayerCollider : MonoBehaviour
{
    [SerializeField] private LayerMask goodMasks = 0;
    [SerializeField] private LayerMask badMasks = 0;
    public static event Action<GameObject> OnGoodCollision;
    public static event Action<GameObject> OnBadCollision;
    public static event Action<GameObject> OnAnyCollision;

    private void OnTriggerEnter(Collider other)
    {
        OnAnyCollision?.Invoke(other.gameObject);

        if(goodMasks.HasLayer(other.gameObject.layer))
        {
            OnGoodCollision?.Invoke(other.gameObject);
        }
        else if(badMasks.HasLayer(other.gameObject.layer))
        {
            OnBadCollision?.Invoke(other.gameObject);
        }
    }
}
