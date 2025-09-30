using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Self] Animator animator;
        // Cinemachine doesn't work and we don't really need it right now.
        // [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        Transform mainCam;

        float currentSpeed;
        float velocity;

        const float ZeroF = 0f;

        private void Awake() {
            mainCam = Camera.main.transform;


        }

        private void Update()
        {
            HandleMovement();
            // Handle UpdateAnimator(); later
        }

        void HandleMovement()
        {
            var movementDirection = new Vector3(input.Direction.x, y: 0f, z: input.Direction.y).normalized;

            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection;


            var adjustedMovement = movementDirection * moveSpeed * Time.deltaTime;

            if (adjustedDirection.magnitude > ZeroF)

            HandleRotation(adjustedDirection);
            controller.Move(adjustedMovement);

            currentSpeed = Mathf.SmoothDamp(currentSpeed, adjustedMovement.magnitude, ref velocity, smoothTime);

        }

        float SmoothSpeed(float value)
        {
            return Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
        
        void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        

    }

}