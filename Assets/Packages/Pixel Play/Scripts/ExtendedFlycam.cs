using UnityEngine;

/*
FEATURES
    WASD/Arrows:    Movement
    Q:    Climb
    E:    Drop
    Shift:    Move faster
    Control:    Move slower
*/
public class ExtendedFlycam : MonoBehaviour
{
    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime * transform.forward;
            transform.position += (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime * transform.right;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime * Time.deltaTime * transform.forward;
            transform.position += (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime * transform.right;
        }
        else
        {
            transform.position += normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime * transform.forward;
            transform.position += normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime * transform.right;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += climbSpeed * Time.deltaTime * transform.up; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= climbSpeed * Time.deltaTime * transform.up; ; }
    }
}