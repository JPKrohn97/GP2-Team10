using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroy = false;

    static T m_Instance;
    public static T Instance {
        get {
            if (m_Instance == null) {
                m_Instance = GameObject.FindFirstObjectByType
                    <T>();
                // if (m_Instance == null) {
                //     GameObject singleton = new GameObject(typeof(T).Name);
                //     m_Instance = singleton.AddComponent<T>();
                // }

            }
            return m_Instance;
        }
    }

    protected virtual void Awake() {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            if (dontDestroy)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}