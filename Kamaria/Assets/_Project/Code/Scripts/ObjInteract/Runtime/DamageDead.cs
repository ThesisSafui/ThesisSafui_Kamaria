using Cinemachine;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class DamageDead : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;

            target.TakeDamage(99999, EffectAttack.None, transform.position, 0, 0, false, 0, WeaponTypes.None,
                KeyStones.None);
        }
    }
}