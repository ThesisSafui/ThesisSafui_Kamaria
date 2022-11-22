using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class CheckOpen : MonoBehaviour
    {
        [SerializeField] private GameObject campEnemy;

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"Camp: {campEnemy.name} = Enter");
            campEnemy.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            //Debug.Log($"Camp: {campEnemy.name} = Exit");
            campEnemy.SetActive(false);
        }
    }
}