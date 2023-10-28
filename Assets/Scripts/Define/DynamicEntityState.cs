using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Define
{
    public enum DynamicEntityState
    {
        None,
        Idle,
        Walk,
        Shoot,
        Action,// khạc lửa, nhả bẫy

        // substate
        PrepareReborn,
        Invisible,
        Ice,
        Angry,

        AutoMove = 254,
        Die = 255
    }
}