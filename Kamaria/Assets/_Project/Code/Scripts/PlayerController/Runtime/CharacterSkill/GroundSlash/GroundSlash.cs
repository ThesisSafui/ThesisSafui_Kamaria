using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Kamaria.Player.Controller
{
    public sealed class GroundSlash : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float speed = 30;
        [SerializeField] private float slowDownRate = 0.01f;
        [SerializeField] private float slowDownTime = 1.25f;
        [SerializeField] private float detectingDistance = .1f;
        [SerializeField] private float lifetime;
        [SerializeField] private float destroyDelay;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private VisualEffect visualEffect;
        [SerializeField] private BoxCollider collider;

        private ExposedProperty propertyLifeTime = "Lifetime";
        private bool stopped;

        private void OnEnable()
        {
            visualEffect.SetFloat(propertyLifeTime, lifetime);
            stopped = false;
            collider.enabled = true;

            StartCoroutine(SlowDown());
            StartCoroutine(TimeCollider());
            StartCoroutine(TimeGroundSlash());
        }

        public void Init(Vector3 direction)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            
            rb.velocity = direction * speed;
        }

        private void FixedUpdate()
        {
            if (stopped) return;
            
            if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up),
                    out var hit, detectingDistance, groundLayer)) return;
                
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        

        private IEnumerator TimeGroundSlash()
        {
            yield return new WaitForSeconds(destroyDelay);
            gameObject.SetActive(false);
        }
        
        private IEnumerator TimeCollider()
        {
            yield return new WaitForSeconds(lifetime);
            collider.enabled = false;
        }
        
        private IEnumerator SlowDown()
        {
            float time = slowDownTime;
            
            while (time > 0)
            {
                rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, time);
                time -= slowDownRate;
                yield return new WaitForSeconds(slowDownRate);
            }

            stopped = true;
        }
    }
}
