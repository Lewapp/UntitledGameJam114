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
    public GameObject spawnObject; // The object to spawn when interacted with
    public int maxSpawns; // Maximum number of objects that can be spawned at once (not currently used)
    #endregion

    #region Private Variables
    private List<GameObject> spawned;
    #endregion

    #region Unity Events
    private void Start()
    {
        spawned = new List<GameObject>(); // Initialize the list to hold spawned objects
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

        DeSpawnSpawned(); // De-spawn any previously spawned object
        spawned.Add(Instantiate(spawnObject, transform.TransformPoint(spawnPoint), Quaternion.identity)); // Spawn the new object at the specified spawn point
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
        var _spawner = (ObjectSpawner)target;

        Vector3 _position = _spawner.transform.TransformPoint(_spawner.spawnPoint);

        Handles.color = Color.yellow;
        Handles.DrawWireCube(_position, Vector3.one * 0.5f);

        EditorGUI.BeginChangeCheck();
        Vector3 _newSpawnPoint = Handles.PositionHandle(_position, Quaternion.identity);
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
