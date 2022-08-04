using UnityEngine;

/// <summary>
/// Sets box colliders to border a camera
/// Useful for setting player boundaries in relation to the camera boundaries
/// </summary>
public class AlignCollidersToCamera : MonoBehaviour
{
    #region //Cached Components
    [Header("Colliders")]
    [SerializeField] private ColliderInfo leftBoxCollider = new ColliderInfo();
    [SerializeField] private ColliderInfo rightBoxCollider = new ColliderInfo();
    [SerializeField] private ColliderInfo topBoxCollider = new ColliderInfo();
    [SerializeField] private ColliderInfo bottomBoxCollider = new ColliderInfo();
    #endregion

    
    #region //Monobehaviour
    private void OnValidate()
    {
        Start();
    }

    private void Start()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.z = transform.position.z;
        transform.position = pos;
        PositionPlayerBounds();
    }
    #endregion


    #region //Position player bounds around camera bounds
    [ContextMenu("Make uniform off left")]
    public void Uniform()
    {
        rightBoxCollider = new ColliderInfo(rightBoxCollider.collider, leftBoxCollider);
        topBoxCollider = new ColliderInfo(topBoxCollider.collider, leftBoxCollider);
        bottomBoxCollider = new ColliderInfo(bottomBoxCollider.collider, leftBoxCollider);
        PositionPlayerBounds();
    }

    [ContextMenu("Align All")]
    public void AlignAll()
    {
        foreach(var comp in FindObjectsOfType<AlignCollidersToCamera>())
            comp.PositionPlayerBounds();
    }

    [ContextMenu("Align")]
    public void PositionPlayerBounds()
    {
        if(Camera.main == null) return;
        SetupBoundary(leftBoxCollider, true, false);
        SetupBoundary(rightBoxCollider, true, true);
        SetupBoundary(topBoxCollider, false, true);
        SetupBoundary(bottomBoxCollider, false, false);
    }

    private void SetupBoundary(ColliderInfo info, bool isHorizontal, bool topOrRight)
    {
        if(info.collider == null) return;
        Camera cam = Camera.main;

        //Ortho sizing
        float halfWidth = 0;
        float halfHeight = 0;

        if(cam.orthographic)
        {
            halfWidth = cam.aspect * cam.orthographicSize;
            halfHeight = halfWidth / cam.aspect;
        }
        else
        {
            float vFOV = cam.fieldOfView;
            float hFOV = Camera.VerticalToHorizontalFieldOfView(vFOV, cam.aspect);
            float distance = Vector3.Distance(transform.position, cam.transform.position);
            halfWidth = distance * Mathf.Tan(hFOV * Mathf.Deg2Rad/2);
            halfHeight = distance * Mathf.Tan(vFOV * Mathf.Deg2Rad/2);
        }

        //Determine indices to use based off boundary direction
        int mainIndex = isHorizontal ? 0 : 1;
        int offIndex = 1 - mainIndex;
        
        //Resize bound
        Vector3 newSize = Vector3.one;
        newSize[mainIndex] = info.boundaryWidth;
        newSize[offIndex] = (isHorizontal ? halfHeight : halfWidth) * 2 + 2 * info.overflow;
        info.collider.size = newSize;

        //Determine main axis positioning
        float halfSize = newSize[mainIndex]/2;
        float mainAxisOffset = halfSize + info.offset;
        
        //Reposition bound
        Vector2 newPosition = Vector2.zero;
        int offsetDir = (topOrRight ? 1: -1);
        newPosition[mainIndex] = ((isHorizontal ? halfWidth : halfHeight) + mainAxisOffset) * offsetDir;
        newPosition[offIndex] = 0;
        info.collider.transform.localPosition = newPosition;
    }

    private void SetupOrtho(ColliderInfo info, bool isHorizontal, bool topOrRight)
    {

    }
    #endregion

    [System.Serializable]
    private struct ColliderInfo
    {
        public BoxCollider collider;
        [Tooltip("Distance from polygon collider")]
        public float offset;
        public float boundaryWidth;
        [Tooltip("Amount boundary exceeds the polygon collider per side")]
        public float overflow;

        public ColliderInfo(BoxCollider collider, ColliderInfo copy)
        {
            this.collider = collider;
            this.offset = copy.offset;
            this.boundaryWidth = copy.boundaryWidth;
            this.overflow = copy.overflow;
        }
    }
}