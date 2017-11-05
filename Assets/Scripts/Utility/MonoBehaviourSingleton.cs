using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A single-instance MonoBehaviour which can be accessed directly.
/// Create subclasses like this:
/// public class Test : MonoBehaviourSingleton<Test> ...
/// </summary>
[DisallowMultipleComponent]
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {
    private static T _instance;

    public static T Instance {
        get { return _instance; }
    }

    public static bool HasInstance {
        get { return _instance != null; }
    }

    protected virtual void Awake() {
        if (_instance != null) Debug.LogError("[MonoBehaviourSingleton] Instance of " + typeof(T) + " already created", this);
        _instance = (T) this;
    }

    protected virtual void OnDestroy() {
        _instance = null;
    }
}
