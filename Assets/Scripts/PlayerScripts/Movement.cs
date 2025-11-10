// This first example shows how to move using Input System Package (New)

using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Movement : MonoBehaviour
{
    private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;
    private float rotationSpeed = 4.0f;

    // Public bool to turn double jump on or off. Fale = on. True = off.
    public bool turnDoubleJumpOff = false;
    private bool doubleJump = false;
    // Potential dash button to be made in the future (maybe)
    // private bool dash = false;


    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    private Vector3 lastMoveDirection = Vector3.zero;
    // Variable to hold what direction to "slide" down like mario64 when hitting a wall.
    private Vector3 lastWallNormal = Vector3.zero;

    [Header("Input Actions")]
    // Vector2 expected variable
    public InputActionReference moveAction; 
    // This variable is expecting a button
    public InputActionReference jumpAction;
    // Input shift button for this one \/
    public InputActionReference walkAction;

    [Header("Air Control Settings")]
    // How much control we can change in the air (0 = none, 1 = same as ground)
    public float airControlFactor = 0.3f; 
    // How quickly to slow down if no input in air.
    public float airDeceleration = 2.0f;

    [Header("Movement Speeds")]
    public float runSpeed = 4.0f;
    public float walkSpeed = 5.0f;

    [ReadOnly]
    public float currentSpeed = 0f;

    // Bool for enemy behavior
    public bool isWalking = false;

    // Setting horizontal velocity.
    private Vector3 currentHorizontalVelocity = Vector3.zero;

    // Coyote Time
    public float coyoteTime = 0.20f;
    public float coyoteTimeCounter;

    // Collisions
    CollisionFlags flags;


    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        groundedPlayer = controller.isGrounded;
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
        // Seeing whether player is grounded or not.
        groundedPlayer = controller.isGrounded;


        // Deciding what speed based on whether player is shifting or running
        // If walking is pressed, will be walk speed.
        // If not, then player is running, and will be set to run speed.
        currentSpeed = walkAction.action.IsPressed() ? walkSpeed : runSpeed;

        // If statement to check if player is on the ground
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            // Resets double jump 
            doubleJump = turnDoubleJumpOff;

            // Reset timer
            coyoteTimeCounter = coyoteTime;
            
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Read WASD input using the moveAction input.
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Gets direction of the WASD keys and maps them into a vector we can use.
        Vector3 inputDir = new Vector3(input.x, 0, input.y);

        // Clamp vector to just 1f so we can use it for movement. Might need to change this if we use joystick.
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);


        // Only allow movement changes if player is grounded
        if (groundedPlayer == true)
        {
            currentHorizontalVelocity = inputDir * currentSpeed;
        }
        else
        {

            // When there's input in the air.
            if (inputDir != Vector3.zero)
            {
                // Reduce the control in the air of the player
                Vector3 targetVelocity = inputDir * currentSpeed;
                currentHorizontalVelocity = Vector3.Lerp(
                       currentHorizontalVelocity,
                       targetVelocity,
                       airControlFactor * Time.deltaTime
                );


            }
            // For when there's no input in the air. It'll gradually slowdown the character.
            else
            {
                // Same lerp function but use airDeceleration since the player isn't inputting anything and Vector3.zero for the 2nd vector 
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
        jumpFunction(inputDir, currentSpeed);

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = currentHorizontalVelocity + (playerVelocity.y * Vector3.up);

        // Walking = player is pressing any movement input and NOT holding shift
        isWalking = walkAction.action.IsPressed();

        // Actually move and get collision flags
        flags = controller.Move(finalMove * Time.deltaTime);

        // If we hit a wall, slide along it
        if ((flags & CollisionFlags.Sides) != 0 && lastWallNormal != Vector3.zero)
        {
            // Remove movement "into" the wall
            lastMoveDirection = Vector3.ProjectOnPlane(lastMoveDirection, lastWallNormal).normalized;
        }
    }


    /// <summary>
    /// Function that handles jumping and double jumping
    /// </summary>
    /// <param name="move"></param>
    void jumpFunction(Vector3 inputDir, float currentSpeed)
    {
        // Regular jump
        if (jumpAction.action.triggered && (groundedPlayer || coyoteTimeCounter > 0f))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            groundedPlayer = controller.isGrounded;
            coyoteTimeCounter = 0f;
        }
        // Double jump
        else if (jumpAction.action.triggered && doubleJump == false)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            // Prevents another double jump to happen.
            doubleJump = true;
            groundedPlayer = controller.isGrounded;

            // This if input checks if the direction is different, which allows the player to jump to a different side much easier.
            // Disallow this if we don't want the player to be able to do this.
            if (inputDir != Vector3.zero)
            {
                // Calculates new Horizontal velocity to be changed and accomodated to the Update() function.
                currentHorizontalVelocity = inputDir.normalized * currentSpeed;
            }
        }


        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        
        Vector3 finalMove = (inputDir * currentSpeed) + (playerVelocity.y * Vector3.up);
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
