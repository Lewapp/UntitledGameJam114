using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement, looking, jumping, and gravity using Unity's CharacterController.
/// Uses the Input System for player input.
/// Should be attached to the main camera GameObject.
/// </summary>
public class PlayerMovement : MonoBehaviour, ITeleportable
{
    #region Interface Variables
    public bool canTeleport { get; set; }
    #endregion

    #region Public Variables
    [Header("Movement Settings")]
    public float moveSpeed = 5f;       // Player movement speed
    public float gravity = 9.81f;      // Gravity strength
    public float jumpHeight = 2f;      // Height of player's jump
    public float maxVelocity = 10f; // Maximum velocity cap for movement

    [Header("Look Settings")]
    public float mouseSensitivity = 5f; // Sensitivity for mouse/camera look
    public float controllerSensitivity = 100f; // Sensitivity for stick/camera look
    #endregion

    #region Private Variables   
    private CharacterController characterController; // Reference to the CharacterController component
    private bool usingController = false; // Flag to check if using a gamepad

    // Movement
    private Vector2 moveInput;         // Stores WASD/left stick movement input
    private Vector3 velocity;          // Tracks current movement velocity 

    // Looking
    private Vector2 lookInput;
    private float xRotation = 0f;      // Vertical look rotation value
    private Camera playerCamera;       // Reference to the player's camera
    #endregion

    #region Unity Events
    private void Start()
    {
        canTeleport = true; // Allow teleportation by default

        // Get the CharacterController from the GameObject
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing from " + this.name);
        }

        // Get the main camera in the scene
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Camera component is missing from the scene");
        }

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement(); // Handle WASD/left stick movement
        ApplyGravity();   // Apply gravity and handle falling
        HandleLook();     // Handle mouse/right stick look
    }
    #endregion 

    #region Update Methods
    /// <summary>
    /// Applies gravity to the player and moves them vertically.
    /// </summary>
    private void ApplyGravity()
    {
        if (characterController == null)
            return;

        // If grounded and moving downwards, reset downward velocity to a small push to stay grounded
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity over time
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity); // Cap downward velocity

        // Move the character based on current vertical velocity
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Handles player movement based on input.
    /// </summary>
    private void HandleMovement()
    {
        if (characterController == null)
            return;

        // Convert input into world-space movement direction
        Vector3 _move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Move the player using CharacterController
        characterController.Move(_move * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Handles looking around with the mouse or right stick.
    /// </summary>
    private void HandleLook()
    {
        if (playerCamera == null)
            return;

        // Determine sensitivity based on input type
        float _sensitivity = usingController ? controllerSensitivity * Time.deltaTime: mouseSensitivity;

        // Horizontal look (rotate player body)
        transform.Rotate(Vector3.up * lookInput.x * _sensitivity);

        // Vertical look (rotate camera)
        xRotation -= lookInput.y * _sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// Teleports the player to the specified position and rotation.
    /// </summary>
    public void Teleport(Vector3 position, Quaternion rotation, bool forceSolo)
    {
        if (!canTeleport)
            return; // If teleportation is not allowed, exit the method

        // Drop any held object if forceSolo is true
        if (forceSolo)
        {
            PlayerInteract playerInteract = GetComponent<PlayerInteract>();
            playerInteract?.DropHeldObject(); 
        }

        characterController.enabled = false;
        transform.position = position; // Set the player's position to the teleport location
        transform.rotation = Quaternion.Euler(0f, 0f, rotation.z); // Set the player's rotation to the teleport rotation
        characterController.enabled = true;
    }
    #endregion

    #region Input Actions
    /// <summary>
    /// Reads movement input from the Input System.
    /// </summary>
    public void MoveIA(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Store the movement input 
    }

    /// <summary>
    /// Handles jumping when grounded.
    /// </summary>
    public void JumpIA(InputAction.CallbackContext context)
    {
        if (characterController == null)
            return;

        // Only jump when input is performed and player is grounded
        if (context.performed && characterController.isGrounded)
        {
            // Calculate jump velocity 
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    /// <summary>
    /// Handles looking around with the mouse/right stick.
    /// </summary>
    public void LookIA(InputAction.CallbackContext context)
    {
        // Store look input 
        lookInput = context.ReadValue<Vector2>();

        // Detect device type dynamically from the callback context
        var device = context.control.device;

        if (device is Gamepad)
            usingController = true;
        else if (device is Mouse)
            usingController = false;
    }
    #endregion
}

