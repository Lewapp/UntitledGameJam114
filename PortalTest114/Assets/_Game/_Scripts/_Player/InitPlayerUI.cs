using UnityEngine;

public class InitPlayerUI : MonoBehaviour
{
    public UIFade fader;

    void Start()
    {
        if (fader)
        {
            fader.ReverseFade();
        }
    }

}
