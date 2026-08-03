using UnityEngine;

namespace VNMaker.Singletons
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (!TryGetComponent<T>(out Instance))
            {
                Instance = gameObject.AddComponent<T>();
            }
        }
    }
}