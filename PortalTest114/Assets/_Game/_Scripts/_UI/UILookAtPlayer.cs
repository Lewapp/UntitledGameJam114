using UnityEngine;

/// <summary>
/// Rotates a UI element to face the player in the game world.
/// </summary>
public class UILookAtPlayer : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private float rotationSpeed = 5f; // Speed at which the UI will rotate to face the player
    [SerializeField] private float rotationOffset = 0f; // Optional offset to the rotation, can be used to adjust the UI's facing direction
    #endregion

    #region Private Varibales
    private Transform player; //Reference to the player's transform
    #endregion

    #region Unity Events
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player GameObject by tag and get its transform
    }

    private void Update()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = player.position - transform.position;

            // Calculate the rotation needed to look at the player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            targetRotation *= Quaternion.Euler(0, rotationOffset, 0); // Apply the rotation offset

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion
}
