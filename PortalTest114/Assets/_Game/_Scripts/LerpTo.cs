using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A simple lerp functionality that allows an object to smoothly move to a target position
/// Additional options for self resetting
/// </summary>
public class LerpTo : MonoBehaviour, IInteractable
{
    #region Public Variables
    public Vector3 targetPosition; // The position to lerp towards
    #endregion

    #region Inspector Variables
    [SerializeField] private float lerpSpeed = 1f; // Speed of the lerp
    [SerializeField] private float returnSpeed = 1f; // Speed of the return lerp
    [SerializeField] private Vector2 lerpAndReturnDelay; // Delay before starting the lerp and return lerp
    [SerializeField] private bool lockInPress = false; // If true, the lerp delay will no stop coroutines
    #endregion

    #region Private Variables
    private bool willSelfReset = false;
    private int activePressCount = 0; // Counter for active presses
    private Vector3 initialPosition; // The initial position of the object
    private Coroutine lerpCoroutine; // Coroutine for lerping the position
    #endregion

    #region Unity Events
    private void Start()
    {
        initialPosition = transform.position; // Store the initial position of the object
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// Starts the lerping process based on the interaction data.
    /// </summary>
    public void Interact(InteractableData data)
    {
        float _delay = 1; // A delay factor to control the lerping timing
        if (lerpCoroutine != null)
        {
            if (lockInPress)
                return; // If lockInPress is true, do not stop the coroutine

            StopCoroutine(lerpCoroutine); // Stop any existing lerp coroutine
            lerpCoroutine = null; // Reset the coroutine reference
            _delay = 0f; // Set delay to zero to immediately start the new lerp
        }

        if (lockInPress)
            willSelfReset = true; // If lockInPress is true, set willSelfReset to true to allow self-resetting

        // If the interaction is a press action, start lerping to the target position
        if (data.isPressed)
        {
            activePressCount++;
            lerpCoroutine = StartCoroutine(LerpThisTo(targetPosition + initialPosition, lerpSpeed, lerpAndReturnDelay.x * _delay)); // If not down, lerp to target position
        }
        else
        {
            activePressCount--;
        }

        // If the active press count is less than or equal to zero, reset the position
        if (activePressCount <= 0)
        {
            activePressCount = 0;
            lerpCoroutine = StartCoroutine(LerpThisTo(initialPosition, returnSpeed, lerpAndReturnDelay.y * _delay)); // If already down, lerp back to initial position
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Smoothly lerps the GameObject to a new position over time.
    /// Will self reset to initial position if willSelfReset is true.
    /// </summary>
    private IEnumerator LerpThisTo(Vector3 newPosition, float speed, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay before starting the lerp

        Vector3 _startPosition = transform.position; // Store the starting position for lerping
        float _timePassed = 0f; // Timer to track lerping progress
        while (_timePassed < 1f)
        {
            _timePassed += speed * Time.deltaTime;
            transform.position = Vector3.Lerp(_startPosition, newPosition, _timePassed); // Lerp towards the new position
            yield return null; // Wait for the next frame
        }

        transform.position = newPosition; // Ensure the final position is set to the target position
        lerpCoroutine = null; // Reset the coroutine reference after completion

        if (willSelfReset)
        {
            willSelfReset = false; // Reset the willSelfReset flag
            lerpCoroutine = StartCoroutine(LerpThisTo(initialPosition, returnSpeed, lerpAndReturnDelay.y)); // Lerp back to the initial position
        }
    }
    #endregion
}


#region Editor Code
#if UNITY_EDITOR
[CustomEditor(typeof(LerpTo))]
[CanEditMultipleObjects]
public class LerpPosition : Editor
{
    public void OnSceneGUI()
    {
        var lerper = (LerpTo)target; // Get the LerpTo component from the target object

        EditorGUI.BeginChangeCheck(); // Start checking for changes to the handle's position
        Vector3 newTargetPosition = Handles.PositionHandle(lerper.targetPosition + lerper.transform.position, Quaternion.identity); // Create a position handle at the target position

        if (EditorGUI.EndChangeCheck()) // If the position has changed
        {
            Undo.RecordObject(lerper, "Change Target Position"); // Record the change for undo functionality
            lerper.targetPosition = newTargetPosition; // Update the target position in the LerpTo component
        }
    }
}
#endif
#endregion

