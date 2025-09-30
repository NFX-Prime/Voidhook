// This first example shows how to move using Input System Package (New)

using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Movement : MonoBehaviour
{
    private float playerSpeed = 5.0f;
    private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;
    private float rotationSpeed = 4.0f;

    private bool doubleJump = false;
    private bool dash = false;


    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    private Vector3 lastMoveDirection = Vector3.zero;
    // Variable to track if direction was reset midair
    private bool usedDoubleJumpDirection = false;

    [Header("Input Actions")]
    public InputActionReference moveAction; // expects Vector2
    public InputActionReference jumpAction; // expects Button

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            // Resets double jump 
            doubleJump = false;
            // Reset direction of double jump
            usedDoubleJumpDirection = false;
        }

        // Read input only if in joint in the
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        // Only allow movement changes if player is grounded
        if (groundedPlayer == true)
        {
            lastMoveDirection = move;
        }
        else
        {

            // If statement to see if the player has a double jump and still wants to jump in a different direction.
            if (doubleJump && !usedDoubleJumpDirection && move != Vector3.zero)
            {
                lastMoveDirection = move;
                usedDoubleJumpDirection = true;
            }
            // This makes it so that when in the air, it'll commit to the last move direction for a comitted jump
            move = lastMoveDirection;
        }

        // Rotation only when moving on ground.
        if (groundedPlayer && move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.forward = move;
        }

        // Calling the jump function
        jumpFunction(move);


        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);

        
    }


    void jumpFunction(Vector3 move)
    {
        // Jump
        if (jumpAction.action.triggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            groundedPlayer = controller.isGrounded;

        }
        else if (jumpAction.action.triggered && doubleJump == false)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            // Prevents another double jump to happen.
            doubleJump = true;
            groundedPlayer = controller.isGrounded;
        }


        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);

    }
}
