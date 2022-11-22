using System;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.VFX_ALL
{
    public sealed class VfxDeltaMouse : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private float offsetY;
        [SerializeField] private PlayerControllerInput playerControllerInput;
        
        private float limitDistance;
        private float distance;
        private Vector3 hitPosDir;
        private Vector3 newHitPos;

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var positionPlayer = playerControllerInput.gameObject.transform.position;

            limitDistance = playerData.CharacterControllerData.Aiming
                ? playerData.CharacterSkillData.LimitCannonAttackAim
                : playerData.CharacterSkillData.LimitCannonAttack;

            playerControllerInput.RaycastVfxDirection();

            FallTarget(playerData.CharacterControllerData.MouseVfxDirection, out Vector3 targetOut);

            hitPosDir = (playerData.CharacterControllerData.MouseVfxDirection - positionPlayer).normalized;
            distance = Vector3.Distance(playerData.CharacterControllerData.MouseVfxDirection, positionPlayer);
            distance = Mathf.Min(distance, limitDistance);
            newHitPos = positionPlayer + hitPosDir * distance;
            newHitPos.y = targetOut.y;

            transform.position = newHitPos;
        }

        private void FallTarget(Vector3 target, out Vector3 targetOut)
        {
            targetOut = new Vector3(target.x, target.y + offsetY, target.z);
        }
    }
}