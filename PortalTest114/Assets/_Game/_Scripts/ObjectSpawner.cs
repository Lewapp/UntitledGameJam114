using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Used to spawn a GameObject at a specified position when interacted with.
/// Should be attached to a button in the scene.
/// </summary>
public class ObjectSpawner : MonoBehaviour, IInteractable
{
    #region Public Variables
    [Header("Spawner Settings")]
    public Vector3 spawnPoint; // The position where the object will be spawned
    #endregion

    #region Inspector Variables
    [SerializeField] private GameObject spawnObject; // The object to spawn when interacted with
    [SerializeField] private int maxSpawns; // Maximum number of objects that can be spawned at once (not currently used)
    [SerializeField] private bool lockRotation = false; // If true, the spawned object will have its rotation locked to the spawner's rotation

    [Header("Audio Settings")]
    [SerializeField] private AudioClip interactSound; // Sound to play when the object is spawned
    [SerializeField] private AudioSource audioSource; // AudioSource to play the interact sound
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f); // Range of pitch variation for the interact sound
    #endregion

    #region Private Variables
    private List<GameObject> spawned;
    #endregion

    #region Unity Events
    private void Start()
    {
        spawned = new List<GameObject>(); // Initialize the list to hold spawned objects

        if (audioSource && StaticSFX.instance)
        {
            StaticSFX.instance.InitialiseNewSource(audioSource); // Register this AudioSource with the StaticSFXManager
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Checks to see if there are any spawned objects and removes the oldest one if the count exceeds maxSpawns.
    /// </summary>
    private void DeSpawnSpawned()
    {
        // Check if there are any spawned objects and if the count exceeds the maximum allowed spawns
        if (spawned.Count >= maxSpawns && spawned.Count > 0)
        {
            Destroy(spawned[0]);
            spawned.RemoveAt(0); // Remove the oldest spawned object if the count exceeds maxSpawns
        }
    }
    #endregion

    #region Interfaces
    public void Interact(InteractableData data)
    {
        if (!spawnObject || maxSpawns <= 0)
            return;

        if (audioSource && interactSound)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y); // Randomise the pitch within the specified range
            audioSource.PlayOneShot(interactSound); // Play the interact sound if an AudioSource and sound are set
        }


        DeSpawnSpawned(); // De-spawn any previously spawned object
        spawned.Add(Instantiate(spawnObject, transform.TransformPoint(spawnPoint), Quaternion.identity)); // Spawn the new object at the specified spawn point

        if (!lockRotation)
            return;

        // If lockRotation is true, lock the rotation of the spawned object
        Rigidbody _rb = spawned[spawned.Count - 1].GetComponent<Rigidbody>();
        if (_rb)
            _rb.constraints = RigidbodyConstraints.FreezeRotation; // Lock the rotation of the spawned object if lockRotation is true
    }
    #endregion
}

#region Editor Code
#if UNITY_EDITOR
[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        // Ensure the target is of type ObjectSpawner
        var _spawner = (ObjectSpawner)target;

        Vector3 _position = _spawner.transform.TransformPoint(_spawner.spawnPoint);

        // Draw a wire cube at the spawn point in the scene view
        Handles.color = Color.yellow;
        Handles.DrawWireCube(_position, Vector3.one * 0.5f);

        EditorGUI.BeginChangeCheck();
        // Allow the user to move the spawn point using a position handle
        Vector3 _newSpawnPoint = Handles.PositionHandle(_position, Quaternion.identity);

        // If the position has changed, update the spawn point in the spawner
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spawner, "Move Spawn Point");
            _spawner.spawnPoint = _spawner.transform.InverseTransformPoint(_newSpawnPoint);
            EditorUtility.SetDirty(_spawner);
        }
    }
}
#endif
#endregion
