using UnityEngine;

/// Dead simple way to create singletons
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;

    public static T Instance
    {
        get
        {
            if (s_instance == null)
            {
                // Search for existing instance.
                s_instance = (T)FindObjectOfType(typeof(T));

                // Create new instance if one doesn't already exist.
                if (s_instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    s_instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                }
            }

            return s_instance;
        }
    }

    public static bool HasInstance => s_instance != null;
}