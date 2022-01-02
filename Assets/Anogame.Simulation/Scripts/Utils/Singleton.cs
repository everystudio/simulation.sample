using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
        static public bool IsExist()
        {
            return m_instance != null;
        }
        static T m_instance = null;
        static bool isShuttingDown = false;

        public static T Instance
        {
            get
            {
                if (isShuttingDown)
                {
                    return null;
                }

                if (m_instance != null)
                {
                    return m_instance;
                }

                System.Type type = typeof(T);

                T instance = GameObject.FindObjectOfType(type) as T;

                if (instance == null)
                {
                    string typeName = type.ToString();

                    GameObject gameObject = new GameObject(typeName, type);
                    instance = gameObject.GetComponent<T>();

                    if (instance == null)
                    {
                        Debug.LogError("Problem during the creation of " + typeName, gameObject);
                    }
                    else
                    {
                        Initialize(instance);
                    }
                }
                else
                {
                    Initialize(instance);
                }
                return instance;
            }
        }
        static void Initialize(T _instance)
        {
            if (m_instance == null)
            {
                m_instance = _instance;

                m_instance.Initialize();
                // んー、シーン内でのみシングルトンなものはstaticを使った方がいいのかな
                //m_instance.SetDontDestroy (true);
            }
            else if (m_instance != _instance)
            {
                DestroyImmediate(_instance.gameObject);
            }
        }

        public virtual void Initialize() { }
        public virtual void OnFinalize() { }

        static void Destroyed(T instance)
        {
            if (instance != null)
            {
                instance.OnFinalize();
                instance = null;
            }
        }

        public virtual void SetDontDestroy(bool _bFlag)
        {
            if (_bFlag)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public virtual void Awake()
        {
            Initialize(this as T);
        }

        void OnDestroy()
        {
            Destroyed(this as T);
        }

        void OnApplicationQuit()
        {
            isShuttingDown = true;
            Destroyed(this as T);
        }
    }
}
