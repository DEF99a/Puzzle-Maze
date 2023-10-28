using Assets.Scripts.Define;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public struct ItemParam
    {
        public int characterId;
        public CharacterBase characterBase;
        public DynamicEntityDirection direct;
        public float speed;
        //public int damage;
        //public Location location;
    }
}