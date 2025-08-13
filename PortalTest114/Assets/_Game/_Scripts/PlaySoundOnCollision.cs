using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Plays a sound when the object collides with another object, with support for random pitch variation and collision
/// filtering based on layers and corner proximity.
/// </summary>
public class PlaySoundOnCollision : MonoBehaviour
{
    #region Inspector Variables
    [Header("Sounds Setings")]
    [SerializeField] private AudioSource collisionSound; // Sound to play on collision
    [SerializeField] private LayerMask collisionLayerMask; // Layer mask to filter collisions
    [SerializeField] private Vector2 randomPitchRange = new Vector2(0.8f, 1.2f); // Range for random pitch variation
    [SerializeField] private float cornerTolerance = 0.1f; // Tolerance for corner detection
    #endregion

    #region Private Variables
    private Rigidbody rb; // Rigidbody component for kinematic checks
    private BoxCollider boxCollider; // BoxCollider component to determine corners of the object
    private Vector3[] corners; // Local positions of the corners of the BoxCollider
    private bool[] cornerTouching; // Array to track which corners are currently touching something
    #endregion

    #region Unity Events
    void Start()
    {
        // If we have a Rigidbody, start checking if it's kinematic
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            StartCoroutine(KinematicCheck());
        }

        // Calculate local positions of corners based on BoxCollider
        boxCollider = GetComponent<BoxCollider>();
        Vector3 _half = boxCollider.size * 0.5f;

        corners = new Vector3[8];
        int i = 0;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    corners[i++] = new Vector3(_half.x * x, _half.y * y, _half.z * z);
                }
            }
        }

        cornerTouching = new bool[8];
    }
    
    void OnCollisionStay(Collision collision)
    {
        // Go through each contact point in the collision
        foreach (ContactPoint _contact in collision.contacts)
        {
            // Get the local position of the contact point relative to this object
            Vector3 _localContact = transform.InverseTransformPoint(_contact.point);

            int _closestCornerIndex = -1;
            float _closestDist = Mathf.Infinity;

            // Find nearest corner to this contact
            for (int i = 0; i < corners.Length; i++)
            {
                float dist = Vector3.Distance(_localContact, corners[i]);
                if (dist < _closestDist)
                {
                    _closestDist = dist;
                    _closestCornerIndex = i;
                }
            }

            // If within tolerance and was not already touching, play sound
            if (_closestDist <= cornerTolerance && !cornerTouching[_closestCornerIndex])
            {
                collisionSound.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
                cornerTouching[_closestCornerIndex] = true;
                collisionSound.PlayOneShot(collisionSound.clip);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        ResetCorners();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Periodically checks if the Rigidbody is set to kinematic and resets the corners if has rigidbody.
    /// This is needed as when the player picks up the box, it becomes kinematic and we lose contact with corners.
    /// </summary>
    private IEnumerator KinematicCheck()
    {
        yield return new WaitForFixedUpdate();
        if (rb.isKinematic)
        {
            ResetCorners();
        }
        StartCoroutine(KinematicCheck());
    }

    /// <summary>
    /// Resets the state of all corners to indicate that no contact is being made.
    /// </summary>
    private void ResetCorners()
    {
        // Reset all corners when we lose contact entirely
        for (int i = 0; i < cornerTouching.Length; i++)
        {
            cornerTouching[i] = false;
        }
    }
    #endregion
}
