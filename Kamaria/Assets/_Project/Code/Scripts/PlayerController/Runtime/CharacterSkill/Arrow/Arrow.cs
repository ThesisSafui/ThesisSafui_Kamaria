using System;
using UnityEngine;
using System.Collections;
using Kamaria.Player.Data;

namespace Kamaria.Player.Controller.Arrow
{
    public sealed class Arrow : MonoBehaviour
    {
        [SerializeField] private float speed = 30;
        [SerializeField] private float destroyDelay;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject arrowNormal;
        [SerializeField] private GameObject arrowFull;

        private void OnEnable()
        {
            StartCoroutine(TimeArrow());
        }

        public void Init(Vector3 direction, bool isChargeFull)
        {
            if (isChargeFull)
            {
                arrowNormal.SetActive(false);
                arrowFull.SetActive(true);
            }
            else
            {
                arrowNormal.SetActive(true);
                arrowFull.SetActive(false);
            }
            
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            
            rb.velocity = direction * speed;
        }
        
        private IEnumerator TimeArrow()
        {
            yield return new WaitForSeconds(destroyDelay);
            gameObject.SetActive(false);
        }
    }
}