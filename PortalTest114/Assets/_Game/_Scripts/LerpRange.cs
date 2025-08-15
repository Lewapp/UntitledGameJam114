using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class LerpRange : MonoBehaviour, IInteractable
{
    [Header("Set Up Settings")]
    [SerializeField] private bool setCentreAsPosition = true; // If true, the central position will be set to the GameObject's position
    [SerializeField] private Vector3 centralPosition;
    [SerializeField] private Vector3 minRange;
    [SerializeField] private Vector3 maxRange;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask ignoreMask; // Layer mask to ignore certain layers during movement checks
    [SerializeField] private Collider checkCollider; 

    private List<Transform> attachedObjects;
    private Vector3 lastPosition;
    private Coroutine currentMovement;

    private void Start()
    {
        attachedObjects = new List<Transform>();
    }

    private void FixedUpdate()
    {
        Vector3 _delta = transform.position - lastPosition;

        foreach (Transform ao in attachedObjects)
        {
            if (!ao)
                continue;

            ao.position += _delta;
        }

        lastPosition = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (attachedObjects.Contains(other.transform))
            return;

        attachedObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (attachedObjects.Contains(other.transform))
            attachedObjects.Remove(other.transform);
    }

    public void Interact(InteractableData data)
    {
        if (!data.isPressed)
        {
            if (currentMovement != null)
                StopCoroutine(currentMovement);

            currentMovement = null;
            return;
        }

        Vector3 direction = Vector3.zero;

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

    private bool IsWithinRange(Vector3 position)
    {
        Vector3 _min = centralPosition + minRange;
        Vector3 _max = centralPosition + maxRange;

        return position.x >= _min.x && position.x <= _max.x &&
               position.y >= _min.y && position.y <= _max.y &&
               position.z >= _min.z && position.z <= _max.z;
    }

    private bool CanMoveTo(Vector3 targetPos)
    {
        Vector3 halfExtents = new Vector3(checkCollider.bounds.extents.z * 1.1f, checkCollider.bounds.extents.z * 1.1f, checkCollider.bounds.extents.z * 1.1f);
        Vector3 targetPosition = targetPos;

        return !Physics.CheckBox(
            targetPos,
            halfExtents,
            transform.rotation,
            ~ignoreMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private IEnumerator SmoothMove(Vector3 direction)
    {
        yield return new WaitForEndOfFrame();

        while (currentMovement != null)
        {
            float _duration = 0.2f; // time in seconds to complete movement
            float _elapsed = 0f;

            Vector3 _startPos = transform.position;
            Vector3 _endPos = _startPos + direction * moveSpeed; // Calculate the end position based on the direction and speed

            bool _isWithinRange = IsWithinRange(_endPos);
            bool _canMove = CanMoveTo(_endPos);

            while (_elapsed < _duration && _isWithinRange && _canMove)
            {
                transform.position = Vector3.Lerp(_startPos, _endPos, _elapsed / _duration);
                _elapsed += Time.deltaTime;
                yield return null;
                _canMove = CanMoveTo(_endPos);
            }

            transform.position = _isWithinRange && _canMove ? _endPos : _startPos;
            yield return null; // Wait until the current movement is finished
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (setCentreAsPosition)
        {
            centralPosition = transform.position; // Set the central position to the GameObject's position
        }

        // Draw a wireframe cube to visualize the range in the editor
        Gizmos.color = Color.green; // Set the color of the Gizmos
        Gizmos.DrawWireCube(centralPosition + (minRange + maxRange) / 2, maxRange - minRange); // Draw the wireframe cube at the center of the range
    }

}
