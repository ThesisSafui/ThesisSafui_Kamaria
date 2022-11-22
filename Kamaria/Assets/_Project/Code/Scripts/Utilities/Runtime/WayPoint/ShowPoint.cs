using UnityEngine;

namespace Kamaria.Utilities.WayPoint
{
    public sealed class ShowPoint : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Color color;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
#endif
    }
}