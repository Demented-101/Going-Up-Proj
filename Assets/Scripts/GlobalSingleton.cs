using UnityEngine;

public class GlobalSingleton : MonoBehaviour
{
    public static GlobalSingleton Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }
}
