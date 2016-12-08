using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {

    public static T Instance { get; private set; }

    public static bool HasInstance { get { return Instance != null; } }

    void Awake() { Initialize(); }
    void OnEnable() { Initialize(); }

	private void Initialize()
    {
        if(Instance == this)
        {

        }
        else if (HasInstance && Instance != (this as T))
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
