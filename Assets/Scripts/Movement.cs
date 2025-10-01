// This first example shows how to move using Input System Package (New)

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
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
    // Variable to hold what direction to "slide" down like mario64 when hitting a wall.
    private Vector3 lastWallNormal = Vector3.zero;

    [Header("Input Actions")]
    // Vector2 expected variable
    public InputActionReference moveAction; 
    // This variable is expecting a button
    public InputActionReference jumpAction; 

    [Header("Air Control Settings")]
    // How much control we can change in the air (0 = none, 1 = same as ground)
    public float airControlFactor = 0.3f; 
    // How quickly to slow down if no input in air.
    public float airDeceleration = 2.0f;  

    // Setting horizontal velocity.
    private Vector3 currentHorizontalVelocity = Vector3.zero;


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

        // If statement to check if player is on the ground
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

        Vector3 inputDir = new Vector3(input.x, 0, input.y);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);


        // Only allow movement changes if player is grounded
        if (groundedPlayer == true)
        {
            currentHorizontalVelocity = inputDir * playerSpeed;
        }
        else
        {

            // when there's input in the air.
            if (inputDir != Vector3.zero)
            {
                // Reduce the control in the air of the player
                Vector3 targetVelocity = inputDir * playerSpeed;
                currentHorizontalVelocity = Vector3.Lerp(
                       currentHorizontalVelocity,
                       targetVelocity,
                       airControlFactor * Time.deltaTime
                );


            }
            // For when there's no input in the air. It'll gradually slowdown the character.
            else
            {
                currentHorizontalVelocity = Vector3.Lerp(
                    currentHorizontalVelocity,
                    Vector3.zero,
                    airDeceleration * Time.deltaTime
                );

            }
        }

        // Rotation only when moving on ground.
        if (currentHorizontalVelocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentHorizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.forward = currentHorizontalVelocity;
        }

        // Calling the jump function
        jumpFunction(inputDir);

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = currentHorizontalVelocity + (playerVelocity.y * Vector3.up);


        // Actually move and get collision flags
        CollisionFlags flags = controller.Move(finalMove * Time.deltaTime);

        // If we hit a wall, slide along it
        if ((flags & CollisionFlags.Sides) != 0 && lastWallNormal != Vector3.zero)
        {
            // Remove movement "into" the wall
            lastMoveDirection = Vector3.ProjectOnPlane(lastMoveDirection, lastWallNormal).normalized;
        }
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

    /// <summary>
    /// Helper Function that saves how the player hit the wall and what side the wall is facing.
    /// </summary>
    /// <param name="hit"></param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Store wall normal
        if ((controller.collisionFlags & CollisionFlags.Sides) != 0)
        {
            lastWallNormal = hit.normal;
        }
    }
}
