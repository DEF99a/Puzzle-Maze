using System.Collections;
using UnityEngine;

namespace Utilities.Components
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            if (transform.childCount > 0)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
        }
    }
}