using System.Collections;
using UnityEditor;
using UnityEngine;

public class LerpTo : MonoBehaviour, IInteractable
{
    public Vector3 targetPosition; // The position to lerp towards

    [SerializeField] private float lerpSpeed = 1f; // Speed of the lerp
    [SerializeField] private float returnSpeed = 1f; // Speed of the return lerp
    [SerializeField] private Vector2 lerpAndReturnDelay; // Delay before starting the lerp and return lerp
    [SerializeField] private bool lockInPress = false; // If true, the lerp delay will no stop coroutines
    [SerializeField] private bool canSelfLock = false; // If true, the lerp can be locked to prevent interruption by new interactions

    private bool isDown;
    private bool willSelfReset = false;
    private bool isLocked = false; // If true, the lerp will not be interrupted by new interactions
    private Vector3 initialPosition; // The initial position of the object
    private Coroutine lerpCoroutine; // Coroutine for lerping the position

    private void Start()
    {
        initialPosition = transform.position; // Store the initial position of the object
        isDown = false; // Initialise the isDown state


    }

    public void Interact(InteractableData data)
    {
        float _delay = 1;
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

        lerpCoroutine = isDown && !isLocked ? 
            StartCoroutine(LerpThisTo(initialPosition, returnSpeed, lerpAndReturnDelay.y * _delay)) : // If already down, lerp back to initial position
            StartCoroutine(LerpThisTo(targetPosition + initialPosition, lerpSpeed, lerpAndReturnDelay.x * _delay)); // If not down, lerp to target position

        isDown = !isDown; // Toggle the isDown state

        if (canSelfLock)
            isLocked = true;
    }

    public void DeInteract(InteractableData data)
    {
        if (!canSelfLock)
            return; // If canSelfLock is false, do not allow de-interaction

        Debug.Log("De-Interacting with LerpTo"); // Log de-interaction for debugging

        isLocked = false; // Reset the isLocked state when de-interacting

        lerpCoroutine = isDown ?
           StartCoroutine(LerpThisTo(initialPosition, returnSpeed, lerpAndReturnDelay.y)) : // If already down, lerp back to initial position
           StartCoroutine(LerpThisTo(targetPosition + initialPosition, lerpSpeed, lerpAndReturnDelay.x)); // If not down, lerp to target position

        isDown = !isDown; // Toggle the isDown state
    }

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
            isDown = false; // Reset the isDown state
            lerpCoroutine = StartCoroutine(LerpThisTo(initialPosition, returnSpeed, lerpAndReturnDelay.y)); // Lerp back to the initial position
        }

    }
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

