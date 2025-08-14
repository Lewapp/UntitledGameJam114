using TMPro;
using UnityEngine;

/// <summary>
/// Rotates a UI element to face the player in the game world.
/// </summary>
public class UILookAtPlayer : MonoBehaviour
{
    #region Inspector Variables
    [Header("Look Settings")]
    [SerializeField] private float rotationSpeed = 5f; // Speed at which the UI will rotate to face the player
    [SerializeField] private float rotationOffset = 0f; // Optional offset to the rotation, can be used to adjust the UI's facing direction

    [Header("Fade Settings")]
    [SerializeField] private float minDistance = 2f; // Minimum distance at which the UI will be fully visible
    [SerializeField] private float maxDistance = 10f; // Maximum distance at which the UI will be visible to the player
    [SerializeField] private TextMeshProUGUI[] textMeshPros; // Array of TextMeshProUGUI components to adjust the alpha based on distance
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
            // Text Look
            // Calculate the direction to the player
            Vector3 _directionToPlayer = player.position - transform.position;

            // Calculate the rotation needed to look at the player
            Quaternion _targetRotation = Quaternion.LookRotation(_directionToPlayer);
            _targetRotation *= Quaternion.Euler(0, rotationOffset, 0); // Apply the rotation offset

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);

            // Text Fade
            // Calculate the distance to the player
            float _distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Calculate the alpha value based on the distance
            float _alpha = Mathf.Clamp01((_distanceToPlayer - minDistance) / (maxDistance - minDistance));

            // Adjust the alpha of each TextMeshProUGUI component
            foreach (var _tmp in textMeshPros)
            {
                if (_tmp == null)
                    continue;

                Color _colour = _tmp.color; // Get the current color of the TextMeshProUGUI component
                _colour.a = 1f - _alpha; // Adjust the alpha value based on the distance
                _tmp.color = _colour; // Apply the new color with adjusted alpha
            }
        }
    }
    #endregion
}
