using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Hellper
{
    public class ColliderHelper
    {
        public static bool HasElastic(Collider2D col)
        {
            return col.CompareTag("Box");
        }
    }
}