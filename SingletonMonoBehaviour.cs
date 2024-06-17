using UnityEngine;

namespace DefaultNamespace.Common
{
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("The instance of " + typeof(T) + " already exists.",Instance.gameObject);
            Debug.LogError("The new instance is",this.gameObject);
            return;
        }

        Instance = this as T;
    }
}
}