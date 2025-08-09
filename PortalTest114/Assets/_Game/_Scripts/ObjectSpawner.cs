using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectSpawner : MonoBehaviour, IInteractable
{
    [Header("Spawner Settings")]
    public Vector3 spawnPoint; // The position where the object will be spawned
    public GameObject spawnObject; // The object to spawn when interacted with
    public int maxSpawns; // Maximum number of objects that can be spawned at once (not currently used)

    private List<GameObject> spawned;

    private void Start()
    {
        spawned = new List<GameObject>(); // Initialize the list to hold spawned objects
    }

    public void Interact(InteractableData data)
    {
        if (!spawnObject || maxSpawns <= 0)
            return;

        DeSpawnSpawned(); // De-spawn any previously spawned object
        spawned.Add(Instantiate(spawnObject, transform.position + spawnPoint, Quaternion.identity)); // Spawn the new object at the specified spawn point
    }

    private void DeSpawnSpawned()
    {
        // Check if there are any spawned objects and if the count exceeds the maximum allowed spawns
        if (spawned.Count >= maxSpawns && spawned.Count > 0)
        {
            Destroy(spawned[0]);
            spawned.RemoveAt(0); // Remove the oldest spawned object if the count exceeds maxSpawns
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        var _spawner = (ObjectSpawner)target;

        Vector3 _position = _spawner.transform.position + _spawner.spawnPoint;

        Handles.color = Color.yellow;
        Handles.DrawWireCube(_position, Vector3.one * 0.5f);

        EditorGUI.BeginChangeCheck();
        Vector3 _newSpawnPoint = Handles.PositionHandle(_position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spawner, "Move Spawn Point");
            _spawner.spawnPoint = _newSpawnPoint - _spawner.transform.position; 
            EditorUtility.SetDirty(_spawner);
        }
    }
}
#endif
