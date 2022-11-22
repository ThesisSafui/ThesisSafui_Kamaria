using UnityEngine;

namespace Kamaria.UIDamage
{
    public sealed class UILookCamera : MonoBehaviour
    {
        private Camera camera;
        
        private void OnEnable()
        {
            camera = Camera.main;
        }
        
        private void Update()
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
                camera.transform.rotation * Vector3.up);
        }
    }
}