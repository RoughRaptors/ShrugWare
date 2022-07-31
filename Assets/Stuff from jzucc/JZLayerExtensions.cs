using UnityEngine;

public static class JZLayerExtensions
{
    public static bool HasLayer(this LayerMask mask, int layer)
    {
        return mask == (mask | 1 << layer);
    }
}
