using System;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class ObjMove : MonoBehaviour
    {
        [SerializeField] private Vector3 offsetTarget;
        [SerializeField] private float duration;
        [SerializeField] private float timeAgain;
    }
}