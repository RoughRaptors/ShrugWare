using UnityEngine;
using UnityEngine.InputSystem;

namespace ShrugWare
{
    public class PlayerMover : MonoBehaviour
    {
        private Microgame myMicrogame = null;
        private PlayerInput playerInput = null;
        private InputAction movement => playerInput.Map.Movement;
        private Rigidbody rb = null;

        [SerializeField] private float moveSpeed = 60f;
        [SerializeField] private bool moveUp = true;
        [SerializeField] private bool moveDown = true;
        [SerializeField] private bool moveLeft = true;
        [SerializeField] private bool moveRight = true;
    

        private void Awake()
        {
            myMicrogame = FindObjectOfType<Microgame>();
            playerInput = new PlayerInput();
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if(myMicrogame != null)
            {
                myMicrogame.MicrogameStarted += EnableMovement;
                myMicrogame.MicrogameEnded += DisableMovement;
            }

            movement.performed += StartMove;
            movement.canceled += StopMove;
        }

        private void OnDisable()
        {
            if(myMicrogame != null)
            {
                myMicrogame.MicrogameStarted -= DisableMovement;
                myMicrogame.MicrogameEnded -= DisableMovement;
            }

            movement.Disable();
            movement.performed -= StartMove;
            movement.canceled -= StopMove;
        }

        private void EnableMovement() => movement.Enable();
        public void DisableMovement()
        {
            rb.velocity = Vector3.zero;
            movement.Disable();
        }

        private void StartMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            float minX = moveLeft ? -1 : 0;
            float maxX = moveRight ? 1 : 0;
            input.x = Mathf.Clamp(input.x, minX, maxX);

            float minY = moveDown ? -1 : 0;
            float maxY = moveUp ? 1 : 0;
            input.y = Mathf.Clamp(input.y, minY, maxY);
            rb.velocity = input * moveSpeed;
        }

        private void StopMove(InputAction.CallbackContext context) => rb.velocity = Vector3.zero;
    }
}