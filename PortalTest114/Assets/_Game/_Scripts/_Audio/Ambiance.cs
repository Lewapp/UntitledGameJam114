using UnityEngine;

public class Ambiance : MonoBehaviour
{
    private void Awake()
    {
        // Ensure the GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
    }
}

