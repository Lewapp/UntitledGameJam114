using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles a range of movement for a GameObject, allowing it to move smoothly within a defined range.
/// Moves based on user interaction, and ensures that attached objects move with it.
/// </summary>
public class LerpRange : MonoBehaviour, IInteractable
{
    #region Inspector Variables
    [Header("Set Up Settings")]
    [SerializeField] private bool setCentreAsPosition = true; // If true, the central position will be set to the GameObject's position
    [SerializeField] private Vector3 centralPosition; // The central position of the range, used for calculations
    [SerializeField] private Vector3 minRange; // The minimum range from the central position
    [SerializeField] private Vector3 maxRange; // The maximum range from the central position
    [SerializeField] private Vector3 collisionRange; // The range used for collision checks

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed; // The speed at which the GameObject moves
    [SerializeField] private LayerMask ignoreMask; // Layer mask to ignore certain layers during movement checks
    [SerializeField] private Collider checkCollider;  // Collider used to check for interactions with other objects
    #endregion

    #region Private Variables
    private List<Transform> attachedObjects; // List of objects that are attached to this GameObject and should move with it
    private Vector3 lastPosition; // The last position of the GameObject, used to calculate movement deltas
    private Coroutine currentMovement; // Saved Coroutine for handling smooth movement
    #endregion

    #region Unity Events
    private void Start()
    {
        attachedObjects = new List<Transform>();
    }

    private void FixedUpdate()
    {
        // Calculate the change in position since the last frame
        Vector3 _delta = transform.position - lastPosition;

        for (int i = attachedObjects.Count - 1; i >= 0; i--)
        {
            attachedObjects[i].position += _delta;


            Rigidbody rb = attachedObjects[i].GetComponent<Rigidbody>();
            if (!rb)
                continue;

            if (!rb.isKinematic)
                continue;

            attachedObjects.Remove(attachedObjects[i]);
        }
        // If there are attached objects, move them by the same delta
        foreach (Transform ao in attachedObjects)
        {
            if (!ao)
                continue;

        }

        // Save the current position as the last position for the next frame
        lastPosition = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        // If the object is already attached, do nothing
        if (attachedObjects.Contains(other.transform))
            return;

        // Attach the object to this GameObject
        attachedObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object is considered attached, remove it from the list
        if (attachedObjects.Contains(other.transform))
            attachedObjects.Remove(other.transform);
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// Sets direction based on user input and starts a coroutine to move the GameObject smoothly in that direction.
    /// </summary>
    public void Interact(InteractableData data)
    {
        // If the interaction as been released, stop any ongoing movement
        if (!data.isPressed)
        {
            if (currentMovement != null)
                StopCoroutine(currentMovement);

            currentMovement = null;
            return;
        }

        Vector3 direction = Vector3.zero;

        // Determine the direction based on the idPress value
        switch (data.idPress)
        {
            case 1: // Move Left
                direction = Vector3.left;
                break;
            case 2: // Move Right
                direction = Vector3.right;
                break;
            case 3: // Move Forward
                direction = Vector3.forward;
                break;
            case 4: // Move Back
                direction = Vector3.back;
                break;
            case 5: // Move Up
                direction = Vector3.up;
                break;
            default: // Move Down
                direction = Vector3.down;
                break;
        }

        // Stop any ongoing movement and start a new one
        if (currentMovement != null)
            StopCoroutine(currentMovement);

        currentMovement = StartCoroutine(SmoothMove(direction));
    }
    #endregion

    #region Methods
    /// <summary>
    /// Checks if the given position is within the defined range.
    /// </summary>
    private bool IsWithinRange(Vector3 position)
    {
        // Sets the min and max range based on the central position and the defined ranges
        Vector3 _min = centralPosition + minRange;
        Vector3 _max = centralPosition + maxRange;

        // Check if the position is within the defined range
        return position.x >= _min.x && position.x <= _max.x &&
               position.y >= _min.y && position.y <= _max.y &&
               position.z >= _min.z && position.z <= _max.z;
    }

    /// <summary>
    /// Checks if the GameObject can move to the target position without colliding with other objects.
    /// </summary>
    private bool CanMoveTo(Vector3 targetPos)
    {
        // If the target position hits a collider, return false
        return !Physics.CheckBox(
            targetPos,
            collisionRange,
            Quaternion.identity,
            ~ignoreMask,
            QueryTriggerInteraction.Ignore
        );
    }
    #endregion

    #region Coroutines
    private IEnumerator SmoothMove(Vector3 direction)
    {
        yield return new WaitForEndOfFrame();

        // If this Coroutine is allowed to keep running, continue with the movement
        while (currentMovement != null)
        {
            // Timing
            float _elapsed = 0f;
            float _duration = 0.2f;

            // Position
            Vector3 _startPos = transform.position;
            Vector3 _endPos = _startPos + direction * moveSpeed * _duration; // Calculate the end position based on the direction and speed

            // Checks
            bool _isWithinRange = IsWithinRange(_endPos);
            bool _canMove = CanMoveTo(_endPos);

            // If the end position is  within range or can move, move towards it
            while (_elapsed < _duration && _isWithinRange && _canMove)
            {
                // Smoothly interpolate the position between the start and end positions
                transform.position = Vector3.Lerp(_startPos, _endPos, _elapsed / _duration);
                _elapsed += Time.deltaTime;
                yield return null;
                // Check if the end position is still within range and can move
                _canMove = CanMoveTo(_endPos);
            }

            // If the end position is not within range or cannot move, reset to the start position
            transform.position = _isWithinRange && _canMove ? _endPos : _startPos;
            yield return null; // Wait until the current movement is finished
        }
    }
    #endregion

    #region Editor Code
    private void OnDrawGizmosSelected()
    {
        // Reset central position if setCentreAsPosition is true
        if (setCentreAsPosition)
        {
            centralPosition = transform.position; // Set the central position to the GameObject's position
        }

        // Draw a wireframe cube to visualize the range in the editor
        Gizmos.color = Color.green; // Set the color of the Gizmos
        Gizmos.DrawWireCube(centralPosition + (minRange + maxRange) / 2, maxRange - minRange); // Draw the wireframe cube at the center of the range
    }
    #endregion

}
