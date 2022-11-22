using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.VFX_ALL
{
    public sealed class VfxShockWave : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float lifeTime;

        private float fallPoint;
        
        public void Init(Vector3 spawn)
        {
            SetOffset(spawn);
            Invoke(nameof(Close), lifeTime);
        }

        private void SetOffset(Vector3 target)
        {
            if (Physics.Raycast(target,
                    Vector3.down,out RaycastHit hit,playerData.CharacterControllerData.GroundLayers))
            {
                fallPoint = hit.point.y;
            }

            Vector3 transformPosition = transform.position;
            transformPosition.y = fallPoint;
            transformPosition += offset;
            this.transform.position = transformPosition;
        }

        private void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}