using UnityEngine;

public class Ambiance : MonoBehaviour
{
    public static Ambiance instance { get; private set; }

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        instance = this;
        // Ensure the GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
    }
}

