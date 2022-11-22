using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Utilities
{
    public sealed class SetPositonObj : MonoBehaviour
    {
        [SerializeField] private List<Transform> oldObjs;
        [SerializeField] private List<Transform> newObjs;
        [SerializeField] private bool move;
        
        private void OnValidate()
        {
            if (move)
            {
                Debug.Log("Move");
                for (int i = 0; i < oldObjs.Count; i++)
                {
                    newObjs[i].transform.position = oldObjs[i].transform.position;
                    newObjs[i].transform.rotation = oldObjs[i].transform.rotation;
                    newObjs[i].transform.localScale = oldObjs[i].transform.localScale;
                }
            }
        }
    }
}