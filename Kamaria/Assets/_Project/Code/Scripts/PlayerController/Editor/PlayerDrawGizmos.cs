using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.GUIEditor
{
    internal sealed class PlayerDrawGizmos : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        /*private void OnDrawGizmos()
        {
            #region GIZMOS_GROUNDED

            Gizmos.color = playerData.CharacterControllerData.IsGrounded
                ? playerData.CharacterControllerData.ColorGizmosGrounded
                : playerData.CharacterControllerData.ColorGizmosNotGrounded;
            Gizmos.DrawSphere(new Vector3(transform.position.x,
                this.transform.position.y - playerData.CharacterControllerData.GroundedOffset,
                this.transform.position.z), playerData.CharacterControllerData.GroundedRadius);

            #endregion

            #region GIZMOS_EXTRA_GRAVITY

            Gizmos.color = playerData.CharacterControllerData.ColorGizmosDoubleExtraGravity;
            Gizmos.DrawRay(new Vector3(transform.position.x,
                this.transform.position.y - playerData.CharacterControllerData.GroundedOffset,
                this.transform.position.z), Vector3.down * playerData.CharacterControllerData.RayExtraGravity);

            #endregion

            #region AIM
            
            if (playerData.CharacterControllerData.Aiming)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * 20);
            }

            #endregion
        }*/
    }
}